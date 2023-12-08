using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundArray : MonoBehaviour {

	[SerializeField] private AudioClip[] m_Clips;

	public AudioClip GetRandomAudioClip() {
		if (m_Clips.Length > 0) {
			return m_Clips[Random.Range(0, m_Clips.Length)];
		}
		return null;
	}
}
