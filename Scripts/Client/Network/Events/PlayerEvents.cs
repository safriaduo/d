using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using UnityEngine;

namespace Dawnshard.Network
{
    public class PlayerStatChangedEvent : IGameEvent
    {
        public StatModel Stat { get; private set; }
        public int PrevValue { get; private set; }
        
        public int CardId { get; private set; }

        public PlayerStatChangedEvent(StatModel stat, int prevValue)
        {
            Stat = stat;
            PrevValue = prevValue;
        }
    }
    
    public class ActiveWorldChangedEvent : IGameEvent
    {
        public string WorldId { get; private set; }
        public bool WorldSelected { get; private set; }

        public ActiveWorldChangedEvent(string worldId, bool worldSelected)
        {
            WorldId = worldId;
            WorldSelected = worldSelected;
        }
    }
    
    public class ChangeShardEvent : IGameEvent
    {
        public Vector3 CardPosition { get; private set; }
        public bool Steal { get; private set; }
        
        public bool HasShard { get; private set; }
        public bool IsOpponentShard { get; private set; }


        public ChangeShardEvent(Vector3 cardPosition, bool steal, bool hasShard, bool isOpponentShard=false)
        {
            CardPosition = cardPosition;
            Steal = steal;
            HasShard = hasShard;
            IsOpponentShard = isOpponentShard;
        }
    }
    
    public class TurnStartedEvent : IGameEvent
    {
        public TurnStartedEvent()
        { 
        }
    }

    public class TurnStoppedEvent : IGameEvent
    {
        public bool IsLocalPlayer { get; private set; }
        public TurnStoppedEvent(bool isLocalPlayer)
        {
            IsLocalPlayer = isLocalPlayer;
        }
    }
    
    public class StartDragEvent : IGameEvent
    {
        public CardModel CardModel;
        public StartDragEvent(CardModel cardModel)
        {
            CardModel = cardModel;
        }
    }
    
    public class EndDragEvent : IGameEvent
    {
        public CardModel CardModel;

        public EndDragEvent(CardModel cardModel)
        {
            CardModel = cardModel;
        }
    }
}
