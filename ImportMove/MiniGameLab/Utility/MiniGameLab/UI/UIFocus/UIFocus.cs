using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class UIFocus : MonoBehaviour
{
	[SerializeField] private RectTransform _canvasRectTransform;
	[SerializeField] private UIFocusType _uiFocusType;
	[SerializeField] private Image _image;
	[SerializeField] private RectTransform _fakeBoy;
	[SerializeField] private Material _material;
	[SerializeField] private Vector2 _center;
	[HideIf(nameof(IsRadial))] [SerializeField] private Vector2 _rectArea;
	[ShowIf(nameof(IsRadial))] [SerializeField] private float _radius;
	[Range(0, 1)] [SerializeField] private float _alpha;
	[SerializeField] private float bobDelta = 20;
	[SerializeField] private float _hardness;

	private Tween _centerTween, _radiusTween, _rectAreaTween;

	public void ShowUIFocus(RectTransform rectTransform, UIFocusType type, UIFocusSpace focusSpace)
	{
		KillAllTween();
		_uiFocusType = type;
		Vector2 center = Vector2.zero;
		if (focusSpace == UIFocusSpace.ScreenSpace)
		{
			_fakeBoy.position = rectTransform.position;
			center = _fakeBoy.anchoredPosition;
		}
		else if (focusSpace == UIFocusSpace.CameraSpace)
		{
			_fakeBoy.position = Camera.main.WorldToScreenPoint(rectTransform.position);
			center = _fakeBoy.anchoredPosition;
		}

		_centerTween = DOTween.To(() => _center, x => _center = x, center, 0.2f).OnUpdate(SetShaderValues);
		if (type == UIFocusType.Radial)
		{
			float radius = rectTransform.sizeDelta.x / 2;
			float bobRadius = radius + bobDelta;
			_hardness = 3;
			_radiusTween = DOTween.To(() => _radius, x => _radius = x, radius, 0.2f).OnUpdate(SetShaderValues)
				.OnComplete(() =>
				{
					_radiusTween = DOTween.To(() => _radius, x => _radius = x, bobRadius, 0.2f)
						.SetLoops(-1, LoopType.Yoyo)
						.OnUpdate(SetShaderValues);
				});
		}
		else if (type == UIFocusType.Rectangular)
		{
			_hardness = 60;
			Vector2 rectArea = rectTransform.sizeDelta;
			Vector2 bobRect = rectArea + new Vector2(bobDelta, bobDelta);
			_rectAreaTween = DOTween.To(() => _rectArea, x => _rectArea = x, rectArea, 0.2f).OnUpdate(SetShaderValues)
				.OnComplete(() =>
				{
					_rectAreaTween = DOTween.To(() => _rectArea, x => _rectArea = x, bobRect, 0.2f)
						.SetLoops(-1, LoopType.Yoyo)
						.OnUpdate(SetShaderValues);
				});
		}

		_image.enabled = true;
	}

	public void HideUIFocus()
	{
		_image.enabled = false;
	}

	private void OnValidate()
	{
		SetShaderValues();
	}

	private void SetShaderValues()
	{
		_material.SetVector("_Center", _center);
		_material.SetVector("_RectArea", _rectArea);
		_material.SetFloat("_Radius", _radius);
		_material.SetFloat("_Hardness", _hardness);
		_material.SetFloat("_Alpha", _alpha);
		_material.SetInt("_IsRadial", (int) _uiFocusType);
	}

	private void KillAllTween()
	{
		KillTween(_centerTween);
		KillTween(_radiusTween);
		KillTween(_rectAreaTween);
	}

	private void KillTween(Tween tween)
	{
		if (tween != null && tween.IsActive()) tween.Kill();
	}

	public Image GetImage() => _image;
	private bool IsRadial() => _uiFocusType == UIFocusType.Radial;
	[Button] private void Debug() => _image.enabled = !_image.enabled;
}

public enum UIFocusType
{
	Radial,
	Rectangular
}

public enum UIFocusSpace
{
	ScreenSpace,
	CameraSpace
}