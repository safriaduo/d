using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeleteAccountButton : MonoBehaviour
{
    [SerializeField] private Button deleteButton;

    private void Start()
    {
        deleteButton.onClick.AddListener(DeleteUser);
    }

    private async void DeleteUser()
    {
        await GameController.Instance.DeleteUser();
        await GameController.Instance.Logout();
        SceneManager.LoadScene(Constants.AuthenticationScene);
    }
}
