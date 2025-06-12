using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewAccountButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(DeviceLogin);
    }

    private async void DeviceLogin()
    {
        try
        {
            LoadingPopup.Instance.Open();
            await GameController.Instance.DeviceLogin();
            //LoadingPopup.Instance.Close();
        }

        catch (Exception e)
        {
            Debug.LogException(e);
            ErrorPopup.Instance.Open();
            ErrorPopup.Instance.SetErrorText(e.Message);
            return;
        }
    }
}
