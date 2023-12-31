using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Beginning, Ready, Play, Results, End }

public class Manager : MonoBehaviour {

	[SerializeField] Baby m_Baby;
	[SerializeField] Parent[] m_Parents;

	[SerializeField] HeartManager m_HeartManager;

	[SerializeField] GameObject m_StartScreen;
	[SerializeField] GameObject m_EndScreen;

	[SerializeField] GameObject[] m_LeftParentHeldIndicators;
	[SerializeField] GameObject[] m_RightParentHeldIndicators;

	[SerializeField] GameObject m_LeftWinner;
	[SerializeField] GameObject m_RightWinner;

	[SerializeField] GameObject m_LeftLoser;
	[SerializeField] GameObject m_RightLoser;

	[SerializeField] GameObject m_EndReadyHolder;

	[SerializeField] AudioClip m_WinSound;
	[SerializeField] AudioClip m_LoseSound;

	[SerializeField] AudioSource m_AudioSource;

	[SerializeField] AudioClip m_GameLoop;
	[SerializeField] AudioClip m_StartMusic;
	[SerializeField] AudioSource m_MusicSource;

	[Space(10)]

	[SerializeField] private int m_ParentStartHeartLevel = 5;

	private bool[] HasPressed { get; set; }
	private bool AllPressed { get; set; }
	private GameState State { get; set; }

	private float ReadyStateStartedTime { get; set; }
	private float BeginningStateStartedTime { get; set; }

	private float EndStateStartedTime { get; set; }

	private bool[] WasPressing { get; set; }

	private bool LoserShown { get; set; }
	private bool WinnerShown { get; set; }

	private void Start() {
		m_StartScreen.SetActive(false);
		m_EndScreen.SetActive(false);
		PlayMusic(m_StartMusic);
		this.State = GameState.Beginning;
		this.BeginningStateStartedTime = Time.time;
		this.HasPressed = new bool[2];
		this.WasPressing = new bool[2];
		Reset();
		ResetAllPressed();
	}

	private void Update() {
		bool hasAllPressed = true;
		for (int i = 0; i < m_Parents.Length; i++) {
			m_Parents[i].UpdateParent();
			if (m_Parents[i].IsGrabbing) {
				this.HasPressed[i] = true;
			}
			if (!this.HasPressed[i]) {
				hasAllPressed = false;
			}
		}
		foreach (GameObject obj in m_LeftParentHeldIndicators) {
			obj.SetActive(this.HasPressed[0]);
		}
		foreach (GameObject obj in m_RightParentHeldIndicators) {
			obj.SetActive(this.HasPressed[1]);
		}
		this.AllPressed = hasAllPressed;

		switch (this.State) {
			case GameState.Beginning:
				m_StartScreen.SetActive(true);
				if (Time.time - this.BeginningStateStartedTime > 1f && this.AllPressed) {
					this.ReadyStateStartedTime = Time.time;
					this.State = GameState.Ready;
				}
				break;
			case GameState.Ready:
				if (Time.time - this.ReadyStateStartedTime > 1f) {
					ResetAllPressed();
					for (int i = 0; i < m_Parents.Length; i++) {
						this.WasPressing[i] = m_Parents[i].IsGrabbing;
					}
					m_StartScreen.SetActive(false);
					m_EndScreen.SetActive(false);
					this.State = GameState.Play;
					PlayMusic(m_GameLoop);
				}
				break;
			case GameState.Play:
				m_Baby.UpdateBaby();
				bool shouldEndGame = false;
				for (int i = 0; i < m_Parents.Length; i++) {
					Parent parent = m_Parents[i];
					Parent otherParent = m_Parents[(i + 1) % 2];
					if (parent.IsGrabbing) {
						if (m_Baby.IsCrying) {
							parent.IncreaseHeartLevel();
							parent.HeartParticles.Play();
							otherParent.DecreaseHeartLevel();
							m_Baby.Happy();
						} else if (m_Baby.State != BabyState.Angry) {
							if (!this.WasPressing[i]) {
								parent.DecreaseHeartLevel();
								parent.AngryParticle.Play();
								otherParent.IncreaseHeartLevel();
								m_Baby.Angry();
							}
						}
					}
					this.WasPressing[i] = parent.IsGrabbing;
					if (parent.HeartLevel >= 10) {
						shouldEndGame = true;
					}
				}
				m_HeartManager.SetColors(m_Parents[0].HeartLevel, m_Parents[0].HeartColor, m_Parents[1].HeartColor);
				if (shouldEndGame) {
					this.State = GameState.Results;
					m_EndReadyHolder.SetActive(false);
					this.EndStateStartedTime = Time.time;
					PlayMusic(m_StartMusic);
					ResetAllPressed();
					m_EndScreen.SetActive(true);
					m_LeftLoser.SetActive(false);
					m_RightLoser.SetActive(false);
					m_LeftWinner.SetActive(false);
					m_RightWinner.SetActive(false);
				}
				break;
			case GameState.Results:
				if (Time.time - this.EndStateStartedTime < 1) {
					//nothing
				} else if (Time.time - this.EndStateStartedTime > 1 && !this.LoserShown) {
					//show loser
					m_AudioSource.PlayOneShot(m_LoseSound);
					if (m_Parents[0].HeartLevel == 10) {
						m_RightLoser.SetActive(true);
					} else {
						m_LeftLoser.SetActive(true);
					}
					this.LoserShown = true;
				} else if (Time.time - this.EndStateStartedTime > 3 && !this.WinnerShown) {
					//show winner
					m_AudioSource.PlayOneShot(m_WinSound);
					if (m_Parents[0].HeartLevel == 10) {
						m_LeftWinner.SetActive(true);
					} else {
						m_RightWinner.SetActive(true);
					}
					this.WinnerShown = true;
				} else if (Time.time - this.EndStateStartedTime > 5) {
					this.State = GameState.End;
					m_EndReadyHolder.SetActive(true);
					ResetAllPressed();
				}
				break;
			case GameState.End:
				if (this.AllPressed) {
					m_EndScreen.SetActive(false);
					this.ReadyStateStartedTime = Time.time;
					this.State = GameState.Beginning;
					this.BeginningStateStartedTime = Time.time;
					ResetAllPressed();
					Reset();
					this.LoserShown = false;
					this.WinnerShown = false;
				}
				break;
		}
	}

	private void Reset() {
		for (int i = 0; i < m_Parents.Length; i++) {
			this.WasPressing[i] = false;
			m_Parents[i].InitializeParent(m_ParentStartHeartLevel);
		}
		m_Baby.InitializeBaby();
	}

	private void ResetAllPressed() {
		this.AllPressed = false;
		for (int i = 0; i < m_Parents.Length; i++) {
			this.HasPressed[i] = false;
		}
	}

	private void PlayMusic(AudioClip clip) {
		m_MusicSource.Stop();
		m_MusicSource.clip = clip;
		m_MusicSource.Play();
	}

}
