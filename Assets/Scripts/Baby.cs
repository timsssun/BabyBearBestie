using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BabyState { Idle, Crying, BuildUp, Angry, Happy, Faking }

public class Baby : MonoBehaviour {

	[SerializeField] private Vector2 m_CryIntervalRange;
	[SerializeField] private Vector2 m_CryDurationRange;
	[SerializeField] private Vector2 m_BuildUpRange;
	[SerializeField] private float m_AngryDuration;
	[SerializeField] private float m_HappyDuration;

	[Space(10)]

	[SerializeField] private Animator m_Animator;

	[Space(10)]

	[SerializeField] private SoundArray[] m_Sounds;
	[SerializeField] private AudioSource m_Source;

	public bool IsCrying {
		get { return this.State == BabyState.Crying; }
	}

	private float CurrentStateDuration { get; set; }
	private float CurrentStateStartTime { get; set; }

	public BabyState State { get; private set; }

	public void InitializeBaby() {
		SetState(BabyState.Idle, m_CryIntervalRange, true);
	}

	public void UpdateBaby() {
		bool isStateOver = Time.time - this.CurrentStateStartTime > this.CurrentStateDuration;
		if (isStateOver) {
			switch (this.State) {
				case BabyState.Idle:
					BuildUp();
					break;
				case BabyState.Crying:
					Angry();
					break;
				case BabyState.BuildUp:
					Cry();
					break;
				case BabyState.Angry:
					Relax();
					break;
				case BabyState.Happy:
					Relax();
					break;
				case BabyState.Faking:
					Relax();
					break;
			}
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

	public void Happy() {
		SetState(BabyState.Happy, new Vector2(m_HappyDuration, m_HappyDuration));
	}

	public void Angry() {
		SetState(BabyState.Angry, new Vector2(m_AngryDuration, m_AngryDuration));
	}

	public void SetState(BabyState state, Vector2 durationRange, bool force = false) {
		if (this.State != state || force) {
			this.CurrentStateDuration = Random.Range(durationRange.x, durationRange.y);
			this.State = state;
			this.CurrentStateStartTime = Time.time;
			SetVisuals();
		}
	}

	private void SetVisuals() {
		m_Animator.SetInteger("State", (int)this.State);
		AudioClip clip = m_Sounds[(int)this.State].GetRandomAudioClip();
		if (clip != null) {
			m_Source.PlayOneShot(clip);
		}
		AudioClip layeredClip = m_Sounds[(int)this.State].GetLayeredClip();
		if (layeredClip != null) {
			m_Source.PlayOneShot(layeredClip);
		}
	}

}
