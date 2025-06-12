using UnityEngine;

[ExecuteAlways]
public class CustomLayoutGroup : MonoBehaviour
{
    public enum LayoutType { Horizontal, Vertical, Depth }
    public enum HorizontalAlignment { Left, Center, Right }
    public enum VerticalAlignment { Top, Middle, Bottom }

    public LayoutType layoutType = LayoutType.Horizontal; // Horizontal, Vertical, or Depth layout
    public float spacing = 1.0f; // Space between objects
    public bool autoResize = true; // Automatically update when the number of children changes

    // Alignment settings exposed to the inspector
    public HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
    public VerticalAlignment verticalAlignment = VerticalAlignment.Top;

    protected int previousChildCount = 0;

    void Start()
    {
        ArrangeChildren();
    }

    void Update()
    {
        if (autoResize && transform.childCount != previousChildCount)
        {
            ArrangeChildren();
            previousChildCount = transform.childCount;
        }
    }

    protected virtual void ArrangeChildren()
    {
        float totalWidth = 0f;
        float totalHeight = 0f;
        float totalDepth = 0f;

        // Calculate total width, height, or depth of children including spacing
        if (layoutType == LayoutType.Horizontal)
        {
            totalWidth = (transform.childCount - 1) * spacing; // Spacing between all children in horizontal layout
        }
        else if (layoutType == LayoutType.Vertical)
        {
            totalHeight = (transform.childCount - 1) * spacing; // Spacing between all children in vertical layout
        }
        else if (layoutType == LayoutType.Depth)
        {
            totalDepth = (transform.childCount - 1) * spacing; // Spacing between all children in depth layout
        }

        // Start offset calculation based on alignment
        float currentOffsetX = 0f;
        float currentOffsetY = 0f;
        float currentOffsetZ = 0f;

        if (layoutType == LayoutType.Horizontal)
        {
            currentOffsetX = GetHorizontalStartingOffset(totalWidth);
        }
        else if (layoutType == LayoutType.Vertical)
        {
            currentOffsetY = GetVerticalStartingOffset(totalHeight);
        }
        else if (layoutType == LayoutType.Depth)
        {
            currentOffsetZ = GetDepthStartingOffset(totalDepth);
        }

        // Arrange the children based on the selected layout type
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 newPosition = child.localPosition;

            if (layoutType == LayoutType.Horizontal)
            {
                newPosition.x = currentOffsetX;
                newPosition.y = GetVerticalAlignmentPosition();
                // Optionally handle z-position if needed
                newPosition.z = 0f;
                currentOffsetX += spacing; // Move by spacing only
            }
            else if (layoutType == LayoutType.Vertical)
            {
                newPosition.x = GetHorizontalAlignmentPosition();
                newPosition.y = currentOffsetY;
                // Optionally handle z-position if needed
                newPosition.z = 0f;
                currentOffsetY -= spacing; // Move by spacing only
            }
            else if (layoutType == LayoutType.Depth)
            {
                newPosition.x = GetHorizontalAlignmentPosition();
                newPosition.y = GetVerticalAlignmentPosition();
                newPosition.z = currentOffsetZ;
                currentOffsetZ -= spacing; // Move by spacing only
            }

            child.localPosition = newPosition;
        }
    }

    // Helper to calculate the starting offset for horizontal alignment
    private float GetHorizontalStartingOffset(float totalWidth)
    {
        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Center:
                return -totalWidth / 2f;
            case HorizontalAlignment.Right:
                return -totalWidth;
            case HorizontalAlignment.Left:
            default:
                return 0f;
        }
    }

    // Helper to calculate the starting offset for vertical alignment
    private float GetVerticalStartingOffset(float totalHeight)
    {
        switch (verticalAlignment)
        {
            case VerticalAlignment.Middle:
                return totalHeight / 2f;
            case VerticalAlignment.Bottom:
                return totalHeight;
            case VerticalAlignment.Top:
            default:
                return 0f;
        }
    }

    // Helper to calculate the starting offset for depth alignment
    private float GetDepthStartingOffset(float totalDepth)
    {
        // For depth, we can center the arrangement around 0
        return totalDepth / 2f;
    }

    // Helper to align objects vertically within horizontal layouts
    private float GetVerticalAlignmentPosition()
    {
        switch (verticalAlignment)
        {
            case VerticalAlignment.Middle:
                return 0f; // Center the object vertically (no offset)
            case VerticalAlignment.Bottom:
                return 0f; // Bottom alignment (can be adjusted if needed)
            case VerticalAlignment.Top:
            default:
                return 0f; // Top alignment (can be adjusted if needed)
        }
    }

    // Helper to align objects horizontally within vertical layouts
    private float GetHorizontalAlignmentPosition()
    {
        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Center:
                return 0f; // Center the object horizontally (no offset)
            case HorizontalAlignment.Right:
                return 0f; // Right alignment (can be adjusted if needed)
            case HorizontalAlignment.Left:
            default:
                return 0f; // Left alignment (can be adjusted if needed)
        }
    }
}
