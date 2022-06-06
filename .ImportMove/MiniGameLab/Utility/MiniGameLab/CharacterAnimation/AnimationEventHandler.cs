using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public CharacterAnimation characterAnimation;
   
    public void OnStateStart(string clipName)
    {
        if (characterAnimation.currentAnimationType.ToString() == clipName)
        {
            // characterAnimation.OnStartAnimationClipEvent?.Invoke();
        }
    }

    public void OnStateEnd(string clipName)
    {
        // Debug.Log(transform.parent.name + ": ClipName: " + clipName);
        if (characterAnimation.currentAnimationType.ToString() == clipName)
        {
            // characterAnimation.OnEndAnimationClipEvent?.Invoke();
        }
    }
}
