using UnityEngine;
using System.Collections;

public class ParachuteRenderer : MonoBehaviour {

	private Animator paraglidingAnimator;

	void Start () {
		paraglidingAnimator = GetComponent<Animator> ();

	}

	void Update () {
		RenderOfRightBrake ();
		RenderOfLeftBrake ();
	}

	private void RenderOfLeftBrake(){
		paraglidingAnimator.Play("LeftBrake", 1, Input.GetAxis("brakeLeft"));
	}

	private void RenderOfRightBrake(){
		paraglidingAnimator.Play("RightBrake", 2, Input.GetAxis("brakeRight"));
	}
}
