using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dawnshard.Menu;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AlturaConnectPopup : PopupInputField
{
    private Action OnLoginSuccess;
    [SerializeField] private Button alturaTutorialButton;
    [SerializeField] private GameObject background;
    [SerializeField] private Popup alturaTutorial;
    public override void Open()
    {
        base.Open();
        background.SetActive(true);
        OnLoginSuccess = () => Close();
    }

    public override void Close()
    {
        LoadingPopup.Instance.Close();
        background.SetActive(false);
        base.Close();
    }

    protected override void Start()
    {
        base.Start();
        button.onClick.AddListener(() => LoginWithAlturaGuardCode(inputField.text));
        alturaTutorialButton.onClick.AddListener(() =>
                alturaTutorial.Open()
            );
    }

    private void LoginWithAlturaGuardCode(string alturaGuardCode)
    {
        LoadingPopup.Instance.Open();
        Web3AuthAPI.Instance.LoginWithAltura(alturaGuardCode, OnLoginSuccess, OnException);
    }

    private void OnException(Exception e)
    {
        ErrorPopup.Instance.Open();
        ErrorPopup.Instance.SetErrorText(e.Message);
        LoadingPopup.Instance.Close();
    }

    public void SetFields(Action OnLoginSuccess)
    {
        this.OnLoginSuccess += OnLoginSuccess;
    }
}
