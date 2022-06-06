using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class CommonStuffs : MonoBehaviour
{
	public static CommonStuffs Instance;

	[SerializeField] private UIFocus UIFocus;

	[Header("UI")]
	[SerializeField] private GameObject touchSafeArea;

	[SerializeField] private GameObject handUI;
	[SerializeField] private Image fadeImg = null;
	[SerializeField] private Image vignetteImg = null;
	[SerializeField] private GameObject _fpsPanel;
	[SerializeField] private TextMeshProUGUI _fpsText;
	[SerializeField] private bool _showFPS;
	private Tween _fadeTween;

	private Sequence _fpsSequence;

	public static Action OnFlashEvent;
	public static Action OnResetEvent;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
		transform.parent = null;
		DontDestroyOnLoad(gameObject);
		touchSafeArea.SetActive(false);
	}

	private void Start()
	{
		ToggleFPS(_showFPS);
	}

	#region UI

	public void FadeIn(Color fadeColor,float duration = 0.3f, Action OnComplete = null)
	{
		fadeColor.a = 0;
		fadeImg.color = fadeColor;
		KillTween(_fadeTween);
		_fadeTween = fadeImg.DOFade(1, duration).OnComplete(() =>
		{
			OnComplete?.Invoke();
		});
	}

	public void FadeOut(Color fadeColor, float duration = 0.3f, Action OnComplete = null)
	{
		fadeColor.a = 1;
		fadeImg.color = fadeColor;
		KillTween(_fadeTween);
		_fadeTween = fadeImg.DOFade(0, duration).OnComplete(() => { OnComplete?.Invoke(); });
	}

	public void Flash(Color color , float delay = 0, float duration = 0.2f, int repeat = 0, Action OnComplete = null)
	{
		fadeImg.color = color;
		KillTween(_fadeTween);
		_fadeTween = fadeImg.DOFade(0, duration).SetLoops(repeat, LoopType.Restart).OnComplete(delegate { OnComplete?.Invoke(); });
		OnFlashEvent?.Invoke();
	}

	public void FadeVignette(float alpha, float duration)
	{
		vignetteImg.DOKill();
		vignetteImg.gameObject.SetActive(true);
		if (alpha == 0)
		{
			vignetteImg.DOFade(alpha, duration).OnComplete(delegate { vignetteImg.gameObject.SetActive(false); });
		}
		else
		{
			vignetteImg.DOFade(alpha, duration);
		}
	}

	public void ShowUIFocus(RectTransform rectTransform, UIFocusType type = UIFocusType.Rectangular, UIFocusSpace uiFocusSpace = UIFocusSpace.ScreenSpace)
	{
		UIFocus.ShowUIFocus(rectTransform, type, uiFocusSpace);
	}

	public void HideUIFocus()
	{
		UIFocus.HideUIFocus();
	}

	#endregion

	#region DEBUG

	public static void KillTween(Tween _tween)
	{
		if (_tween != null && _tween.IsActive()) _tween.Kill();
	}


	public void ToggleFPS(bool state)
	{
		_fpsPanel.gameObject.SetActive(state);
		if (_fpsPanel.gameObject.activeInHierarchy)
		{
			if (_fpsSequence == null)
			{
				_fpsSequence = DOTween.Sequence().SetAutoKill(false).SetLoops(-1);
				_fpsSequence.AppendInterval(0.5f);
				_fpsSequence.AppendCallback(() => { _fpsText.text = "FPS : " + Mathf.FloorToInt(1f / Time.deltaTime); });
			}
		}
		else
		{
			KillTween(_fpsSequence);
			_fpsSequence = null;
		}
	}
// #if UNITY_EDITOR
// 	private void Update()
// 	{
// 		if (Input.GetKeyDown(KeyCode.S))
// 		{
// 			touchSafeArea.SetActive(!touchSafeArea.activeInHierarchy);
// 		}
// 		else if (Input.GetKeyDown(KeyCode.H))
// 		{
// 			handUI.SetActive(!handUI.activeInHierarchy);
// 		}
// 		else if (Input.GetKeyDown(KeyCode.F))
// 		{
// 			ToggleFPS(!_fpsPanel.gameObject.activeInHierarchy);
// 		}
// 	}
// #endif

	#endregion

	public static void SetDirtyPrefab(UnityEngine.Object obj)
	{
#if UNITY_EDITOR

		EditorUtility.SetDirty(obj);
		PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
#endif
	}

	public static void SetSceneDirty()
	{
#if UNITY_EDITOR
		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
#endif
	}

}