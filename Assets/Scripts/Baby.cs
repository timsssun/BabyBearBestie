using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BabyState { Idle, Crying, Faking }

public class Baby : MonoBehaviour {

	[SerializeField] private Vector2 m_CryIntervalRange;

	[SerializeField] private Vector2 m_CryDurationRange;

	[Space(10)]

	[SerializeField] private GameObject m_CryingIndicator;
	[SerializeField] private GameObject m_IdleIndicator;
	[SerializeField] private GameObject m_FakingIndicator;

	private float CryStartTime { get; set; }
	private float LastCryEndTime { get; set; }
	private float CurrentIntervalUntilCrying { get; set; }
	private float CurrentCryingDuration { get; set; }

	private BabyState State { get; set; }

	private void Start() {
		Relax();
	}

	private void Update() {
		switch (this.State) {
			case BabyState.Idle:
				m_IdleIndicator.SetActive(true);
				if (Time.time - this.LastCryEndTime > this.CurrentIntervalUntilCrying) {
					Cry();
				}
				break;
			case BabyState.Crying:
				if (Time.time - this.CryStartTime > this.CurrentCryingDuration) {
					Relax();
				}
				break;
			case BabyState.Faking:
				m_FakingIndicator.SetActive(true);
				break;
		}
	}

	private void Cry() {
		this.CryStartTime = Time.time;
		this.State = BabyState.Crying;
		this.CurrentCryingDuration = Random.Range(m_CryDurationRange.x, m_CryDurationRange.y);
		SetVisuals();
	}

	private void Relax() {
		this.LastCryEndTime = Time.time;
		this.State = BabyState.Idle;
		this.CurrentIntervalUntilCrying = Random.Range(m_CryIntervalRange.x, m_CryIntervalRange.y);
		SetVisuals();
	}

	private void SetVisuals() {
		m_CryingIndicator.SetActive(false);
		m_IdleIndicator.SetActive(false);
		m_FakingIndicator.SetActive(false);
		switch (this.State) {
			case BabyState.Idle:
				m_IdleIndicator.SetActive(true);
				break;
			case BabyState.Crying:
				m_CryingIndicator.SetActive(true);
				break;
			case BabyState.Faking:
				m_FakingIndicator.SetActive(true);
				break;
		}
	}

}
