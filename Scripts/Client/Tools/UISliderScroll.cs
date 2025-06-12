using UnityEngine;
using UnityEngine.UI;

public class UISliderScroll : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect component
    public Slider slider; // Reference to the UI Slider

    private float contentWidth; // Width of the content
    private float viewportWidth; // Width of the viewport


    private void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        // Get the width of the content and viewport
        contentWidth = scrollRect.content.rect.width;
        viewportWidth = scrollRect.viewport.rect.width;

        // Check if there is a need for scrolling
        if (contentWidth <= viewportWidth)
        {
            slider.gameObject.SetActive(false); // Deactivate the slider
        }
        else
        {
            slider.gameObject.SetActive(true); // Activate the slider

            // Set the maximum value of the slider based on the content and viewport widths
            slider.maxValue = contentWidth - viewportWidth;
        }
    }

    public void OnSliderValueChanged(float value)
    {
        // Calculate the normalized position based on the slider value
        float normalizedPosition = value / (contentWidth - viewportWidth);

        // Set the horizontal normalized position of the ScrollRect
        scrollRect.horizontalNormalizedPosition = normalizedPosition;
    }
}