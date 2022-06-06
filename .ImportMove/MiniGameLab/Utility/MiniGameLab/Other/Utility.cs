using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Fardin
{
    public class Utility : MonoBehaviour
    {
        // public static Utility instance;

        private Tween timeScaleTween;

        // private void Awake()
        // {
        //     if (instance == null)
        //         instance = this;
        //     else
        //     {
        //         Destroy(gameObject);
        //     }
        // }

        #region Slow Mo

        public void SlowMo(float _targetTimeScale, float _duration, Ease _ease = Ease.Linear, bool autoReset = true,
            Action OnComplete = null)
        {
            if (timeScaleTween != null && timeScaleTween.IsActive()) timeScaleTween.Kill();
            timeScaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, _targetTimeScale, _duration)
                .OnUpdate(() => { Time.fixedDeltaTime = 0.02f * Time.timeScale; })
                .SetEase(_ease)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (autoReset)
                        ResetTimeScale(true);
                    OnComplete?.Invoke();
                });
        }

        public void ResetTimeScale(bool smooth)
        {
            if (timeScaleTween != null && timeScaleTween.IsActive()) timeScaleTween.Kill();
            if (smooth)
            {
                timeScaleTween = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.5f)
                    .OnUpdate(() => { Time.fixedDeltaTime = 0.02f * Time.timeScale; })
                    .SetEase(Ease.Linear)
                    .SetUpdate(true);
            }
            else
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }

        #endregion

        #region Reveal Text

        public static void RevealText(string _textString, TextMeshProUGUI _text, float _speed, Action _OnComplete)
        {
            int counter = 0;
            int totalVisibleCharacter = _textString.Length;
            RectTransform parentRectTransform = _text.transform.parent.GetComponent<RectTransform>();
            _text.text = _textString;

            _text.maxVisibleCharacters = 0;
            DOTween.To(() => counter, x => counter = x, totalVisibleCharacter, _speed)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true)
                .OnUpdate(() =>
                {
                    _text.maxVisibleCharacters = counter;
                    if (parentRectTransform)
                        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
                })
                .OnComplete(() =>
                {
                    _text.maxVisibleCharacters = totalVisibleCharacter;
                    _OnComplete?.Invoke();
                });
        }

        public void RevealText(TextMeshPro _text, float _speed, Action _OnComplete)
        {
            int totalVisibleCharacter = _text.textInfo.characterCount;
            _text.maxVisibleCharacters = 0;
            int counter = 0;
            DOTween.To(() => counter, x => counter = x, totalVisibleCharacter, _speed)
                .SetEase(Ease.Linear)
                .SetSpeedBased(true)
                .OnUpdate(() => { _text.maxVisibleCharacters = counter; })
                .OnComplete(() =>
                {
                    _text.maxVisibleCharacters = totalVisibleCharacter;
                    _OnComplete?.Invoke();
                })
                .SetUpdate(true);
        }

        #endregion

       

        #region Bezier Curve

        public static void CreatePathBetweenPoints(int numberOfPoints, ref Vector3[] points, Vector3 p0, Vector3 p1,
            Vector3 p2)
        {
            for (int i = 0; i < numberOfPoints + 1; i++)
            {
                float t = i / (float) numberOfPoints;
                points[i] = CalculateQuadraticeCurve(t, p0, p1, p2);
            }
        }

        private static Vector3 CalculateQuadraticeCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            //p1.x *= tightFactor;
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        #endregion
        
        public static float Norm(float value, float min, float max) {
            return (value - min) / (max - min);
        }

        public static float Lerp(float norm, float min, float max) {
            return (max - min) * norm + min;
        }

        public static float Map(float value, float sourceMin, float sourceMax, float destMin, float destMax) {
            if (value <= sourceMin) return destMin;
            else if (value >= sourceMax) return destMax;
            else return Lerp(Norm(value, sourceMin, sourceMax), destMin, destMax);
        }
    }

    public static class ExtensionMethods
    {
        #region Update Text

        public static void UpdateText(this TextMeshProUGUI _text, int _startCount, int _endCount, float _duration,
            string _suffix = "", string _prefix = "",
            Action OnStart = null,
            Action OnComplete = null, Action OnUpdate = null, float _delay = 0)
        {
            // StartCoroutine(_UpdateText(_text, _startCount, _endCount, _duration, OnStart, OnComplete, OnUpdate, _delay));
            _text.text = _startCount.ToStringShort() + _suffix;
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(_delay);
            seq.AppendCallback(() => { OnStart?.Invoke(); });
            float val = _startCount;
            DOTween.To(() => val, x => val = x, _endCount, _duration)
                .OnUpdate(() =>
                {
                    _text.text = _prefix + val.ToStringShort() + _suffix;
                    OnUpdate?.Invoke();
                })
                .OnComplete(() =>
                {
                    _text.text = _prefix + _endCount.ToStringShort() + _suffix;
                    OnComplete?.Invoke();
                });
        }

        #endregion
        
        public static string ToStringShort(this int num)
        {
            if (num < 1000)
            {
                return num.ToString("#0", CultureInfo.CurrentCulture);
            }

            if (num < 10000)
            {
                num /= 10;
                return (num / 100f).ToString("#.00'K'", CultureInfo.CurrentCulture);
            }

            if (num < 1000000)
            {
                num /= 100;
                return (num / 10f).ToString("#.0'K'", CultureInfo.CurrentCulture);
            }

            if (num < 10000000)
            {
                num /= 10000;
                return (num / 100f).ToString("#.00'M'", CultureInfo.CurrentCulture);
            }

            num /= 100000;
            return (num / 10f).ToString("#,0.0'M'", CultureInfo.CurrentCulture);
        }

        public static string ToStringShort(this float num)
        {
            num = (int) num;
            if (num < 1000)
            {
                return num.ToString("#0", CultureInfo.CurrentCulture);
            }

            if (num < 10000)
            {
                num /= 10;
                return (num / 100f).ToString("#.00'K'", CultureInfo.CurrentCulture);
            }

            if (num < 1000000)
            {
                num /= 100;
                return (num / 10f).ToString("#.0'K'", CultureInfo.CurrentCulture);
            }

            if (num < 10000000)
            {
                num /= 10000;
                return (num / 100f).ToString("#.00'M'", CultureInfo.CurrentCulture);
            }

            num /= 100000;
            return (num / 10f).ToString("#,0.0'M'", CultureInfo.CurrentCulture);
        }
        public static void KillTween(this object item)
        {
            if (DOTween.IsTweening(item)) DOTween.Kill(item);
        }
    }
}