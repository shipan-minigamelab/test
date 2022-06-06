using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class ProgressionBar : MonoBehaviour
    {
        public Image progressionImage;

        // public TextType textType;
        //
        // [ShowIf("textType", TextType.UNITY_TEXT)]
        // public Text progressionText;
        //
        // [ShowIf("textType", TextType.TEXT_MEST_PRO)]
        public TextMeshProUGUI progressionTextMeshProUGUI;

        // [ShowIf("textType", TextType.SHADOW_TEXT)]
        // public ShadowText progressionShadowText;

        private float maxValue;

        public void SetProgression(float _currentValue, float _maxValue = -1)
        {
            float percentage = (_currentValue * 100) / _maxValue;

            float fillAmount = percentage / 100;

            if (fillAmount >= 0 && fillAmount <= 100)
            {
                if (_currentValue > 0)
                {
                    DOTween.To(() => progressionImage.fillAmount, value => { progressionImage.fillAmount = value; },
                        fillAmount, 0.3f);
                }
                else
                {
                    progressionImage.fillAmount = 0;
                }
            }
//            Debug.Log("SetProgression percentage : " + percentage);

            
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetText(string _message)
        {
            if (progressionTextMeshProUGUI != null)
            {
                progressionTextMeshProUGUI.SetText(_message);
            }
            
            // if (textType == TextType.TEXT_MEST_PRO)
            // {
            //     if (progressionTextMeshProUGUI != null)
            //     {
            //         progressionTextMeshProUGUI.SetText(_message);
            //     }
            // }
            //
            // else if (textType == TextType.SHADOW_TEXT)
            // {
            //     if (progressionShadowText != null)
            //     {
            //         progressionShadowText.SetText(_message);
            //     }
            // }
            //
            // else
            // {
            //     if (progressionText != null)
            //     {
            //         progressionText.text = _message;
            //     }
            // }
        }


        public enum TextType
        {
            NONE,
            UNITY_TEXT,
            TEXT_MEST_PRO,
            SHADOW_TEXT
        }
    }
