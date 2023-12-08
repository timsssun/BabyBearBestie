using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : MonoBehaviour {

	[SerializeField] private Heart[] m_Hearts;

	public void SetColors(int firstAmount, Color first, Color second) {
		for (int i = 0; i < m_Hearts.Length; i++) {
			Color colorToUse = first;
			if (i >= firstAmount) {
				colorToUse = second;
			}
			SetColor(i, colorToUse);
		}
	}

	private void SetColor(int heartIndex, Color color) {
		if (heartIndex >= 0 && heartIndex < m_Hearts.Length) {
			if (m_Hearts[heartIndex].SpriteRenderer.color != color) {
				m_Hearts[heartIndex].Animator.SetTrigger("Beat");
			}
			m_Hearts[heartIndex].SpriteRenderer.color = color;
		}
	}

}
