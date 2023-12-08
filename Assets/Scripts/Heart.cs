using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour {

	public Animator Animator { get; private set; }
	public SpriteRenderer SpriteRenderer { get; private set; }

	private void Start() {
		this.Animator = GetComponent<Animator>();
		this.SpriteRenderer = GetComponent<SpriteRenderer>();
	}
}
