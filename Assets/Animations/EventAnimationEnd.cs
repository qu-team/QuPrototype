using UnityEngine;
using System.Collections;
using System;

public class EventAnimationEnd : MonoBehaviour {

	public Action EndAnimation;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AnimationEnded(){
		EndAnimation ();
	}
}
