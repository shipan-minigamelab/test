using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandMouseCursor : MonoBehaviour
{
    [SerializeField] private RectTransform _holderRect;
    [SerializeField] private GameObject _downGO;
    [SerializeField] private GameObject _upGO;

    private void OnEnable()
    {
        _upGO.SetActive(true);
        _downGO.SetActive(false);
    }

    private void Update()
    {
        _holderRect.position = Vector3.Lerp(_holderRect.position, Input.mousePosition, 10 * Time.unscaledDeltaTime);
        if (Input.GetMouseButtonDown(0))
        {
            _downGO.SetActive(true);
            _upGO.SetActive(false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _upGO.SetActive(true);
            _downGO.SetActive(false);
        }
    }
}
