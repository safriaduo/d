using UnityEngine;

public class PreventSleepMode : MonoBehaviour
{
#if UNITY_ANDROID
    void Start()
    {
        // Impedisci al dispositivo di entrare in standby
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnApplicationQuit()
    {
        // Ripristina il comportamento predefinito quando l'app viene chiusa
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    } 
#endif
}
