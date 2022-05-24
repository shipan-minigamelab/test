using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public  class GameMenu : MonoBehaviour
{
    public bool scaleMenuEnabled = true;
    public bool sideMenuEnabled = true;
    public bool dynamicMenuEnabled = false;
    
    [Header("Fade Info")]
    public float fadeDuration = 0.3f;
   
    [Header("Scale Info")]
    public float scaleDuration = 0.2f;
    public float startScale = 0;
    public float endScale = 1;
    public Ease scaleEase;
    [Header("  ")]
    [ShowIf(nameof(sideMenuEnabled))]  
    public float moveDuration = 0.2f;

    
   [HideInInspector] public bool isDynamicMenuShow = false;

    [Header("Reference")]
    public CanvasGroup canvasGroup;
    public RectTransform panelHolder;
    
    [HideInInspector]   public Transform panelPointer;
    [HideInInspector]    public Camera mainCamera;
   
    public event Action OnShowEvent;
    public event Action OnHideEvent;


    [Button]
    public   void Show()
    {
        if (scaleMenuEnabled)
        {
            ScaleMenuShow();
        }

        if (dynamicMenuEnabled)
        {
            DynamicMenuShow();
        }
        else if (sideMenuEnabled)
        {
            SideMenuShow();
        }
        
    }

    [Button]
    public  void Hide()
    {
        canvasGroup.DOKill();
        panelHolder.DOKill();
        
        if (scaleMenuEnabled)
        {
            ScaleMenuHide();
        }
        
        if (dynamicMenuEnabled)
        {
            DynamicMenuHide();
        }
        else if (sideMenuEnabled)
        {
            SideMenuHide();
        }
    }

    private void OnDestroy()
    {
        canvasGroup.DOKill();
        panelHolder.DOKill();
    }

    public  void ScaleMenuShow()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();

        canvasGroup.DOFade(1, fadeDuration);
        panelHolder.DOKill();
        
        panelHolder.DOScale(startScale, 0.01f);
        
        panelHolder.DOScale(endScale, scaleDuration).SetEase(scaleEase).OnComplete(delegate
        {
            canvasGroup.blocksRaycasts = true;
            OnShowEvent?.Invoke();
        });
    }

    public void ScaleMenuHide()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, fadeDuration).OnComplete(delegate
        {
            OnHideEvent?.Invoke();
            panelHolder.localScale = Vector3.zero;
        });
    }
    

    public  void SideMenuShow()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, fadeDuration);
        panelHolder.DOPivotY(1, 0.01f);
        panelHolder.DOPivotY(0, moveDuration).OnComplete(() =>
        {
            OnShowEvent?.Invoke();
            canvasGroup.blocksRaycasts = true;
        });
    }


    public  void SideMenuHide()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, fadeDuration);
        panelHolder.DOPivotY(0, 0.01f);
        panelHolder.DOPivotY(1, moveDuration).OnComplete(() =>
        {
            canvasGroup.alpha = 0;
            OnHideEvent?.Invoke();
        });

    }
    
    public void SetPanelPointer(Transform target)
    {
        dynamicMenuEnabled = true;
        sideMenuEnabled = false;
        panelPointer = target;
        UpdatePanelPosition();
    }

    public void DynamicMenuShow()
    {
        isDynamicMenuShow = true;
        OnShowEvent?.Invoke();

    }
    
    public void DynamicMenuHide()
    {
        OnHideEvent?.Invoke();
        isDynamicMenuShow = false;
    }
    
    private void UpdatePanelPosition()
    {
        if (panelPointer != null && mainCamera != null)
        {
            Vector3 wantedPos = mainCamera.WorldToScreenPoint(panelPointer.position);
            wantedPos.z = 0;
            panelHolder.transform.position = wantedPos;
        }
    }

    private void Update()
    {
        if (isDynamicMenuShow)
        {
            UpdatePanelPosition();
        }
    }
}

