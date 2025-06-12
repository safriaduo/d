using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Network
{
    public class CardKeywordChangedEvent : IGameEvent
    {
        public string KeywordId { get; private set; }
        public bool Added { get; private set; }

        public CardKeywordChangedEvent(string keywordId, bool added)
        {
            KeywordId = keywordId;
            Added = added;
        }
    }

    public class CardStatChangedEvent : IGameEvent
    {
        public StatModel Stat { get; private set; }
        public int PrevValue { get; private set; }

        public CardStatChangedEvent(StatModel stat, int prevValue)
        {
            Stat = stat;
            PrevValue = prevValue;
        }
    }

    public class CardReadyChangedEvent : IGameEvent
    {
        public bool CanFight { get; private set; }
        public bool CanReap { get; private set; }

        public CardReadyChangedEvent(bool canFight, bool canReap)
        {
            CanFight = canFight;
            CanReap = canReap;
        }
    }

    public class CardTargettableEvent : IGameEvent
    {
        public bool StartedTarget { get; private set; }
        public bool Replace { get; private set; }
        public bool Fight { get; private set; }


        public CardTargettableEvent(bool startedTarget, bool replace, bool fight)
        {
            StartedTarget = startedTarget;
            Replace = replace;
            Fight = fight;
        }
    }

    public class CardWorldChangedEvent : IGameEvent
    {
        public string WorldId { get; private set; }

        public CardWorldChangedEvent(string worldId)
        {
            WorldId = worldId;
        }
    }

    public class ReapCreatureTriggered : IGameEvent
    {
        public int ReapCardId { get; private set; }
        public ReapCreatureTriggered(int reapCardId)
        {
            ReapCardId = reapCardId;
        }
    }


    public class FightCreatureTriggered : IGameEvent
    {
        //public bool IsAttacking { get; private set; }
        public int DefenderId{ get; private set; }
        public int AttackerId{ get; private set; }
         
        public FightCreatureTriggered(int attackerId, int defenderId)
        {
            DefenderId = defenderId;
            AttackerId = attackerId;
        }
    }


    public class CardAbilityTriggeredEvent : IGameEvent
    {
        public AbilityModel Ability { get; private set; }
        public List<int> TargetIds { get; private set; }
        public int CardInstanceId { get; private set; }

        public CardAbilityTriggeredEvent(AbilityModel ability, List<int> targetIds, int cardInstanceId)
        {
            Ability = ability;
            TargetIds = targetIds;
            CardInstanceId = cardInstanceId;
        }
    }
    
    public class PlayerAbilityTriggeredEvent : IGameEvent
    {
        public string TriggerId { get; private set; }
        public string EffectId { get; private set; }
        public int CardInstanceId { get; private set; }

        public List<string> TargetPlayerIDs { get; private set; }

        public PlayerAbilityTriggeredEvent(string triggerId, string effectId, int cardInstanceId, List<string> targetPlayerIds)
        {
            TriggerId = triggerId;
            EffectId = effectId;
            CardInstanceId = cardInstanceId;
            TargetPlayerIDs = targetPlayerIds;
        }
    }

    public class CardMovedEvent : IGameEvent
    {
        public string OrigZoneId { get; private set; }
        public string DestZoneId { get; private set; }
        public int CardInstanceId { get; private set; }


        public CardMovedEvent(string origZoneId, string destZoneId, int cardInstanceId)
        {
            OrigZoneId = origZoneId;
            DestZoneId = destZoneId;
            CardInstanceId = cardInstanceId;
        }
    }
    
    public class CardTargetsUpdateEvent : IGameEvent
    {
        public Dictionary<string, List<int>> TargetsDictionary { get; private set; }
        public CardTargetsUpdateEvent(Dictionary<string, List<int>> targetsDictionary)
        {
            TargetsDictionary = targetsDictionary;
        }
    }
    
    public class CardToAttackEvent : IGameEvent
    {
        public Vector3 DefenderCardPos { get; private set; }
        public CardToAttackEvent(Vector3 opponentCardPos)
        {
            DefenderCardPos = opponentCardPos;
        }
    }

    public class HighlightCardEvent : IGameEvent
    {
        public HighlightCardEvent()
        {
            
        }
    }
}