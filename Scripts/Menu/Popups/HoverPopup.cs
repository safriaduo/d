using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dawnshard.Menu
{
    public class HoverPopup : Popup, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text text;
        private RectTransform parentRect;
        private float yOffset;

        public void SetText(string text)
        {
            this.text.text = text;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Open();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Close();
        }

        protected override void Start()
        {
            base.Start();
            parentRect = parentObject.GetComponent<RectTransform>();
            yOffset = parentRect.anchoredPosition.y;
        }

        void Update()
        {
            if (parentObject.activeSelf)
            {
                // Convert the screen mouse position to a local position relative to the parent
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, null, out localPoint))
                {
                    // Set the UI element's anchored position to the local point.
                    parentRect.anchoredPosition = localPoint + new Vector2(0, yOffset);
                }
            }
        }
    }
}