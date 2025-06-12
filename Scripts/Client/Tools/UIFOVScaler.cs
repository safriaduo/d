using UnityEngine;

public class UIFOVScaler : MonoBehaviour
{
    [Tooltip("The camera whose FOV changes will affect the UI scale.")]
    public Camera targetCamera;

    [Tooltip("Baseline FOV at which the UI scale is unchanged (e.g., 60).")]
    public float baselineFOV = 60f;

    // The original local scale of the UI element.
    private Vector3 originalScale;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (targetCamera == null)
            return;

        // Compute the scaling factor using the tangent of half the angles.
        float baselineTan = Mathf.Tan(baselineFOV * 0.5f * Mathf.Deg2Rad);
        float currentTan = Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float scaleFactor = baselineTan / currentTan;

        // Apply the scaling factor. Non-uniform scaling can be applied if needed.
        transform.localScale = originalScale * scaleFactor;
    }
}