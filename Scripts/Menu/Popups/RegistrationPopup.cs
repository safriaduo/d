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
    public class RegistrationPopup : Popup
    {
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField repeatPasswordField;
        [SerializeField] private Button registerButton;

        protected override void Start()
        {
            base.Start();

            //registerButton.onClick.AddListener(RegisterAsync);
        }

        //private async void RegisterAsync()
        //{
        //    if (IsValidRegistration())
        //    {
        //        try
        //        {
        //            //await GameController.Instance.EmailSignUp(emailField.text, passwordField.text, usernameField.text);
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.LogException(e);
        //            ShowError(e.Message);
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        ShowError("The password doesn't match");
        //    }
        //}

        private bool IsValidRegistration()
        {
            if (passwordField.text != repeatPasswordField.text)
            {
                return false;
            }

            return true;
        }

        public override void Close()
        {
            base.Close();

            emailField.text = "";
            usernameField.text = "";
            passwordField.text = "";
            repeatPasswordField.text = "";
        }

    }
}
