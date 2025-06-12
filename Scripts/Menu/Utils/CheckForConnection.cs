using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckForConnection : MonoBehaviour
{
    private const float CONNECTION_TIMEOUT = 2f;
    private float timeOutNetwork = 2f;
    [SerializeField] PopupOneButton connectionPopup;

    private void Start()
    {
        connectionPopup.SetUpPopupButton("It seems that you're not connected\nto the internet", "Cancel", () =>
        {
            connectionPopup.Close();
            timeOutNetwork = 2f;
        }, "Network Issue");
    }

    void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            timeOutNetwork -= Time.deltaTime;
            if (timeOutNetwork < 0)
            {
                connectionPopup.Open();
            }
        }
        else
        {
            if (timeOutNetwork <= 0)
            {
                CheckFormatch();
                //Qui bisogna controllare se c'è ancora il match
            }
            timeOutNetwork = CONNECTION_TIMEOUT;
        }
    }

    private async void CheckFormatch()
    {
        try
        {
            if (await GameController.Instance.IsUserStillInMatchAsync())
                connectionPopup.Close();
            else
            {
                connectionPopup.SetUpPopupButton("The match you were into is ended\nPlease go back to home",
                    "Go To Home", () => SceneManager.LoadScene(Constants.MenuScene), "Match ended");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
