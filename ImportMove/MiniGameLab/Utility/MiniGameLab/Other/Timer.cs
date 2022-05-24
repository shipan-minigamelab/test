using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Timer
{
    public float TotalTime = 140f;

    private Tweener TimerTweener;

    public Action OnTimerStart;
    public Action OnTimerPause;
    public Action OnTimerStop;
    public Action<float> OnTimerUpdate;
    public Action OnTimerEnd;

    private float currentTime;

    private float matchTimeLeft;
    private int currentMinute;
    private int currentSecond;
    private int tempSecond;

    public float PassedTime { get { return currentTime; } }
    public bool isRunning { get; private set; }

    public void StartTimer()
    {
        if (TimerTweener.IsActive()) TimerTweener.Kill();
        currentTime = 0;
        TimerTweener = DOTween.To(() => currentTime, x =>
        {
            currentTime = x;
            if (OnTimerUpdate != null)
            {
                // OnTimerUpdate(1 - (currentTime / TotalTime));
                OnTimerUpdate(TotalTime - currentTime);
            }
        },
        TotalTime, TotalTime)
            .SetEase(Ease.Linear)
            .OnPause(() => { if (OnTimerPause != null) OnTimerPause(); })
            .OnComplete(() => {
                isRunning = false;
                if (OnTimerEnd != null) OnTimerEnd(); 
            });

        if (OnTimerStart != null) OnTimerStart();
        isRunning = true;
    }

    public void Pause()
    {
        if (!TimerTweener.IsActive() || TimerTweener.IsComplete() || !TimerTweener.IsPlaying()) return;
        isRunning = false;
        TimerTweener.Pause();
    }

    public void Resume()
    {
        if (!TimerTweener.IsActive()) return;
        isRunning = true;
        TimerTweener.Play();
    }

    public void StopTimer()
    {
        if (!TimerTweener.IsActive()) return;
        isRunning = false;
        TimerTweener.Kill();
        currentTime = 0;
        OnTimerStop?.Invoke();
    }

    public void OnDestroy()
    {
        OnTimerStart = null;
        OnTimerPause = null;
        OnTimerUpdate = null;
        OnTimerEnd = null;
    }
}
