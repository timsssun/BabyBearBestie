using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BabyState { Idle, Crying, BuildUp, Faking }

public class Baby : MonoBehaviour {

	[SerializeField] private Vector2 m_CryIntervalRange;
	[SerializeField] private Vector2 m_CryDurationRange;
	[SerializeField] private Vector2 m_BuildUpRange;

	[Space(10)]

	[SerializeField] private GameObject m_CryingIndicator;
	[SerializeField] private GameObject m_IdleIndicator;
	[SerializeField] private GameObject m_FakingIndicator;
	[SerializeField] private GameObject m_BuildUpIndicator;

	public bool IsCrying {
		get { return this.State == BabyState.Crying; }
	}

	private float CurrentStateDuration { get; set; }
	private float CurrentStateStartTime { get; set; }

	private BabyState State { get; set; }

	private void Start() {
		Relax();
	}

	private void Update() {
		switch (this.State) {
			case BabyState.Idle:
				m_IdleIndicator.SetActive(true);
				if (Time.time - this.CurrentStateStartTime > this.CurrentStateDuration) {
					BuildUp();
				}
				break;
			case BabyState.Crying:
				if (Time.time - this.CurrentStateStartTime > this.CurrentStateDuration) {
					Relax();
				}
				break;
			case BabyState.BuildUp:
				if (Time.time - this.CurrentStateStartTime > this.CurrentStateDuration) {
					Cry();
				}
				break;
			case BabyState.Faking:
				m_FakingIndicator.SetActive(true);
				break;
		}
	}

	private void BuildUp() {
		SetState(BabyState.BuildUp, m_BuildUpRange);
	}

	private void Cry() {
		SetState(BabyState.Crying, m_CryDurationRange);
	}

	public void Relax() {
		SetState(BabyState.Idle, m_CryIntervalRange);
	}

	public void SetState(BabyState state, Vector2 durationRange) {
		this.CurrentStateDuration = Random.Range(durationRange.x, durationRange.y);
		this.State = state;
		this.CurrentStateStartTime = Time.time;
		SetVisuals();
	}

	private void SetVisuals() {
		m_CryingIndicator.SetActive(false);
		m_IdleIndicator.SetActive(false);
		m_FakingIndicator.SetActive(false);
		m_BuildUpIndicator.SetActive(false);
		switch (this.State) {
			case BabyState.Idle:
				m_IdleIndicator.SetActive(true);
				break;
			case BabyState.Crying:
				m_CryingIndicator.SetActive(true);
				break;
			case BabyState.BuildUp:
				m_BuildUpIndicator.SetActive(true);
				break;
			case BabyState.Faking:
				m_FakingIndicator.SetActive(true);
				break;
		}
	}

}
