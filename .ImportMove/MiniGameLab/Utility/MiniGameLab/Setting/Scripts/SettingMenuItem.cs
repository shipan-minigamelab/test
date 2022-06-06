using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CommonStuff
{
	public class SettingMenuItem : MonoBehaviour
	{
		public Button Button;
		public Slider Slider;
		public Image ButtonBGImage;
		public Image ButtonIcon;
		

		[Space]
		public Color EnableColor;
		public Color DisableColor;
		
		public float Value => Slider.value;

		public event Action<float> OnValueChanged;
		public event Action OnButtonClicked;

		private void Start()
		{
			Slider.onValueChanged.AddListener(delegate(float sliderValue)
			{
				OnValueChanged?.Invoke(sliderValue);
			});
			
			Button.onClick.AddListener(delegate
			{
				OnButtonClicked?.Invoke();
			});
		}

		private void UpdateUI(float value)
		{
			ButtonBGImage.color = value > 0 ? EnableColor : DisableColor;
			Button.transform.DOComplete();
			ButtonIcon.transform.DOKill();
			ButtonIcon.transform.DOScale(value > 0 ? 1 : 0.8f, 0.3f);

			if (value is < 0.1f or > 0.9f)
			{
				Button.transform.DOShakeRotation(0.1f, new Vector3(0, 0, 15f), 3);
			}

		}

		public void SetValue(float value)
		{
			Slider.value = value;
			UpdateUI(value);
		}
	}
}