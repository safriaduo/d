using UnityEngine;

[ExecuteAlways]
public class RotationalLayoutGroup : MonoBehaviour
{
    [Header("Arc Settings")]
    [Tooltip("Radius of the circle on which children will be arranged.")]
    public float radius = 200f;
    
    [Tooltip("Start angle in degrees for the leftmost card.")]
    public float startAngle = -45f;
    
    [Tooltip("End angle in degrees for the rightmost card.")]
    public float endAngle = 45f;
    
    [Tooltip("Offset of the circle's center relative to the parent's pivot (in local space).")]
    public Vector2 arcCenterOffset = Vector2.zero;
    
    [Header("Rotation Settings")]
    [Tooltip("Additional rotation offset (in degrees) applied to each child.")]
    public float rotationOffset = 0f;
    
    [Tooltip("If true, positions and rotations are applied in local space.")]
    public bool useLocal = true;

    void ApplyLayout()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
            return;
        
        // Determine the center of the arc.
        // If using local coordinates, the center is simply the arcCenterOffset.
        Vector3 center = useLocal ? (Vector3)arcCenterOffset : transform.position + (Vector3)arcCenterOffset;
        
        // For each child, compute an interpolated angle along the arc.
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            // Calculate t in [0,1]. If there's only one child, set it to the middle.
            float t = (childCount == 1) ? 0.5f : (float)i / (childCount - 1);
            // Interpolate angle from start to end.
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            // Convert angle to radians.
            float rad = angle * Mathf.Deg2Rad;
            // Compute the child's position along the arc.
            Vector3 pos = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
            
            if (useLocal)
                child.localPosition = pos;
            else
                child.position = pos;
            
            // Option 1: Rotate tangent to the arc.
            // The tangent to a circle at a given point is at an angle of (angle + 90).
            float desiredRotation = angle + 90f + rotationOffset;
            
            // Option 2: If you prefer the cards to face a specific direction, you can change this logic.
            if (useLocal)
                child.localRotation = Quaternion.Euler(0, 0, desiredRotation);
            else
                child.rotation = Quaternion.Euler(0, 0, desiredRotation);
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        ApplyLayout();
    }
    #endif

    private void Update()
    {
        // Update continuously if children might be added/removed or if parameters change at runtime.
        ApplyLayout();
    }
}
