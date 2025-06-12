using System;
using System.Collections;
using System.Collections.Generic;
using AutoLetterbox;
using UnityEngine;

public class MobileGameSceneCamera : MonoBehaviour
{
    [SerializeField] private ForceCameraRatio forceCameraRatio;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AlignToCameraMargin alignToCameraMargin;

    [Header("FOV Settings")]
    [Tooltip("FOV at 4:3 (1.33 aspect ratio)")]
    [SerializeField] float fovAt43 = 70f;
    
    [Tooltip("FOV at 16:9 (1.78 aspect ratio)")]
    [SerializeField] float fovAt169 = 55f;
    
    [Tooltip("FOV at 18:9 (2.0 aspect ratio) and wider")]
    [SerializeField] float fovAt189 = 53f;
    [SerializeField] private bool ignoreWebGL = false;
    
    private void Awake()
    {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        forceCameraRatio.enabled = false;
        if(!mainCamera.orthographic)
            AdjustFOV();
#endif
        if (ignoreWebGL)
        {
            forceCameraRatio.enabled = false;
            if(!mainCamera.orthographic)
                AdjustFOV();
        }
    }
    
    public void AdjustFOV()
    {
        float aspect = (float)Screen.width / Screen.height;
        float newFOV = GetFOVForAspect(aspect);
        if (Camera.main != null)
        {
            Camera.main.fieldOfView = newFOV;
            Debug.Log($"Screen aspect: {aspect:F2} => Setting FOV to {newFOV:F2}");
        }
        else
        {
            Debug.LogWarning("Main camera not found.");
        }
    }
    
    private float GetFOVForAspect(float aspect)
    {
        float aspect43 = 4f / 3f;   // ≈1.33
        float aspect169 = 16f / 9f;  // ≈1.78
        float aspect189 = 18f / 9f;  // 2.0

        // If the screen is narrower than or equal to 4:3, use fovAt43.
        if (aspect <= aspect43)
            return fovAt43;
        
        // If the screen is wider than or equal to 18:9, use fovAt189.
        if (aspect >= aspect189)
            return fovAt189;
        
        // For aspect ratios between 4:3 and 16:9, interpolate from fovAt43 to fovAt169.
        if (aspect <= aspect169)
        {
            float t = (aspect - aspect43) / (aspect169 - aspect43);
            return Mathf.Lerp(fovAt43, fovAt169, t);
        }
        else // For aspect ratios between 16:9 and 18:9, interpolate from fovAt169 to fovAt189.
        {
            float t = (aspect - aspect169) / (aspect189 - aspect169);
            return Mathf.Lerp(fovAt169, fovAt189, t);
        }
    }
}
