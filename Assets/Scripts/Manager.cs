using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Beginning, Ready, Play, End }

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

	[SerializeField] AudioClip m_WinSound;
	[SerializeField] AudioClip m_LoseSound;

	[SerializeField] AudioSource m_AudioSource;

	[Space(10)]

	[SerializeField] private int m_ParentStartHeartLevel = 5;

	private bool[] HasPressed { get; set; }
	private bool AllPressed { get; set; }
	private GameState State { get; set; }

	private float ReadyStateStartedTime { get; set; }

	private float EndStateStartedTime { get; set; }

	private bool[] WasPressing { get; set; }

	private bool LoserShown { get; set; }
	private bool WinnerShown { get; set; }

	private void Start() {
		m_StartScreen.SetActive(false);
		m_EndScreen.SetActive(false);
		this.State = GameState.Beginning;
		this.HasPressed = new bool[2];
		this.WasPressing = new bool[2];
		Reset();
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
				if (this.AllPressed) {
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
							otherParent.DecreaseHeartLevel();
							m_Baby.Happy();
						} else if (m_Baby.State != BabyState.Angry) {
							if (!this.WasPressing[i]) {
								parent.DecreaseHeartLevel();
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
					this.State = GameState.End;
					this.EndStateStartedTime = Time.time;
					ResetAllPressed();
					m_EndScreen.SetActive(true);
					m_LeftLoser.SetActive(false);
					m_RightLoser.SetActive(false);
					m_LeftWinner.SetActive(false);
					m_RightWinner.SetActive(false);
				}
				break;
			case GameState.End:
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
					if (this.AllPressed) {
						this.State = GameState.Ready;
						Reset();
						this.LoserShown = false;
						this.WinnerShown = false;
					}
				}
				break;
		}
	}

	private void Reset() {
		this.AllPressed = false;
		for (int i = 0; i < m_Parents.Length; i++) {
			this.WasPressing[i] = false;
			this.HasPressed[i] = false;
			m_Parents[i].InitializeParent(m_ParentStartHeartLevel);
		}
		m_Baby.InitializeBaby();
	}

	private void ResetAllPressed() {
		for (int i = 0; i < m_Parents.Length; i++) {
			this.HasPressed[i] = false;
		}
	}

}
