using Newtonsoft.Json;

public class PlayCardRequest
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("cardTargetId")]
    public int CardTargetId { get; set; }
    [JsonProperty("replacedCardId")]
    public int ReplacedCardId { get; set; }
}

public class FightCreatureRequest
{
    [JsonProperty("attackerId")]
    public int AttackerId { get; set; }
    [JsonProperty("defenderId")]
    public int DefenderId { get; set; }
}

public class DiscardCardRequest
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
}

public class ReapCreatureRequest
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
}

public class StopTurnRequest
{
}

public class SelectActiveWorldRequest
{
    [JsonProperty("worldId")]
    public string WorldId { get; set; }
}

public class GetAvailableTargetsRequest
{
    [JsonProperty("cardId")]
    public int CardId { get; set; }
    [JsonProperty("triggerId")]
    public string TriggerId { get; set; }
}

public class MulliganRequest
{
    [JsonProperty("accepted")]
    public bool Accepted { get; set; }
}

public class ConcedeRequest
{
}

