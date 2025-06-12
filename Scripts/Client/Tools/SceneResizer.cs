using System;
using UnityEngine;

public class SceneResizer : MonoBehaviour
{
    [Header("Baseline Resolution (16:9)")]
    [Tooltip("For example, 1920 for width.")]
    public float baselineWidth = 16;
    
    [Tooltip("For example, 1080 for height.")]
    public float baselineHeight = 9;

    void Awake()
    {
        ResizeScene();
    }

    


    void ResizeScene()
    {
        // Get current screen dimensions.
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // Compute independent scaling factors for X and Y.
        float baseScreenRatio = baselineWidth / baselineHeight;
        
        // Apply non-uniform scaling (Z is left as 1).
        transform.localScale = new Vector3(screenWidth/screenHeight/baseScreenRatio, 1, 1f);
    }
}
