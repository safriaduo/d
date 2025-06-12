using System;
using System.Collections.Generic;
using System.Linq;
using GameAnalyticsSDK;
using UnityEngine;

public class GameSceneAnalytics : MonoBehaviour
{
    private float gameStartTime;
    private int turnDuration = 0;
    private bool playerHasResigned = false;
    private int localPlayerShard = 0;
    private int opponentPlayerShard = 0;
    private List<string> worldIds = new List<string>();
    private List<string> cardIds = new List<string>();
    private GameEndType endType = GameEndType.Loss;
    bool isRanked = false;
    

    public enum GameEndType
    {
        Loss,
        Win
    }
    
    
    void Start()
    {
        gameStartTime = Time.time;
    }

    public void IncreaseTurnDuration()
    {
        turnDuration++;
    }
    
    public void SetLocalPlayerShard(int playerShard)
    {
        localPlayerShard=playerShard;
    }
    
    public void PlayerHasConceded()
    {
        playerHasResigned=true;
    }
    
    public void SetOpponentPlayerShard(int opponentShard)
    {
        opponentPlayerShard=opponentShard;
    }
    
    public void SetCardIds(List<string> setNames)
    {
        foreach (string setName in setNames)
        {
            var world = setName.Split('_').First(); 
            var cards = setName.Split('_').Last();
            foreach (var card in cards)
                cardIds.Add(worldIds+"_"+card);
        }
    }
    
    public void SetWorldsAnalytics(List<string> worlds)
    {
        worldIds = worlds;
    }

    public void SetGameEndType(GameEndType gameEndType)
    {
        endType=gameEndType;
    }

    public void SetGameType(bool isRanked)
    {
        this.isRanked = isRanked;
    }

    private void OnDestroy()
    {
        float gameTime = Time.time - gameStartTime;

        foreach (var world in worldIds)
        {
            var ranked = isRanked ? "ranked" : "quest";
                GameAnalytics.NewDesignEvent($"match_summary:{endType.ToString().ToLower()}:{ranked}:{world}:duration_sec",
                    gameTime);
                GameAnalytics.NewDesignEvent($"match_summary:{endType.ToString().ToLower()}:{ranked}:{world}:turns",
                    turnDuration);
                GameAnalytics.NewDesignEvent($"match_summary:{endType.ToString().ToLower()}:{ranked}:{world}:shard",
                    localPlayerShard);
                GameAnalytics.NewDesignEvent($"match_summary:{endType.ToString().ToLower()}:{ranked}:{world}:opponent_shard",
                    opponentPlayerShard);

                if (playerHasResigned)
                {
                    GameAnalytics.NewDesignEvent($"match_summary:{endType.ToString().ToLower()}:{ranked}:{world}:resign",
                        1);
                }

        }
        
        Debug.Log("Logged match summary with GameAnalytics.");
    }
}
