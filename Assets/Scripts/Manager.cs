using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Beginning, Play, End }

public class Manager : MonoBehaviour {

	[SerializeField] Baby m_Baby;
	[SerializeField] Parent[] m_Parents;

	[SerializeField] HeartManager m_HeartManager;

	[SerializeField] GameObject m_StartScreen;
	[SerializeField] GameObject m_EndScreen;

	[Space(10)]

	[SerializeField] private int m_ParentStartHeartLevel = 5;

	private bool[] HasPressed { get; set; }
	private bool AllPressed { get; set; }
	private GameState State { get; set; }

	private bool[] WasPressing { get; set; }

	private void Start() {
		m_StartScreen.SetActive(false);
		m_EndScreen.SetActive(false);
		this.State = GameState.Beginning;
		this.HasPressed = new bool[2];
		this.WasPressing = new bool[2];
		this.AllPressed = false;
		for (int i = 0; i < m_Parents.Length; i++) {
			this.WasPressing[i] = false;
			this.HasPressed[i] = false;
			m_Parents[i].InitializeParent(m_ParentStartHeartLevel);
		}
		m_Baby.UpdateBaby();
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
		this.AllPressed = hasAllPressed;

		switch (this.State) {
			case GameState.Beginning:
				m_StartScreen.SetActive(true);
				if (this.AllPressed) {
					m_StartScreen.SetActive(false);
					ResetAllPressed();
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
							m_Baby.Relax();
						} else {
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
				}
				break;
			case GameState.End:
				m_EndScreen.SetActive(true);
				if (this.AllPressed) {
					ResetAllPressed();
					this.State = GameState.Beginning;
					m_EndScreen.SetActive(false);
				}
				break;
		}
	}

	private void ResetAllPressed() {
		for (int i = 0; i < m_Parents.Length; i++) {
			this.HasPressed[i] = false;
		}
	}

}
