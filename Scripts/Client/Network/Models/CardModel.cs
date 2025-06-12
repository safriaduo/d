using Dawnshard.Database;
using Dawnshard.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dawnshard.Network
{
    public class CardModel
    {
        public CardModel() { } 
        
        public string ZoneId { get; set; }

        public string BodyText { get; set; }

        public int ID { get; set; }

        public string IncandescenseLevel { get; set; }

        public string WorldId { get; set; }

        public string Name { get; set; }

        public string ArtworkPath { get; set; }

        public string Type { get; set; }

        public string Rarity { get; set; }

        public int InstanceId { get; set; }

        public string OwnerId { get; set; }
        
        public bool IsOwnerTurn { get; set; }

        public bool IsOwnerLocalPlayer => OwnerId == GameController.Instance.LocalPlayerId;

        public bool CanFight { get; set; }

        public bool CanReap { get; set; }

        public bool IsReady => CanFight || CanReap;

        public bool CanBePlayed { get; set; }

        /// <summary>
        /// This is a dictionary of actions that requires a target in order to perform.
        /// Players won't be able to perform the action if they don't select a target.
        /// </summary>
        public Dictionary<string, List<int>> AvailableTargetIds { get; set; }

        public List<string> Keywords { get; set; }

        public List<StatModel> Stats { get; set; }

        public List<AbilityModel> Abilities { get; set; }
    }
}