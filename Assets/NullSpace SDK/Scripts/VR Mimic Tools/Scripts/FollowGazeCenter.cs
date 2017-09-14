﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGazeCenter : MonoBehaviour
{
	public Camera cameraToFollow;
	public Canvas follower;
	private CanvasGroup followerGrp;

	public enum UpdateState { Idle, WaitingToMove, Moving, Fading, Inactive }
	public enum Orientation { WorldUp, HeadsetUp }
	[Header("Current State")]
	public UpdateState CurrentState = UpdateState.Idle;
	public Orientation FollowOrientation = Orientation.WorldUp;

	[Header("Gaze Following Variables")]
	[Range(.5f, 3)]
	public float delayBetweenFollow = .5f;
	[Range(.5f, 10)]
	public float gazeUIDistanceFromCamera = 3;

	public AnimationCurve tweenCurve;

	[Header("Tween Timing Controls")]
	public float tweenCounter;
	[Range(.001f, 10)]
	public float tweenDuration = 1;

	[Header("Fade Delay Controls")]
	public bool WillFade = true;
	public float delayCounter = 0;
	[Range(.001f, 10)]
	public float delayTillFadeOut = 5;
	//private bool OnlyFadeDuringIdle = true;

	[Header("Alpha Fade Timing Controls")]
	public float fadeCounter = 0;
	[Range(.001f, 10)]
	public float fadeDuration = 1.5f;

	[Header("Exposed Math Variables")]
	[SerializeField]
	private float movementDelayCounter;
	[SerializeField]
	private Vector3 oldPosition;
	[SerializeField]
	private Vector3 potentialTargetPosition;
	[SerializeField]
	private Vector3 targetPosition;
	[SerializeField]
	private float distanceBetweenCurrentAndTarget = 0;
	[SerializeField]
	private float MinDistanceBeforeMove = 3;
	[SerializeField]
	private bool NeedsUpdate = true;

	void Start()
	{
		if (follower)
		{
			followerGrp = follower.GetComponent<CanvasGroup>();
		}
		ResetGazeUI();
	}

	public void ResetGazeUI()
	{
		oldPosition = Vector3.zero;
		potentialTargetPosition = Vector3.zero;
		targetPosition = Vector3.zero;
		NeedsUpdate = true;
		CurrentState = UpdateState.Idle;
		delayCounter = delayTillFadeOut;
		fadeCounter = 0;
		SetAlpha(0);
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Home))
		{
			ResetGazeUI();
		}

		if (CurrentState == UpdateState.Inactive)
			return;

		GeneralUpdate();

		if (CurrentState == UpdateState.Idle)
		{
			UpdateIdle();
		}
		else if (CurrentState == UpdateState.WaitingToMove)
		{
			UpdateWaiting();
		}
		else if (CurrentState == UpdateState.Moving)
		{
			UpdateMoving();
		}
		else if (CurrentState == UpdateState.Fading)
		{
			UpdateFading();
		}
	}

	private void GeneralUpdate()
	{
		Vector3 targ = (follower.transform.position - cameraToFollow.transform.position) * 1.1f;
		Vector3 up = FollowOrientation == Orientation.WorldUp ? Vector3.up : cameraToFollow.transform.up;
		Vector3 target = cameraToFollow.transform.position + cameraToFollow.transform.forward * gazeUIDistanceFromCamera * 2;
		follower.transform.LookAt(cameraToFollow.transform.position + targ, up);

		potentialTargetPosition = GetCameraTargetPosition();
		distanceBetweenCurrentAndTarget = Vector3.Distance(follower.transform.position, potentialTargetPosition);
	}

	void UpdateIdle()
	{
		if (distanceBetweenCurrentAndTarget > MinDistanceBeforeMove)
		{
			if (NeedsUpdate)
			{
				NeedsUpdate = false;
				StartCounter();
			}
			CurrentState = UpdateState.WaitingToMove;
		}
		else
		{
			CheckStartFading();
		}
	}

	void UpdateWaiting()
	{
		if (movementDelayCounter > 0)
		{
			movementDelayCounter -= Time.deltaTime;
		}
		else if (movementDelayCounter <= 0)
		{
			targetPosition = GetCameraTargetPosition();
			oldPosition = follower.transform.position;
			CurrentState = UpdateState.Moving;
			tweenCounter = 0;
		}
	}

	void UpdateMoving()
	{
		TweenToTargetPosition();
		CheckLeaveMovementPhase();
	}
	void TweenToTargetPosition()
	{
		tweenCounter += Time.deltaTime;
		float val = tweenCurve.Evaluate(tweenCounter / tweenDuration);
		Vector3 tweenedPosition = Vector3.Lerp(oldPosition, targetPosition, val);
		follower.transform.position = tweenedPosition;
		NeedsUpdate = true;
	}
	void CheckLeaveMovementPhase()
	{
		if (tweenCounter >= tweenDuration)
		{
			tweenCounter = 0;
			CurrentState = UpdateState.Idle;
		}
	}

	void CheckStartFading()
	{
		if (delayCounter > 0)
		{
			delayCounter -= Time.deltaTime;
		}
		if (delayCounter <= 0)
		{
			CurrentState = UpdateState.Fading;
		}
	}
	void UpdateFading()
	{
		if (WillFade && followerGrp != null)
		{
			if (delayCounter <= 0)
			{
				fadeCounter += Time.deltaTime;
				SetAlpha(Mathf.Clamp(fadeCounter / fadeDuration, 0, 1.0f));
			}
			if (fadeCounter > fadeDuration)
			{
				CurrentState = UpdateState.Inactive;
			}
		}
	}
	void SetAlpha(float alphaTweenPercent)
	{
		if (followerGrp)
		{
			float curveValue = tweenCurve.Evaluate(alphaTweenPercent);
			followerGrp.alpha = 1 - curveValue;
		}
	}

	private void StartCounter()
	{
		movementDelayCounter = delayBetweenFollow;
	}

	private Vector3 GetCameraTargetPosition()
	{
		return cameraToFollow.transform.position + cameraToFollow.transform.forward * gazeUIDistanceFromCamera;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		if (follower != null)
		{
			Gizmos.DrawSphere(follower.transform.position, .05f);
			var targ = follower.transform.position - targetPosition;
			float val = tweenCurve.Evaluate(tweenCounter / tweenDuration);
			Vector3 tweenedPosition = Vector3.Lerp(oldPosition, targetPosition, val);
			Gizmos.DrawLine(follower.transform.position, follower.transform.position + targ.normalized * val);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(follower.transform.position + targ.normalized * val, targetPosition);
			Gizmos.DrawSphere(targetPosition, .05f);
		}
	}
}