using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameAnalyticsSDK;
using UnityEngine.SceneManagement;

public class GameAnalyticsAPI : MonoBehaviour, IGameAnalyticsATTListener
{
    private float sessionStartTime;
    private int numberOfGamesPerSession=0;
    private bool isPlayerPlayingGame = false;

    public static GameAnalyticsAPI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            GameAnalytics.Initialize();
        }
    }
    
    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }
}