using System.Collections.Generic;
using Newtonsoft.Json;

public class ShopDTO
{
    [JsonProperty("Items")]
    public List<ShopItemDTO> Items { get; set; } = new();
}

public class ShopItemDTO
{
    [JsonProperty("Name")]
    public string Name { get; set; }
    [JsonProperty("Description")]
    public string Description { get; set; }
    [JsonProperty("SKU")]
    public string SKU { get; set; }
    [JsonProperty("ResourceSpent")]
    public string ResourceSpent { get; set; }
    [JsonProperty("ResourceObtained")]
    public string ResourceObtained { get; set; }
    [JsonProperty("Cost")]
    public int Cost { get; set; }
    [JsonProperty("AmountGained")]
    public int AmountGained { get; set; }
}

public class BattlePassDTO
{
    [JsonProperty("Name")]
    public string Name { get; set; }
    [JsonProperty("Levels")]
    public List<BattlePassLevelDTO> Levels { get; set; } = new();
}

public class BattlePassLevelDTO
{
    [JsonProperty("IsPremium")]
    public bool IsPremium { get; set; }
    [JsonProperty("LevelUpExperience")]
    public int LevelUpExperience { get; set; }
    [JsonProperty("Reward")]
    public BattlePassRewardDTO Reward { get; set; } = new();
}

public class BattlePassRewardDTO
{
    [JsonProperty("ResourceId")]
    public string ResourceId { get; set; }
    [JsonProperty("Amount")]
    public int Amount { get; set; }
}

public class DeckDTO
{
    [JsonProperty("Name")]
    public string Name { get; set; }
    [JsonProperty("CardSetIDs")]
    public List<string> CardSetIDs { get; set; } = new();
}

public class GameStateDTO
{
    [JsonProperty("CurrentPlayer")]
    public PlayerDTO CurrentPlayer { get; set; } = new();
    [JsonProperty("CurrentOpponent")]
    public PlayerDTO CurrentOpponent { get; set; } = new();
}

public class PlayerDTO
{
    [JsonProperty("ID")]
    public string ID { get; set; }
    [JsonProperty("Stats")]
    public List<StatDTO> Stats { get; set; } = new();
    [JsonProperty("Zones")]
    public List<ZoneDTO> Zones { get; set; } = new();
    [JsonProperty("ActiveWorldId")]
    public string ActiveWorldId { get; set; }
    [JsonProperty("IsWorldSelected")]
    public bool IsWorldSelected { get; set; }
    [JsonProperty("WorldIds")]
    public List<string> WorldIds { get; set; } = new();
    [JsonProperty("MulliganCompleted")]
    public bool MulliganCompleted { get; set; }
    [JsonProperty("Username")]
    public string Username { get; set; }
    [JsonProperty("DeckName")]
    public string DeckName { get; set; }
    [JsonProperty("CardSets")]
    public List<CardSetInfoDTO> CardSets { get; set; } = new();
}

public class CardSetInfoDTO
{
    [JsonProperty("Name")]
    public string Name { get; set; }
    [JsonProperty("Link")]
    public string Link { get; set; }
}

public class ZoneDTO
{
    [JsonProperty("InstanceID")]
    public int InstanceID { get; set; }
    [JsonProperty("Cards")]
    public List<CardDTO> Cards { get; set; } = new();
    [JsonProperty("MaxCards")]
    public int MaxCards { get; set; }
    [JsonProperty("NumCards")]
    public int NumCards { get; set; }
    [JsonProperty("ZoneID")]
    public string ZoneID { get; set; }
    [JsonProperty("ZoneVisibility")]
    public int ZoneVisibility { get; set; }
}

public class CardDTO
{
    [JsonProperty("InstanceID")]
    public int InstanceID { get; set; }
    [JsonProperty("Abilities")]
    public List<AbilityDTO> Abilities { get; set; } = new();
    [JsonProperty("Stats")]
    public List<StatDTO> Stats { get; set; } = new();
    [JsonProperty("Keywords")]
    public List<string> Keywords { get; set; } = new();
    [JsonProperty("OwnerID")]
    public string OwnerID { get; set; }
    [JsonProperty("CanFight")]
    public bool CanFight { get; set; }
    [JsonProperty("CanReap")]
    public bool CanReap { get; set; }
    [JsonProperty("ID")]
    public int ID { get; set; }
    [JsonProperty("AvailableTargets")]
    public List<AvailableTargetsDTO> AvailableTargets { get; set; } = new();
    [JsonProperty("CanPlay")]
    public bool CanPlay { get; set; }
    [JsonProperty("Incandescense")]
    public string Incandescense { get; set; }
}

public class AvailableTargetsDTO
{
    [JsonProperty("ActionID")]
    public string ActionID { get; set; }
    [JsonProperty("AvailableTargetIDs")]
    public List<int> AvailableTargetIDs { get; set; } = new();
}

public class TargetsList
{
    [JsonProperty("CardId")]
    public int CardId { get; set; }
    [JsonProperty("Targets")]
    public List<AvailableTargetsDTO> Targets { get; set; } = new();
}

public class StatDTO
{
    [JsonProperty("ID")]
    public string ID { get; set; }
    [JsonProperty("Value")]
    public int Value { get; set; }
    [JsonProperty("OriginalValue")]
    public int OriginalValue { get; set; }
    [JsonProperty("MaxValue")]
    public int MaxValue { get; set; }
}

public class AbilityDTO
{
    [JsonProperty("TriggerID")]
    public string TriggerID { get; set; }
    [JsonProperty("EffectID")]
    public string EffectID { get; set; }
}

