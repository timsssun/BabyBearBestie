using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

	[SerializeField] Baby m_Baby;
	[SerializeField] Parent[] m_Parents;

	[Space(10)]

	[SerializeField] private int m_ParentStartHeartLevel = 3;

	private void Start() {
		foreach (Parent parent in m_Parents) {
			parent.SetHeartLevel(m_ParentStartHeartLevel);
		}
	}

	private void Update() {
		if (m_Baby.IsCrying) {
			foreach (Parent parent in m_Parents) {
				if (parent.IsGrabbing) {
					parent.IncreaseHeartLevel();
					m_Baby.Relax();
				}
			}
		}
	}

}
