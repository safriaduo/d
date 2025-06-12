using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarResizer : MonoBehaviour
{
    [Header("Baseline Resolution (16:9)")]
    [Tooltip("For example, 1920 for width.")]
    public float baselineWidth = 16;
    
    [Tooltip("For example, 1080 for height.")]
    public float baselineHeight = 9;

    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    void Awake()
    {
        ResizeScene();
    }

    


    void ResizeScene()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // Compute independent scaling factors for X and Y.
        float baseScreenRatio = baselineWidth / baselineHeight;
        
        // Apply non-uniform scaling (Z is left as 1).
        horizontalLayoutGroup.padding.left = Mathf.RoundToInt((float)horizontalLayoutGroup.padding.left*screenWidth/screenHeight/baseScreenRatio);
    }
}
