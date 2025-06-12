using Dawnshard.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dawnshard.Menu
{
    public class ConfirmPopup : Popup
    {
        [SerializeField] protected TMP_Text titleText;
        [SerializeField] protected TMP_Text bodyText;
        [SerializeField] protected TMP_Text confirmButtonText;
        [SerializeField] protected Button confirmButton;

        public void SetConfirmPopup(Action onClick, bool isConfirmInteractable, string bodyText="", string titleText="", string confirmButtonText="")
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                onClick();
                Close();
            });
            if(!string.IsNullOrEmpty(bodyText))
                this.bodyText.text = bodyText;
            if(!string.IsNullOrEmpty(confirmButtonText))
                this.confirmButtonText.text = confirmButtonText;
            if (!string.IsNullOrEmpty(titleText))
            {
                this.titleText.text = titleText;
            }
            confirmButton.interactable = isConfirmInteractable;
        }
        
    }
}