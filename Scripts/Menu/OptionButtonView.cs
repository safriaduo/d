using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Linq;

namespace Dawnshard.Menu
{
    public class OptionButtonView : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab; // Prefab for the button
        [SerializeField] private Transform buttonContainer; // Parent transform for the buttons
        [SerializeField] private ToggleGroup toggleGroup;
        
        /// <summary>
        /// Set the submenu options buttons
        /// </summary>
        public void SetOptions(Dictionary<string, Action> options, string invoke,
            Dictionary<string, bool> notification = null, Dictionary<string, bool> lockedOptions = null)
        {
            // Clear existing buttons
            ClearButtons();
            Dictionary<string, Toggle> toggles = new();
            // Iterate through the dictionary and create buttons for each option
            foreach (var option in options)
            {
                // Instantiate the button prefab
                GameObject toggleGo = Instantiate(buttonPrefab, buttonContainer);
                Toggle toggle = toggleGo.GetComponentInChildren<Toggle>();
                toggle.group = toggleGroup;

                // Set the button's text
                var toggleText = toggleGo.GetComponentInChildren<TMP_Text>();
                toggleText.text = option.Key;

                if (option.Value != null)
                {
                    // Set the button's click event
                    toggle.onValueChanged.AddListener((bool isOn) =>
                    {
                        if (isOn)
                        {
                            option.Value.Invoke();
                        }
                    });

                    // if (toggleGo.transform.GetSiblingIndex() == 0)
                    // {
                    //     toggle.SetIsOnWithoutNotify(true);
                    // }
                    if (notification != null)
                    {
                        if(notification.ContainsKey(option.Key))
                            toggleGo.GetComponent<NotificationView>().ShowNotification();
                    }
                    if (lockedOptions != null)
                    {
                        if (lockedOptions.ContainsKey(option.Key))
                            toggle.interactable = false;
                    }
                }
                toggles.Add(option.Key,toggle);
            }
            if(!String.IsNullOrEmpty(invoke))
                toggles[invoke].isOn = true;
        }

        public void SetNotification()
        {
            
        }

        private void ClearButtons()
        {
            // Destroy all existing buttons in the container
            foreach (Transform button in buttonContainer)
            {
                Destroy(button.gameObject);
            }
        }
    }
}