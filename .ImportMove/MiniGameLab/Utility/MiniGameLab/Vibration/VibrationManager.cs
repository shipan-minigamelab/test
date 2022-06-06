using MoreMountains.NiceVibrations;
using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager instance;

    public float DelayBetweenVibration = 100f;
    private float LastVibrationStartedAt = -1;
    private bool VibrationAllowed = true;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        // int defaultValue = 1;
// #if UNITY_ANDROID
//         defaultValue = 0;
// #elif UNITY_IOS
//             defaultValue = 1;
// #endif
    }

    private void Start()
    {
    }

    public void ToggleVibration(int value)
    {
        VibrationAllowed = value == 0 ? false : true;
        // set global settings Vibration/Haptic value here ...
    }
    
    public void ToggleVibration(bool value)
    {
        VibrationAllowed = value;
        // set global settings Vibration/Haptic value here ...
    }

    public bool IsVibrationOn()
    {
        return VibrationAllowed;
    }

    public void PlayHapticLight()
    {
        if(!VibrationAllowed) return;
        if (LastVibrationStartedAt == -1) LastVibrationStartedAt = Time.time * 1000;
        else
        {
            float diff = Time.time * 1000 - LastVibrationStartedAt;
            if(diff <= DelayBetweenVibration) return;
        }
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    /// <summary>
    /// CAUTION : Use only if will be called once, not frequently...
    /// </summary>
    public void PlayHapticLightForced()
    {
        if(!VibrationAllowed) return;
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    public void PlayHapticMedium()
    {
        if(!VibrationAllowed) return;
        if (LastVibrationStartedAt == -1) LastVibrationStartedAt = Time.time * 1000;
        else
        {
            float diff = Time.time * 1000 - LastVibrationStartedAt;
            if(diff <= DelayBetweenVibration) return;
        }
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    /// <summary>
    /// CAUTION : Use only if will be called once, not frequently...
    /// </summary>
    public void PlayHapticMediumForced()
    {
        if(!VibrationAllowed) return;
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    public void PlayHapticHeavy()
    {
        if(!VibrationAllowed) return;
        if (LastVibrationStartedAt == -1) LastVibrationStartedAt = Time.time * 1000;
        else
        {
            float diff = Time.time * 1000 - LastVibrationStartedAt;
            if(diff <= DelayBetweenVibration) return;
        }
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
    
    /// <summary>
    /// CAUTION : Use only if will be called once, not frequently...
    /// </summary>
    public void PlayHapticHeavyForced()
    {
        if(!VibrationAllowed) return;
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        LastVibrationStartedAt = Time.time * 1000;
    }
}