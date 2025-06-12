using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogoutButton : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button> ();
        button.onClick.AddListener(Logout);
    }

    private async void Logout()
    {
        LoadingPopup.Instance.Open();
        await GameController.Instance.Logout();
        LoadingPopup.Instance.Close();
        SceneManager.LoadScene(Constants.AuthenticationScene);
    }
}
