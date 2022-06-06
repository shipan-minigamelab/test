using System;
using System.Collections;
using System.Collections.Generic;
using CommonStuff;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
	public AnimationType currentAnimationType;
	public Animator _animator;

	public Action OnInstrumentHitAnimationEvent;
	public Action OnShootAnimationEvent;

	private Tween weightTween;
	private Action OnStartAnimationClipEvent = null;
	private Action OnEndAnimationClipEvent = null;


	private void OnEnable()
	{
		SubscribeStateBehaviours();
		if (currentAnimationType != AnimationType.NONE) PlayAnimationWithKey(currentAnimationType.ToString());
	}

	private void SubscribeStateBehaviours()
	{
		AnimationStateBehaviour[] stateBehaviours = _animator.GetBehaviours<AnimationStateBehaviour>();
		for (int i = 0; i < stateBehaviours.Length; ++i)
		{
			stateBehaviours[i].OnStateEnterEvent = OnStartClip;
			stateBehaviours[i].OnStateEndEvent = OnEndClip;
		}
	}

	private void OnStartClip(AnimationType animationType)
	{
		currentAnimationType = animationType;
		// if (animationType == currentAnimationType)
		{
			OnStartAnimationClipEvent?.Invoke();
			OnStartAnimationClipEvent = null;
		}
	}

	private void OnEndClip(AnimationType animationType)
	{
		if (animationType == currentAnimationType)
		{
			OnEndAnimationClipEvent?.Invoke();
			OnEndAnimationClipEvent = null;
		}
	}


	public void PlayAnimation(AnimationType animationType, Action OnComplete = null, bool force = false)
	{
		Debug.Log("PLAY ANIMATION: " + animationType.ToString());
		if (!force) if (currentAnimationType == animationType)
				return;

		if (animationType == AnimationType.NONE) return;

		currentAnimationType = animationType;
		ResetTrigger(animationType);
		PlayAnimationWithKey(animationType.ToString());

		if (OnComplete != null)
		{
			OnEndAnimationClipEvent = OnComplete;
		}
	}

	public void SetLayerWeight(int index, float targetWeight, bool animate = true)
	{
		if (animate)
		{
			CommonStuffs.KillTween(weightTween);
			float value = _animator.GetLayerWeight(index);
			weightTween = DOTween.To(() => value, x => value = x, targetWeight, 1)
				.OnUpdate(() => { _animator.SetLayerWeight(index, value); });
		}

		else _animator.SetLayerWeight(index, targetWeight);
	}


	private void PlayAnimationWithKey(string key)
	{
		_animator.ResetTrigger(key);
		_animator.SetTrigger(key);
	}

	public void ResetTrigger(AnimationType animationType)
	{
		_animator.ResetTrigger(animationType.ToString());
	}

	public void SetAnimationClipSpeed(int keyID, float speed)
	{
		_animator.SetFloat(keyID, speed);
	}

	#region ANIMATION EVENT

	public void OnInstrumentHit()
	{
		OnInstrumentHitAnimationEvent?.Invoke();
	}

	public void OnGroundStep()
	{
		// AudioManager.Instance.Play(AudioIDs.FootStep);
	}

	public void OnShoot()
	{
		OnShootAnimationEvent?.Invoke();
	}

	#endregion
}