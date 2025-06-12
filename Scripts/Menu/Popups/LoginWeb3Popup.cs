using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dawnshard.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Menu
{
    public class LoginWeb3Popup : LoginPopup
    {
        [SerializeField] private Button googleButton;
        [SerializeField] private Button appleButton;
        [SerializeField] private Button walletButton;
        [SerializeField] private AlturaConnectPopup alturaConnectPopup;
        [SerializeField] private Popup launchArenaPopup = null;


        private const string MATCH_EMAIL_PATTERN =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
            + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        private bool IsEmailValid(string email)
        {
            return email != null && Regex.IsMatch(email, MATCH_EMAIL_PATTERN);
        }

        protected override void Start()
        {
            base.Start();
            googleButton.onClick.AddListener(() =>
            {
                LoginWithSocialProvider(Provider.GOOGLE);
            });
            appleButton.onClick.AddListener(() =>
            {
                LoginWithSocialProvider(Provider.APPLE);
            });
            loginButton.onClick.AddListener(() =>
            {
                LoginWithSocialProvider(Provider.EMAIL_PASSWORDLESS, emailField.text);
            });
            walletButton.onClick.AddListener(() =>
            {
                alturaConnectPopup.Open();
                alturaConnectPopup.SetFields(OnSuccess);
            });
            emailField.onValueChanged.AddListener(emailText =>
            {
                loginButton.interactable = IsEmailValid(emailText);
            });
            loginButton.interactable = false;
        }

        protected void LoginWithSocialProvider(Provider provider, string email = "")
        {
            LoadingPopup.Instance.Open();
            try
            {
                Web3AuthAPI.Instance.Login(provider,
                    email,
                   OnSuccess);
            }

            catch (Exception e)
            {
                ShowError(e.Message);
                Debug.LogException(e);

                return;
            }
        }

        private async void OnSuccess()
        {
            await GameController.Instance.RefreshAssets();
            LoadingPopup.Instance.Close();
            Close();
        }
    }
}
