using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private bool autoAddEventToButton = true;
    [SerializeField] private bool customTransform = false;

    [ShowIf("customTransform")] [SerializeField]
    private Transform _transform;

    [SerializeField] private float scaleMultiplier = 0.1f;
    [SerializeField] private float duration = 0.2f;
    [HorizontalLine()] [SerializeField] private bool punchRotation = false;

    [ShowIf("punchRotation")] [SerializeField]
    private float rotAmount = 15f;

    [ShowIf("punchRotation")] [SerializeField]
    private float rotDuration = 0.2f;

    private Tween punchTween;
    private Tween punchRotTween;

    private void Start()
    {
        if (autoAddEventToButton)
        {
            Button btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => { ScaleAnimation(); });
            }
        }
    }

    public void ScaleAnimation()
    {
        if (!customTransform) _transform = this.transform;
        if (punchTween != null && punchTween.IsActive()) punchTween.Kill(true);
        punchTween = _transform.DOPunchScale(_transform.localScale * scaleMultiplier, duration)
            .SetEase(Ease.OutBack);

        if (punchRotation)
        {
            if (punchRotTween != null && punchRotTween.IsActive()) punchRotTween.Kill(true);
            punchRotTween = _transform.DOPunchRotation(new Vector3(0, 0, rotAmount), rotDuration, 10, 0)
                .SetEase(Ease.OutQuad);
        }
    }

    private void OnDestroy()
    {
        if(punchTween != null && punchTween.IsActive()) punchTween.Kill();
        if(punchRotTween != null && punchRotTween.IsActive()) punchRotTween.Kill();
    }
}