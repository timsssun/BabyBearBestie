using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ParentState { Idle, Grabbing }

public class Parent : MonoBehaviour {

	[SerializeField] private KeyCode m_InputKey;

	[Space(10)]

	[SerializeField] private Color m_HeartColor;

	[SerializeField] private Animator m_Animator;

	[SerializeField] private GameObject m_IdleIndicator;
	[SerializeField] private GameObject m_GrabIndicator;

	[Space(10)]

	[SerializeField] TextMeshPro m_LevelText;

	private ParentState State { get; set; }
	public int HeartLevel { get; private set; }
	public Color HeartColor => m_HeartColor;

	public bool IsGrabbing {
		get { return this.State == ParentState.Grabbing; }
	}

	public void InitializeParent(int level) {
		SetHeartLevel(level);
		Relax();
	}

	public void UpdateParent() {
		if (Input.GetKeyDown(m_InputKey)) {
			Grab();
		}
		if (Input.GetKeyUp(m_InputKey)) {
			Relax();
		}
	}

	private void Grab() {
		this.State = ParentState.Grabbing;
		SetVisuals();
	}

	private void Relax() {
		this.State = ParentState.Idle;
		SetVisuals();
	}

	private void SetVisuals() {
		m_IdleIndicator.SetActive(false);
		m_GrabIndicator.SetActive(false);
		switch (this.State) {
			case ParentState.Idle:
				m_IdleIndicator.SetActive(true);
				break;
			case ParentState.Grabbing:
				m_Animator.SetTrigger("Grab");
				m_GrabIndicator.SetActive(true);
				break;
		}
	}

	public void SetHeartLevel(int level) {
		this.HeartLevel = level;
		m_LevelText.text = this.HeartLevel.ToString();
	}

	public void IncreaseHeartLevel() {
		this.HeartLevel++;
		m_LevelText.text = this.HeartLevel.ToString();
	}

	public void DecreaseHeartLevel() {
		this.HeartLevel = Mathf.Max(0, this.HeartLevel - 1);
		m_LevelText.text = this.HeartLevel.ToString();
	}

}
