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
    public class LoginPopup : Popup
    {
        [SerializeField] protected TMP_InputField emailField;
        [SerializeField] protected TMP_InputField passwordField;
        [SerializeField] protected Button loginButton;

        protected override void Start()
        {
            base.Start();

            loginButton.onClick.AddListener(LoginAsync);
        }

        protected virtual async void LoginAsync()
        {
            try
            {
                //await GameController.Instance.EmailLogin(emailField.text, passwordField.text);
                
            }

            catch (Exception e)
            {
                ShowError(e.Message);
                Debug.LogException(e);

                return;
            }
        }
    }
}
