using UnityEngine;
using UnityEngine.UI;

public class BackgroundScrolling: MonoBehaviour
{
    [Header("References")]
    [Tooltip("The background image's RectTransform. It should be a child of a viewport/panel.")]
    public RectTransform backgroundRect;

    [Tooltip("The scrollbar controlling the horizontal scroll.")]
    public Scrollbar scrollbar;

    private float surplus;   // Extra width beyond the viewport
    private float initialX;  // The initial anchored X position (assumed to be left-aligned)

    void Start()
    {
        // Get the viewport RectTransform from the background's parent.
        RectTransform viewportRect = backgroundRect.parent.GetComponent<RectTransform>();
        float viewportWidth = viewportRect.rect.width;
        float bgWidth = backgroundRect.rect.width;

        // Calculate how much wider the background is than the viewport.
        surplus = bgWidth - viewportWidth;

        // Save the initial X position of the background (assuming left-aligned)
        initialX = backgroundRect.anchoredPosition.x;
    }

    void Update()
    {
        if(!scrollbar.gameObject.activeSelf)
            return;
        // Map the scrollbar value (0 to 1) to an offset.
        // At 0, offset is 0 (background is left-aligned).
        // At 1, offset is -surplus (background is shifted left so its right edge aligns with the viewport's right edge).
        var scrollBarClamped = Mathf.Clamp(scrollbar.value, -0, 1);
        float offset = -scrollBarClamped * surplus;

        // Update the background's anchored position while keeping the Y position unchanged.
        backgroundRect.anchoredPosition = new Vector2(initialX + offset, backgroundRect.anchoredPosition.y);
    }
}
