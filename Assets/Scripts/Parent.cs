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

	[SerializeField] private ParticleSystem m_HeartParticle;

	private ParentState State { get; set; }
	public int HeartLevel { get; private set; }
	public Color HeartColor => m_HeartColor;
	public ParticleSystem HeartParticles => m_HeartParticle;

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
		switch (this.State) {
			case ParentState.Idle:
				break;
			case ParentState.Grabbing:
				m_Animator.SetTrigger("Grab");
				break;
		}
	}

	public void SetHeartLevel(int level) {
		this.HeartLevel = level;
	}

	public void IncreaseHeartLevel() {
		this.HeartLevel++;
	}

	public void DecreaseHeartLevel() {
		this.HeartLevel = Mathf.Max(0, this.HeartLevel - 1);
	}

}
