using System.Collections.Generic;
using Newtonsoft.Json;

public class GameEvent
{
    public enum EventOneofCase
    {
        None = 0,
        CardMovedTriggered = 1,
        CardStatChanged = 2,
        PlayerStatChanged = 3,
        CardKeywordChanged = 4,
        CardReadyChanged = 5,
        ActiveWorldChanged = 6,
        CardAbilityTriggered = 7,
        PlayerAbilityTriggered = 8,
        CardWorldChanged = 9,
        ReapCreatureEvent = 10,
        FightCreatureEvent = 11,
        ZoneCardAdded = 12,
        ZoneCardRemoved = 13
    }

    [JsonIgnore]
    public EventOneofCase EventCase { get; set; } = EventOneofCase.None;

    [JsonProperty("cardMovedTriggered")]
    public CardMovedTriggered CardMovedTriggered { get; set; }
    [JsonProperty("cardStatChanged")]
    public CardStatChanged CardStatChanged { get; set; }
    [JsonProperty("playerStatChanged")]
    public PlayerStatChanged PlayerStatChanged { get; set; }
    [JsonProperty("cardKeywordChanged")]
    public CardKeywordChanged CardKeywordChanged { get; set; }
    [JsonProperty("cardReadyChanged")]
    public CardReadyChanged CardReadyChanged { get; set; }
    [JsonProperty("activeWorldChanged")]
    public ActiveWorldChanged ActiveWorldChanged { get; set; }
    [JsonProperty("cardAbilityTriggered")]
    public CardAbilityTriggered CardAbilityTriggered { get; set; }
    [JsonProperty("playerAbilityTriggered")]
    public PlayerAbilityTriggered PlayerAbilityTriggered { get; set; }
    [JsonProperty("cardWorldChanged")]
    public CardWorldChanged CardWorldChanged { get; set; }
    [JsonProperty("reapCreatureEvent")]
    public ReapCreatureEvent ReapCreatureEvent { get; set; }
    [JsonProperty("fightCreatureEvent")]
    public FightCreatureEvent FightCreatureEvent { get; set; }
    [JsonProperty("zoneCardAdded")]
    public ZoneCardAdded ZoneCardAdded { get; set; }
    [JsonProperty("zoneCardRemoved")]
    public ZoneCardRemoved ZoneCardRemoved { get; set; }
}

public class ZoneCardAdded
{
    [JsonProperty("cardData")]
    public CardDTO CardData { get; set; }
    [JsonProperty("zoneInstanceId")]
    public int ZoneInstanceId { get; set; }
    [JsonProperty("zoneId")]
    public string ZoneId { get; set; }
    [JsonProperty("cardInstanceId")]
    public int CardInstanceId { get; set; }
}

public class ZoneCardRemoved
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("zoneInstanceId")]
    public int ZoneInstanceId { get; set; }
}

public class CardMovedTriggered
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("origZoneId")]
    public string OrigZoneId { get; set; }
    [JsonProperty("destZoneId")]
    public string DestZoneId { get; set; }
}

public class CardStatChanged
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("stat")]
    public StatDTO Stat { get; set; }
    [JsonProperty("prevValue")]
    public int PrevValue { get; set; }
}

public class PlayerStatChanged
{
    [JsonProperty("playerId")]
    public string PlayerId { get; set; }
    [JsonProperty("stat")]
    public StatDTO Stat { get; set; }
    [JsonProperty("prevValue")]
    public int PrevValue { get; set; }
}

public class CardKeywordChanged
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("keywordId")]
    public string KeywordId { get; set; }
    [JsonProperty("added")]
    public bool Added { get; set; }
}

public class CardReadyChanged
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("canFight")]
    public bool CanFight { get; set; }
    [JsonProperty("canReap")]
    public bool CanReap { get; set; }
}

public class CardWorldChanged
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("worldId")]
    public string WorldId { get; set; }
}

public class ActiveWorldChanged
{
    [JsonProperty("playerId")]
    public string PlayerId { get; set; }
    [JsonProperty("worldId")]
    public string WorldId { get; set; }
    [JsonProperty("worldSelected")]
    public bool WorldSelected { get; set; }
}

public class CardAbilityTriggered
{
    [JsonProperty("ability")]
    public AbilityDTO Ability { get; set; }
    [JsonProperty("sourceCardId")]
    public int SourceCardId { get; set; }
    [JsonProperty("targetCardIds")]
    public List<int> TargetCardIds { get; set; } = new();
}

public class PlayerAbilityTriggered
{
    [JsonProperty("ability")]
    public AbilityDTO Ability { get; set; }
    [JsonProperty("sourceCardId")]
    public int SourceCardId { get; set; }
    [JsonProperty("targetPlayerIds")]
    public List<string> TargetPlayerIds { get; set; } = new();
}

public class FightCreatureEvent
{
    [JsonProperty("attackerId")]
    public int AttackerId { get; set; }
    [JsonProperty("defenderId")]
    public int DefenderId { get; set; }
}

public class ReapCreatureEvent
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
}

public class Error
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }
}

public class GameUpdate
{
    [JsonProperty("events")]
    public List<GameEvent> Events { get; set; } = new();
    [JsonProperty("error")]
    public Error Error { get; set; }
    [JsonProperty("gameState")]
    public GameStateDTO GameState { get; set; }
}

public class GameStart
{
    [JsonProperty("gameState")]
    public GameStateDTO GameState { get; set; }
    [JsonProperty("turnDuration")]
    public int TurnDuration { get; set; }
}

public class GameStateUpdate
{
    [JsonProperty("gameState")]
    public GameStateDTO GameState { get; set; }
}

public class GameEnd
{
    [JsonProperty("winnerId")]
    public string WinnerId { get; set; }
    [JsonProperty("battlePassExp")]
    public int BattlePassExp { get; set; }
    [JsonProperty("battlePassId")]
    public string BattlePassId { get; set; }
}

public enum OpCode
{
    Start = 0,
    Matchupdate = 1,
    Fight = 2,
    Reap = 3,
    Discard = 4,
    Play = 5,
    Stopturn = 6,
    Selectactiveworld = 7,
    Rejected = 8,
    End = 9,
    Opponentleft = 10,
    Gameupdate = 11,
    Mulligancompleted = 12,
    Concede = 13
}

