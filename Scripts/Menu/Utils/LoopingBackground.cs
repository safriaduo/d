using UnityEngine;
using UnityEngine.UI;

public class LoopingBackground : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The RawImage that displays the background.")]
    [SerializeField] private RawImage backgroundRawImage;
    
    [Tooltip("The ScrollRect component that handles scrolling.")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("Scrolling Settings")]
    [Tooltip("The multiplier for the scrolling offset.")]
    [SerializeField] private float scrollSpeed = 1f;

    void Update()
    {
        // Example for horizontal scrolling:
        // Calculate the offset based on the horizontal content position.
        float offsetX = scrollRect.content.anchoredPosition.x * scrollSpeed;
        
        // Update the uvRect; here we use offsetX for horizontal scrolling.
        // Use modulo 1 to ensure the offset stays between 0 and 1.
        backgroundRawImage.uvRect = new Rect(offsetX % 1f,0, 1, 1);
    }
}
