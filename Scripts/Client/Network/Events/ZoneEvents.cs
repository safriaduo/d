using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Network
{
    public class ZoneCardRemovedEvent : IGameEvent
    {
        public int CardId { get; private set; }
        
        public ZoneCardRemovedEvent(int cardId)
        {
            CardId = cardId;
        }
    }

    public class ZoneCardAddedEvent : IGameEvent
    {
        public int CardId { get; private set; }

        public CardModel Card { get; private set; }

        public ZoneCardAddedEvent(int cardId, CardModel card)
        {
            CardId = cardId;
            Card = card;
        }
    }

    public class UpdateBoardEvent : IGameEvent
    {
        public UpdateBoardEvent(){}
    }
    
    public class ResponseElaborationEndedEvent : IGameEvent
    {
        public ResponseElaborationEndedEvent()
        {
            
        }
    }
}