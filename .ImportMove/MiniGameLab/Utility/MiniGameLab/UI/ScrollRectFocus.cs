using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollRectFocus
{
    private static float Norm(float value, float min, float max) {
        return (value - min) / (max - min);
    }

    private static float Lerp(float norm, float min, float max) {
        return (max - min) * norm + min;
    }

    private static float Map(float value, float sourceMin, float sourceMax, float destMin, float destMax) {
        if (value <= sourceMin) return destMin;
        else if (value >= sourceMax) return destMax;
        else return Lerp(Norm(value, sourceMin, sourceMax), destMin, destMax);
    }

    public static float GetFocusNormalizedValue(this ScrollRect target, int index)
    {
        float normalizedValue = 0;
        
        if (index < target.content.childCount)
        {
            int maxLength = 0;

            for (int i = 0; i < target.content.childCount; i++)
            {
                if (target.content.GetChild(i).gameObject.activeInHierarchy)
                {
                    maxLength++;
                }
            }
            
            normalizedValue = Map(index, 0, maxLength, 0f, 1f);
        }
        else
        {
            Debug.LogError("Index Out Of Range Exception : " + target.gameObject.name);
        }

        return normalizedValue;
    }


    // public static void FocusOnItem(this ScrollRect rect,  int index,  Action OnComplete = null)
    // {
    //     float normalizedValue = rect.GetFocusNormalizedValue(index);
    //     rect.horizontalNormalizedPosition = 0.0f;
    //     rect.horizontalNormalizedPosition = Mathf.Clamp01(normalizedValue - 0.3f);
    //     
    //     rect.DOHorizontalNormalizedPos(normalizedValue, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
    //     {
    //         OnComplete?.Invoke();
    //     });
    // }
    
    public static Tweener DOHorizontalFocusOnItem(this ScrollRect target, int index, float duration, bool snapping = false)
    {

        float normalizedValue = target.GetFocusNormalizedValue(index);
        
        return DOTween.To(() => target.horizontalNormalizedPosition, x => target.horizontalNormalizedPosition = x, normalizedValue, duration)
            .SetOptions(snapping).SetTarget(target);
    }
    
    public static Tweener DOVerticalFocusOnItem(this ScrollRect target,  int index, float duration, bool snapping = false)
    {
        float normalizedValue = target.GetFocusNormalizedValue(index);
        return DOTween.To(() => target.verticalNormalizedPosition, x => target.verticalNormalizedPosition = x, normalizedValue, duration)
            .SetOptions(snapping).SetTarget(target);
    }
    
    
}