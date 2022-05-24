using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class UIFocusDemo : MonoBehaviour
{
	[SerializeField] private RectTransform[] rects;
	[SerializeField] private int currentIndex = 0;
	[SerializeField] UIFocusType type;
	[SerializeField] private RectTransform testREct;
	[SerializeField] private Vector2 testRectPos;
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)) CycleThroughRects();
	}

	[Button]
	private void CycleThroughRects()
	{
		currentIndex++;
		if (currentIndex >= rects.Length) currentIndex = 0;
		if (currentIndex == rects.Length - 1) type = UIFocusType.Radial;
		else type = UIFocusType.Rectangular;
		CommonStuffs.Instance.ShowUIFocus(rects[currentIndex], type);
	}
}