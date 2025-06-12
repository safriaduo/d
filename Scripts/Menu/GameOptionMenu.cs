using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Views;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionMenu : OptionsMenu
{
    [SerializeField] private Button concedeButton;
    [SerializeField] private GameObject mulliganUI;
    [SerializeField] private GameSceneAnalytics gameSceneAnalytics;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(UIManager.CurrentStateUI!=UIManager.StateUI.Pause)
                Open();
            else 
                Close();
        }
    }

    public override void Open()
    {
        base.Open();
        UIManager.CurrentStateUI = UIManager.StateUI.Pause;
    }

    public override void Close()
    {
        base.Close();
        UIManager.CurrentStateUI = UIManager.StateUI.None;
    }

    protected override void Start()
    {
        base.Start();
        if(concedeButton.gameObject.activeSelf)
            concedeButton.onClick.AddListener(() =>
            {
                ConcedeMatch();
                if (mulliganUI.activeSelf)
                {
                    mulliganUI.SetActive(false);
                }
            });
    }

    /// <summary>
    /// Call this method to concede the current match
    /// </summary>
    private async void ConcedeMatch()
    {
        Close();
        // Perform actions to concede the match
        gameSceneAnalytics.PlayerHasConceded();
        await GameController.Instance.ConcedeMatch();
        Debug.Log("Match conceded");
    }
}
