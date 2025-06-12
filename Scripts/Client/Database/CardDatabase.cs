using Dawnshard.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dawnshard.Database
{
    public static class CardDatabase
    {
        private const string CARD_LIBRARY_PATH = "CardLibraries/";

        public static List<CardModel> Cards { get; private set; } = new List<CardModel>();

        public static CardModel GetCardById(int id)
        {
            return Cards.Find(c => c.ID == id);
        }

        public static CardModel GetCardByName(string name)
        {
            return Cards.Find(c => c.Name == name);
        }

        public static void Load()
        {
            // Load all files in the CardLibraries folder
            TextAsset[] cardFiles = Resources.LoadAll<TextAsset>(CARD_LIBRARY_PATH);

            // Initialize a list to hold all card records
            List<CardRecord> allCardsRecords = new List<CardRecord>();

            foreach (var file in cardFiles)
            {
                var cardRecords = JsonConvert.DeserializeObject<List<CardRecord>>(file.text);
                allCardsRecords.AddRange(cardRecords);
            }

            // Parse all card records to CardModel objects
            Cards = ParseCardRecords(allCardsRecords);

            Debug.Log("Card Database Loaded. Found " + Cards.Count + " cards.");
        }

        private static List<CardModel> ParseCardRecords(List<CardRecord> records)
        {
            List<CardModel> models = new List<CardModel>();

            for (int i = 0; i < records.Count; i++)
            {
                CardRecord record = records[i];

                CardModel cardModel = new CardModel
                {
                    ID = i+1,
                    Name = record.Name,
                    Type = record.Type,
                    BodyText = record.Description,
                    Rarity = record.Rarity,
                    WorldId = record.World,
                    ArtworkPath = $"Artworks/{record.World}/{record.Artwork}",
                    Stats = GetCardStatModels(record),
                    Abilities = new List<AbilityModel>()
                    {
                        new AbilityModel(record.Trigger1, record.Effect1, record.EffectParams1),
                        new AbilityModel(record.Trigger2, record.Effect2, record.EffectParams2),
                    }
                };

                models.Add(cardModel);
            }

            return models;
        }

        private static List<StatModel> GetCardStatModels(CardRecord record)
        {
            int attackValue = 0;
            int healthValue = 0;
            int shardValue = 0;
            int costValue = 0;

            if (!string.IsNullOrEmpty(record.Attack))
            {
                attackValue = GetFloatParameter(record.Attack);
            }

            if (!string.IsNullOrEmpty(record.Health))
            {
                healthValue = GetFloatParameter(record.Health);
            }

            if (!string.IsNullOrEmpty(record.Shard))
            {
                shardValue = GetFloatParameter(record.Shard);
            }

            if (!string.IsNullOrEmpty(record.Cost))
            {
                costValue = GetFloatParameter(record.Cost);
            }

            StatModel attackStatModel = new StatModel
            {
                ID = Constants.AttackStat,
                OriginalValue = attackValue,
                Value = attackValue,
            };

            StatModel healthStatModel = new StatModel
            {
                ID = Constants.HealthStat,
                OriginalValue = healthValue,
                Value = healthValue,
            };

            StatModel markStatModel = new StatModel
            {
                ID = Constants.MarkStat,
                OriginalValue = 0,
                Value = 0,
            };

            StatModel convertStatModel = new StatModel
            {
                ID = Constants.ConvertStat,
                OriginalValue = 0,
                Value = 0,
            };

            StatModel shardStatModel = new StatModel
            {
                ID = Constants.ShardStat,
                OriginalValue = shardValue,
                Value = shardValue,
            };

            StatModel costStatModel = new StatModel
            {
                ID = Constants.CostStat,
                OriginalValue = costValue,
                Value = costValue,
            };

            return new List<StatModel> { attackStatModel, healthStatModel, markStatModel, convertStatModel, shardStatModel, costStatModel };
        }

        public static int GetFloatParameter(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                s = s.Replace(",00", null).Replace(".00", null);
                try
                {
                    float x = float.Parse(s);
                    return (int)x;
                }
                catch (FormatException)
                {
                    throw new Exception("Cannot parse float parameter " + s);
                }
            }

            return 0;
        }

        private class CardRecord
        {
            public string Name { get; set; }
            public string World { get; set; }
            public string Type { get; set; }
            public string Rarity { get; set; }
            public string Description { get; set; }
            public string Keyword1 { get; set; }
            public string Keyword2 { get; set; }
            [JsonProperty("Trigger 1")]
            public string Trigger1 { get; set; }
            [JsonProperty("Effect 1")]
            public string Effect1 { get; set; }
            [JsonProperty("Effect Params 1")]
            public string EffectParams1 { get; set; }
            public string Target1 { get; set; }
            public string Condition1 { get; set; }
            public string ConditionParams1 { get; set; }
            [JsonProperty("Trigger 2")]
            public string Trigger2 { get; set; }
            [JsonProperty("Effect 2")]
            public string Effect2 { get; set; }
            [JsonProperty("Effect Params 2")]
            public string EffectParams2 { get; set; }
            public string Target2 { get; set; }
            public string Condition2 { get; set; }
            public string ConditionParams2 { get; set; }
            public string Artwork { get; set; }
            public string Lore { get; set; }
            public string Effect { get; set; }
            public string Traits { get; set; }
            public string Synergies { get; set; }
            public string Antisynergies { get; set; }
            public string Exceptions { get; set; }
            public string Cost { get; set; }
            public string Shard { get; set; }
            public string Attack { get; set; }
            public string Health { get; set; }
            public string ShardControlMin { get; set; }
            public string CreatureControlMin { get; set; }
            public string EfficiencyMin { get; set; }
            public string DisruptionMin { get; set; }
            public string CreatureProtectionMin { get; set; }
            public string EffectivePowerMin { get; set; }
            public string ExpectedShardMin { get; set; }
            public string OtherMin { get; set; }
            public string MinAERC { get; set; }
            public string ShardControlMax { get; set; }
            public string CreatureControlMax { get; set; }
            public string EfficiencyMax { get; set; }
            public string DisruptionMax { get; set; }
            public string CreatureProtectionMax { get; set; }
            public string EffectivePowerMax { get; set; }
            public string ExpectedShardMax { get; set; }
            public string OtherMax { get; set; }
            public string AERCPercent { get; set; }
            public string BaseAERC { get; set; }
            public string MaxAERC { get; set; }
        }
    }
}
