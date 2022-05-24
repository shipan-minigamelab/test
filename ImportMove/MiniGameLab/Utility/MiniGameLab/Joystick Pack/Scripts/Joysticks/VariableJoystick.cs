using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VariableJoystick : Joystick
{
	public float MoveThreshold
	{
		get { return moveThreshold; }
		set { moveThreshold = Mathf.Abs(value); }
	}

	
	[SerializeField] private float moveThreshold = 1;
	[SerializeField] private JoystickType joystickType = JoystickType.Fixed;

	private Vector2 fixedPosition = Vector2.zero;

	private Transform _transform;

	private void Awake()
	{
		_transform = transform;
	}
	protected override void Start()
	{
		base.Start();
		fixedPosition = background.anchoredPosition;
		SetMode(joystickType);
	}

	private void OnDestroy()
	{
		_transform.DOKill();
	}

	public void SetMode(JoystickType joystickType)
	{
		this.joystickType = joystickType;
		if (joystickType == JoystickType.Fixed)
		{
			background.anchoredPosition = fixedPosition;
			background.gameObject.SetActive(true);
		}
		else
			background.gameObject.SetActive(false);
	}
	private bool isInterectable = true;

	public bool ActiveSelf
	{
		get => isInterectable;
		set
		{
			if (value == true)
			{
				_transform.localScale = Vector3.zero;
				_transform.DOScale(1, 0.3f).SetEase(Ease.OutBack, 2).SetDelay(0.25f);
			}
			else
			{
				// JoystickCanvasGroup.DOFade(0, 0.1f);
				ToggleJustVisibility(false);
			}

			isInterectable = value;
		}
	}

	public void ToggleJustVisibility(bool state)
	{
		background.gameObject.SetActive(state);
		_canvasGroup.alpha = state ? 1 : 0;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (joystickType != JoystickType.Fixed)
		{
			_transform.DOKill();
			_transform.localScale = Vector3.one;
			// background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
			background.anchoredPosition = ScreenPointToAnchoredPosition(Input.mousePosition);
			background.gameObject.SetActive(true);
		}

		base.OnPointerDown(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (joystickType != JoystickType.Fixed)
			background.gameObject.SetActive(false);

		base.OnPointerUp(eventData);
	}

	protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
	{
		if (joystickType == JoystickType.Dynamic && magnitude > moveThreshold)
		{
			Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
			background.anchoredPosition += difference;
		}

		base.HandleInput(magnitude, normalised, radius, cam);
	}
}

public enum JoystickType
{
	Fixed,
	Floating,
	Dynamic
}