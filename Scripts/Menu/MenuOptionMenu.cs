using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuOptionMenu : OptionsMenu
{
    [SerializeField] private Button logoutButton;
    [SerializeField] private Button quitButton;

    protected override void Start()
    {
        base.Start();
        logoutButton.onClick.AddListener(() => LogoutGameAsync());
        quitButton.onClick.AddListener(() => QuitGame());
    }

    /// <summary>
    /// Call this method to quit the game
    /// </summary>
    private void QuitGame()
    {
        Application.Quit();

        Debug.Log("Quitted game");
    }

    /// <summary>
    /// Call this method to logout from the game
    /// </summary>
    private async void LogoutGameAsync()
    {
        //TODO: add AssetReferencer and uncomment
        LoadingPopup.Instance.Open();
        await GameController.Instance.Logout();
        LoadingPopup.Instance.Close();
        Close();
        SceneManager.LoadScene(Constants.AuthenticationScene);
    }
}
