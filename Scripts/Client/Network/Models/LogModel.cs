using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Network
{
    public class LogModel 
    {
        public enum LogEntryActionType
        {
            AbilityOnCard,
            AbilityOnPlayer,
            Fight,
            Reap,
            Play,
            Discard,
            SelectWorld,
            StopTurn,
            Error
        }

        public LogModel(CardModel sourceCard, List<CardModel> cardTarget, List<PlayerModel> playerTarget, LogEntryActionType LogEntryType, bool localPlayer)
        {
            SourceCard = sourceCard;
            CardTarget = cardTarget;
            PlayerTarget = playerTarget;
            this.LogEntryType = LogEntryType;
            LocalPlayer = localPlayer;
        }

        public CardModel SourceCard { get; set; }
        public List<CardModel> CardTarget { get; set; }
        public List<PlayerModel> PlayerTarget { get; set; }
        public LogEntryActionType LogEntryType { get; set; }
        public Sprite ActionIcon { get; set; }
        public string ActionText { get; set; }
        public bool LocalPlayer { get; set; }
    }
}