using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Database;
using Newtonsoft.Json;
using UnityEngine;

namespace Dawnshard.Network
{
    public class CardSetModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("worldId")]
        public string WorldId { get; set; }
        [JsonProperty("itemId")]
        public int ItemId { get; set; }
        [JsonProperty("incandescenseLevel")]
        public string IncandescenseLevel { get; set; }
        [JsonProperty("cardIDs")]
        public List<string> Cards { get; set; }

        /*public CardSetModel(CardSetDTO setDTO)
        {
            switch (setDTO.IncandescenseLevel)
            {
                case 0:
                {
                    IncandescenseLevel = Constants.VoidIncandescense;
                    break;
                }
                case 1:
                {
                    IncandescenseLevel = Constants.ShinyIncandescense;
                    break;
                }
                case 2:
                {
                    IncandescenseLevel = Constants.BlazingIncandescense;
                    break;
                }
                case 3:
                {
                    IncandescenseLevel = Constants.OneiricIncandescense;
                    break;
                }
                default:
                {
                    IncandescenseLevel = Constants.VoidIncandescense;
                    break;
                }

            }
            Name = setDTO.Name;
            UniqueId = setDTO.UniqueId.ToString();
            WorldId = setDTO.WorldId;
            Cards = setDTO.Cards.ToList();
        }*/
        
        public CardSetModel(string name, string worldId, string incandescenseLevel, List<string> cards)
        {
            IncandescenseLevel = incandescenseLevel;
            Name = name;
            WorldId = worldId;
            Cards = cards;
        }
        
        public CardSetModel(int itemId, string name, string worldId, string incandescenseLevel, List<string> cards)
        {
            ItemId = itemId;
            IncandescenseLevel = incandescenseLevel;
            Name = name;
            WorldId = worldId;
            Cards = cards;
        }
        
        public CardSetModel()
        {
            IncandescenseLevel = Constants.VoidIncandescense;
            Name = "No set";
            WorldId = "";
            Cards = new();
        }
    }
}
