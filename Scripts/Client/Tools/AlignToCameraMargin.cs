using UnityEngine;

public class AlignToCameraMargin : MonoBehaviour
{
    [Header("Alignment Options")]
    [Tooltip("If true, align the upper margin; if false, align the lower margin.")]
    public bool alignTop = true;

    [Tooltip("If true, move along the Y axis; if false, move along the Z axis.")]
    public bool moveAlongY = true;

    [Header("Camera Reference")]
    [Tooltip("The camera whose view is used for alignment. Defaults to Main Camera if not set.")]
    public Camera targetCamera;

    [Header("Perspective Settings")]
    [Tooltip("For perspective cameras, optionally specify a fixed distance for the conversion. Leave 0 to use object's current distance.")]
    public float perspectiveDistance = 0f;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        perspectiveDistance = targetCamera.fieldOfView;
    }

    /// <summary>
    /// Aligns the GameObject so that its upper or lower margin matches the camera's view margin.
    /// </summary>
    public void Align()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("Target camera not assigned.");
            return;
        }

        // Determine the offset between the object's pivot and its top or bottom edge.
        float offset = 0f;
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            Bounds bounds = rend.bounds;
            if (moveAlongY)
            {
                // Offset from pivot to top (if aligning upper margin) or bottom (if aligning lower margin)
                offset = alignTop ? (bounds.max.y - transform.position.y) : (transform.position.y - bounds.min.y);
            }
            else
            {
                // Offset from pivot to front (if aligning "upper" margin) or back (if aligning "lower" margin)
                offset = alignTop ? (bounds.max.z - transform.position.z) : (transform.position.z - bounds.min.z);
            }
        }
        else
        {
            Debug.LogWarning("Renderer not found on this GameObject. Assuming zero offset.");
        }

        // Determine the "distance" for viewport conversion.
        // For perspective cameras, we need the correct z distance.
        float distance = perspectiveDistance > 0 
            ? perspectiveDistance 
            : (moveAlongY ? Mathf.Abs(targetCamera.transform.position.z - transform.position.z) : 0f);

        if (moveAlongY)
        {
            // Get the top and bottom world positions in the camera view.
            Vector3 topWorld = targetCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, distance));
            Vector3 bottomWorld = targetCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, distance));

            // Compute desired Y position.
            float desiredY = alignTop ? (topWorld.y - offset) : (bottomWorld.y + offset);
            transform.position = new Vector3(transform.position.x, desiredY, transform.position.z);
        }
        else
        {
            // For movement along Z, assume the camera is oriented so that its vertical view corresponds to the Z axis.
            Vector3 topWorld = targetCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, distance));
            Vector3 bottomWorld = targetCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, distance));

            // Compute desired Z position.
            float desiredZ = alignTop ? (topWorld.z - offset) : (bottomWorld.z + offset);
            transform.position = new Vector3(transform.position.x, transform.position.y, desiredZ);
        }
    }

#if UNITY_EDITOR
    // This allows you to see changes immediately in the Editor.
    private void OnValidate()
    {
        Align();
    }
#endif

    // Optionally, call Align() at runtime when needed.
    private void Start()
    {
        Align();
    }
}
