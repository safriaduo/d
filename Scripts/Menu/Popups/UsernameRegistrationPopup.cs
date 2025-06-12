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
    public class UsernameRegistrationPopup : Popup
    {
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private Button registerButton;
        [SerializeField] private LoginWeb3Popup loginWeb3Popup;

        protected override void Start()
        {
            base.Start();
            CheckForUsername();
            //closeButton.gameObject.SetActive(false);
        }

        private void CheckForUsername()
        {
            registerButton.onClick.AddListener(RegisterAsync);
            if(TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered)
            {
                Close();
                return;
            }
            Open();
        }

        private async void RegisterAsync()
        {
            if (IsValidRegistration())
            {
                try
                {
                    await GameController.Instance.UpdateUsername(usernameField.text);
                    Close();

                    loginWeb3Popup.Open();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    ShowError(e.Message);
                    return;
                }
            }
            else
            {
                ShowError("The username must be at least 3 characters long.");
            }
        }

        private bool IsValidRegistration()
        {
            return usernameField.text.Length >= 3;
        }
    }
}
