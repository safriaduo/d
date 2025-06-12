using System.Collections.Generic;

public static class Constants
{
    public const string MenuTutorialPrefs = "menu.tutorial";

    //------------------STATES-------------------//
    public const string PlayState = "PlayState";
    public const string CollectionState = "CollectionState";
    public const string RewardsState = "RewardsState";
    public const string HomeState = "HomeState";
    public const string BattlePassState = "BattlePassState";
    public const string ShopState = "ShopState";
    public const string ForgeState = "ForgeState";


    //------------------SCENES------------------//
    public const string AuthenticationScene = "LoginScene";
    public const string GameScene = "GameScene";
    public const string MobileGameScene = "MobileGameScene";
    public const string MenuScene = "MenuScene";
    
    //------------------GAME MODES------------------//
    public const string RankedMode = "ranked";
    public const string ReverseMode = "reversed";

    
    //------------------OPTION MENU------------------//
    public const string PlayStateQuest = "Quest";
    public const string PlayStateFriendly = "Training";
    public const string PlayStateRanked = "Ranked";
    public const string PlayStateReverse = "Sabotage";

    
    public const string CollectionStateSets = "Sets";
    public const string CollectionStateDecks = "Decks";
    public const string CollectionStateCreateDeck = "Create Deck";
    
    public const string ForgeStateForge = "Forge";
    public const string ForgeStateBurn = "Burn";


    //----------------CARD TYPES----------------//
    public const string CreatureType = "Creature";
    public const string ActionType = "Action";
    public const string ArtifactType = "Artifact";

    //----------------CARD STATS----------------//
    public const string AttackStat = "Attack";
    public const string HealthStat = "Health";
    public const string MarkStat = "Mark";
    public const string CostStat = "Cost";
    public const string ConvertStat = "Convert";

    //---------------PLAYER STATS---------------//
    public const string ShardStat = "Shard";
    public const string CardLimitStat = "CardLimit";

    //-----------------KEYWORDS-----------------//
    public const string ShieldKeyword = "Shield";
    public const string ExhaustKeyword = "Exhaust";
    public const string HasteKeyword = "Haste";
    public const string DeadlyKeyword = "Deadly";
    public const string HiddenKeyword = "Hidden";
    public const string SilenceKeyword = "Silence";
    public const string BrutalityKeyword = "Brutality";
    public const string EphemeralKeyword = "Ephemeral";
    public const string StaticKeyword = "Static";
    public const string ProvokeKeyword = "Provoke";
    public static readonly string[] KeywordText = new string[8]
    {
        "Shield",
        "Haste",
        "Deadly",
        "Hidden",
        "Brutality",
        "Ephemeral",
        "Static",
        "Provoke"
    };

    //------------------ZONES------------------//
    public const string BoardZone = "Board";
    public const string ActionZone = "Action";
    public const string HandZone = "Hand";
    public const string GraveyardZone = "Graveyard";
    public const string DeckZone = "Deck";

    //-------------PLAYER TARGETS-------------//
    public const string PlayerTarget = "PlayerTarget";
    public const string OpponentTarget = "OpponentTarget";
    public const string AllPlayers = "AllPlayers";

    //--------------COMPUTED CARD TARGETS--------------//
    public const string ThisCard = "ThisCard";
    public const string AllCards = "AllCards";
    public const string AllPlayerCards = "AllPlayerCards";
    public const string AllOpponentCards = "AllOpponentCards";
    public const string WeakestPlayerCreature = "WeakestPlayerCreature";
    public const string WeakestOpponentCreature = "WeakestOpponentCreature";
    public const string StrongestPlayerCreature = "StrongestPlayerCreature";
    public const string StrongestOpponentCreature = "StrongestOpponentCreature";
    public const string RandomCreature = "RandomCreature";
    public const string RandomPlayerCreature = "RandomPlayerCreature";
    public const string RandomOpponentCreature = "RandomOpponentCreature";

    //--------------SELECTED CARD TARGETS--------------//
    public const string PlayerCard = "PlayerCard";
    public const string OpponentCard = "OpponentCard";
    public const string PlayerCardAndBloodbound = "PlayerCardAndBloodbound";
    public const string OpponentCardAndBloodbound = "OpponentCardAndBloodbound";
    public const string TargetCard = "TargetCard";
    
    //--------------SELECTED CARD TARGETS TEXT--------------//
    public const string BloodboundText = "Bloodbound";
    
    //----------------TRIGGERS----------------//
    public const string OnPlay = "OnPlay";
    public const string OnPlayAndReap = "OnPlayAndReap";
    public const string OnPlayAndFight = "OnPlayAndFight";
    public const string OnFight = "OnFight";
    public const string OnFightAndReap = "OnFightAndReap";
    public const string OnReap = "OnReap";
    public const string OnDraw = "OnDraw";
    public const string OnDestroy = "OnDestroy";
    public const string TurnStart = "TurnStart";
    public const string TurnEnd = "TurnEnd";
    
    //----------------TRIGGERS TEXT----------------//
    public static readonly string[] TriggerText = new string[9]
    {
        "Play",
        "Play and Reap",
        "Play and Fight",
        "Fight and Reap",
        "Fight",
        "Reap",
        "Destroyed",
        "Turn Start",
        "Turn End"
    };
    
    //----------------OPERATIONS----------------//
    public const string GraterOperation = ">";
    public const string EqualOperation = "=";
    public const string LesserOperation = "<";

