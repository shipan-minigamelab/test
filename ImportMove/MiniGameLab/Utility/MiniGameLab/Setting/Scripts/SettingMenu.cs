using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CommonStuff
{
	public class SettingMenu : MonoBehaviour
	{
		public static SettingMenu Instance;

		[SerializeField] private Button closeButton;
		[SerializeField] private Button settingButton;
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private Transform holderTransform;
		[SerializeField] private List<SettingMenuItemInfo> SettingItemInfos;


		public event Action<SettingItemType> OnItemButtonClicked;
		public event Action<SettingItemType, float> OnItemValueChange;

		public event Action<bool> OnMenuShowEvent;

		private void Awake()
		{
			Instance = this;
		}


		private void Start()
		{
			foreach (var itemInfo in SettingItemInfos)
			{
				itemInfo.MenuItem.OnButtonClicked += delegate { OnItemButtonClicked?.Invoke(itemInfo.ItemType); };

				itemInfo.MenuItem.OnValueChanged += delegate(float value) { OnItemValueChange?.Invoke(itemInfo.ItemType, value); };
			}

			closeButton?.onClick.AddListener(delegate { OnMenuShowEvent?.Invoke(false); });

			settingButton?.onClick.AddListener(delegate { OnMenuShowEvent?.Invoke(true); });
		}


		public void Show()
		{
			canvasGroup.alpha = 0;
			canvasGroup.DOFade(1, 0.3f);
			canvasGroup.blocksRaycasts = true;
			holderTransform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
		}

		public void Hide()
		{
			canvasGroup.DOFade(0, 0.2f);
			canvasGroup.blocksRaycasts = false;
		}


		public void SetItemValue(SettingItemType itemType, float value)
		{
			var itemInfo = GetItemInfo(itemType);
			itemInfo.MenuItem.SetValue(value);
		}

		public SettingMenuItemInfo GetItemInfo(SettingItemType itemType)
		{
			return SettingItemInfos.Find(item => item.ItemType == itemType);
		}

		public void ToggleSettingsButton(bool state)
		{
			settingButton.gameObject.SetActive(state);
		}
	}

	[System.Serializable]
	public struct SettingMenuItemInfo
	{
		public SettingItemType ItemType;
		public SettingMenuItem MenuItem;
	}

	public enum SettingItemType
	{
		NONE,
		MUSIC,
		SFX,
		HAPTIC
	}
}