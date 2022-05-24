using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[ExecuteAlways]
public class ShaderTime : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private float _max;
    private readonly int _shaderTimeKey = Shader.PropertyToID("ShaderTime");
    [SerializeField] private float _time = 0;

    private Tween _shaderTween;

    private void Start()
    {
        CommonStuffs.OnFlashEvent += ResetTimer;
        StartShaderTimer();
    }

    private void OnDestroy()
    {
        CommonStuffs.OnFlashEvent -= ResetTimer;
        if (_shaderTween != null && _shaderTween.IsActive()) _shaderTween.Kill();
    }

    private void StartShaderTimer()
    {
        _shaderTween = DOTween.To(() => _time, x => _time = x, _max, _speed)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetSpeedBased(true)
            .SetUpdate(UpdateType.Fixed)
            .OnUpdate(() => { Shader.SetGlobalFloat(_shaderTimeKey, _time); })
            .OnComplete(ResetTimer);
    }

    private void ResetTimer()
    {
        if (_time > (_max * 0.7f))
        {
            CommonStuffs.KillTween(_shaderTween);
            _shaderTween = DOTween.To(() => _time, x => _time = x, 0, 0f)
                .SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed)
                .OnUpdate(() => { Shader.SetGlobalFloat(_shaderTimeKey, _time); })
                .OnComplete(StartShaderTimer);
        }
    }
}