    //----------------KEY VALUES----------------//
    public const int ShardToWin = 10;

    //----------------EFFECT IDS----------------//
    public const string CreateCardEffect = "CreateCard";
    public const string AddKeywordEffect = "AddKeyword";
    public const string StealShardEffect = "StealShard";
    public const string LoseShardEffect = "LoseShard";
    public const string LockShardEffect = "LockShard";
    public const string GainShardEffect = "GainShard";
    public const string ReadyCreatureEffect = "ReadyCreature";

    public static readonly string[] EffectsText = new string[]
    {
        "Silence",
        "Exhaust",
        "Lock",
        "Mark",
        "Convert",
        "Bloodbound"
    };
    //----------------TARGET TYPES----------------//
    public const string FightAction = "Fight";
    public const string PlayAction = "Play";
    public const string ReplaceAction = "Replace";

    //----------------TARGET LAYERS----------------//
    public const string ReapLayer = "PlayerShard";
    public const string GraveyardLayer = "Graveyard";
    public const string BoardLayer = "Battlefield";
    public const string CardLayer = "Card";

    //----------------INCANDESCENSE----------------//
    public const string VoidIncandescense = "Void";
    public const string OneiricIncandescense = "Oneiric";
    public const string BlazingIncandescense = "Blazing";
    public const string ShinyIncandescense = "Shiny";
    #region IncandescenseConversion
    public static int IncandescenseStringToInt(string incandescenceLevel)
    {
        switch (incandescenceLevel)
        {
            case Constants.VoidIncandescense:
            {
                return 0;
                break;
            }
            case Constants.ShinyIncandescense:
            {
                return 1;
            }
            case Constants.BlazingIncandescense:
            {
                return 2;
            }
            case Constants.OneiricIncandescense:
            {
                return 3;
            }
            default:
            {
                return -1;
            }

        }
    }
    public static string IncandescenseIntToString(int incandescenceLevel)
    {
        switch (incandescenceLevel)
        {
            case 0:
            {
                return Constants.VoidIncandescense;
                break;
            }
            case 1:
            {
                return Constants.ShinyIncandescense;
                break;
            }
            case 2:
            {
                return Constants.BlazingIncandescense;
            }
            case 3:
            {
                return Constants.OneiricIncandescense;
            }
            default:
            {
                return "Error, no such level of incandescense exists";
            }

        }
    }
    #endregion

    //----------------OPTION PLAYER PREFS----------------//
    public const string LanguageSetting = "LanguageSetting";
    public const string SoundEffectsSetting = "SoundEffectsSetting";
    public const string MusicSetting = "MusicSetting";
    public const string MuteSetting = "MuteSetting";
    
    //----------------RARITY----------------//
    public const string Music = "Music";
    public const string SFX = "SFX";

    //----------------RARITY----------------//
    public const string CommonRarity = "Common";
    public const string UncommonRarity = "Uncommon";
    public const string RareRarity = "Rare";
    //----------------CURRENCY----------------//
    public static string Tethras = "tethras";
    public static string Debristar = "debristar";
    public static string StandardPack = "standard_pack";
    public static string ShinyPack = "shiny_pack";
    public static string BlazingPack = "blazing_pack";
    public static string WorldPack = "world_pack";
    public static string BattlePass = "premium_battle_pass";
    public static string BattlePassAndroid = "com.deepmonolith.dawnshard.premium_battle_pass";
    public static string BattlePassiOS = "com.DeepMonolith.Dawnshard.premium_battle_pass";


    //----------------PROPERTIES----------------//
    public static string IncandescenseProperty = "Incandescense";
    public static string WorldIdProperty = "WorldId";
    public static string UniqueIdProperty = "UniqueId";
    public static string CardProperty = "Card";
    //----------------Ranks----------------//
    public static string WandererRank = "Wanderer";
    public static string AscendantRank = "Ascendant";
    public static string LaunchArenaLeaderboard = "Launch Arena";
    public const string WeekendTournament = "Weekend Ranked";
    
    //----------------WORLDS----------------//
    public static string AncestralLodge = "Ancestral Lodge";
    public static string GoldenWingsOrder = "Golden Wings Order";
    public static string ArmyofDarkness = "Army of Darkness";
    public static string ProgenyBeyondTime = "Progeny Beyond Time";
    public static string Courtofthe7Vessels = "Court of the 7 Vessels";
    public static string LostEden = "Lost Eden";

    //----------------TEXT TO ICON----------------//
    public static readonly Dictionary<string,string> TextToIcon = new ()
    {
        {"Shards","Shard"},
        {"Shard","Shard"},
        {"Golden Wings Order","Golden Wings Order"},
        {"Ancestral Lodge","Ancestral Lodge"},
        {"Army of Darkness","Army of Darkness"},
        {"Progeny Beyond Time","Progeny Beyond Time"},
        {"Court of the 7 Vessels","Court of the 7 Vessels"},
        {"Lost Eden","Lost Eden"},
        {"[C]", "Draw"},
    };

    public static string EthAddress = "W3Address";
    public static string PrivateKey = "W3PrivKey";
    public static string W3Token= "W3Token";
    public static string AlturaToken= "AlturaToken";
    
    public const string WEB_STORE_BODY =
        "You will be redirected to the web store page. Proceed?\n You will need your player ID:";
    public const string WEB_STORE_URL = "https://shop.playdawnshard.com/";
    public const string WEB_STOORE_TITLE = "go to web store";
}
