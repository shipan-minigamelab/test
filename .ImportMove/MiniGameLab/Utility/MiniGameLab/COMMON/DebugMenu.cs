using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{
	[SerializeField] private RectTransform debugPanel;

	private void Start()
	{
		debugPanel.anchoredPosition = new Vector2(0, debugPanel.sizeDelta.y);
	}

	public void ShowDebugPanel()
	{
		debugPanel.DOAnchorPosY(0, 0.2f);
	}

	public void CloseDebugPanel()
	{
		debugPanel.DOAnchorPosY(debugPanel.sizeDelta.y, 0.1f);
	}

	public static void Log(string log)
	{
#if UNITY_EDITOR
		Debug.Log(log);
#endif
	}
}