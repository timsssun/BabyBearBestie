using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Beginning, Play, End }

public class Manager : MonoBehaviour {

	[SerializeField] Baby m_Baby;
	[SerializeField] Parent[] m_Parents;

	[SerializeField] GameObject m_StartScreen;
	[SerializeField] GameObject m_EndScreen;

	[Space(10)]

	[SerializeField] private int m_ParentStartHeartLevel = 5;

	private bool[] HasPressed { get; set; }
	private bool AllPressed { get; set; }
	private GameState State { get; set; }

	private void Start() {
		m_StartScreen.SetActive(false);
		m_EndScreen.SetActive(false);
		this.State = GameState.Beginning;
		this.HasPressed = new bool[2];
		this.AllPressed = false;
		for (int i = 0; i < m_Parents.Length; i++) {
			this.HasPressed[i] = false;
			m_Parents[i].SetHeartLevel(m_ParentStartHeartLevel);
		}
	}

	private void Update() {
		bool hasAllPressed = true;
		for (int i = 0; i < m_Parents.Length; i++) {
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
				m_EndScreen.SetActive(false);
				if (this.AllPressed) {
					ResetAllPressed();
					this.State = GameState.Play;
				}
				break;
			case GameState.Play:
				for (int i = 0; i < m_Parents.Length; i++) {
					Parent parent = m_Parents[i];
					if (parent.IsGrabbing) {
						if (m_Baby.IsCrying) {
							parent.IncreaseHeartLevel();
							m_Baby.Happy();
						} else {
							parent.DecreaseHeartLevel();
							m_Baby.Angry();
						}
					}
					if (parent.HeartLevel >= 10) {
						this.State = GameState.End;
					}
				}
				break;
			case GameState.End:
				m_StartScreen.SetActive(false);
				m_EndScreen.SetActive(true);
				if (this.AllPressed) {
					ResetAllPressed();
					this.State = GameState.Beginning;
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
