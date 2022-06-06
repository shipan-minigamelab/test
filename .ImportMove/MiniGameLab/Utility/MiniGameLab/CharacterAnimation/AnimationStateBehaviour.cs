using System;
using UnityEngine;

public class AnimationStateBehaviour : StateMachineBehaviour
{
    public AnimationType AnimationType;
    public Action<AnimationType> OnStateEnterEvent;
    public Action<AnimationType> OnStateExitEvent;
    public Action<AnimationType> OnStateEndEvent;
    [SerializeField] private float currentClipTime = 0;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentClipTime = 0;
        OnStateEnterEvent?.Invoke(AnimationType);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentClipTime = 0;
        OnStateExitEvent?.Invoke(AnimationType);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentClipTime += Time.deltaTime;
        if (currentClipTime >= stateInfo.length)
        {
            currentClipTime = 0;
            OnStateEndEvent?.Invoke(AnimationType);
        }
    }
}