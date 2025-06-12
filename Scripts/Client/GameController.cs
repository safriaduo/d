using Dawnshard.Database;
using Dawnshard.Network;
using Google.Protobuf;
using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WriteStorageObject = Nakama.WriteStorageObject;

/// <summary>
/// The game controller injects all the dependencies in the components of the game
/// It's similar to the Main() function 
/// </summary>
public class GameController : MonoBehaviour
{
    private const string SESSION_PREF = "nakama.session";
    private const string REFRESH_SESSION_PREF = "nakama.sessionRefresh";
    private const string DECKS_COLLECTION = "decks";
    private const string USER_DATA_COLLECTION = "user_data";
    private const string LEADERBOARD_ID = "Ascendant";
    private const string GAME_MODE_METADATA = "game_mode";
    private const string PLAYER_DECK_METADATA = "player_deck";
    private const string AI_DECK_METADATA = "ai_deck";
    private const string TUTORIAL_METADATA = "completed_tutorials";
    private const string START_TUTORIAL_RPC = "tutorial_match";
    private const string ADD_ALTURA_CONNECTION = "altura_guard_connect";
    private const string REVOKE_ALTURA_CONNECTION = "altura_guard_revoke";
    private const string AI_MATCH_RPC = "ai_match";
    private const string FRIENDLY_MATCH_RPC = "friendly_match";
    private const string SAVE_DECK_RPC = "save_deck";

    public List<DeckModel> Decks { get; private set; }

    public UserMetadataModel UserMetadata { get; private set; }

    public Dictionary<string, int> Wallet { get; private set; }

    public IClient Client { get; private set; }
    public ISession Session { get; private set; }
    public ISocket Socket { get; private set; }

    public IGameInteractor GameInteractor { get; private set; }

    public INetworkGateway NetworkGateway { get; private set; }

    public IEventDispatcher EventDispatcher { get; private set; }

    public string Address => PlayerPrefs.GetString(Constants.EthAddress);

    public string PrivateKey => PlayerPrefs.GetString(Constants.PrivateKey);

    public string W3Token => PlayerPrefs.GetString(Constants.W3Token);

    public string AlturaToken => PlayerPrefs.GetString(Constants.AlturaToken);

    public bool IsInRankedMatch { get; private set; }




    /// <summary>
    /// Connection to the server
    /// </summary>
    public NakamaConnection connection;

    public static GameController Instance { get; private set; }
    public static string Username { get; private set; } = "";

    private Coroutine rankedMatchTimeout;
    private IMatchmakerTicket matchmakingTicket;
    private string matchId;
    private IMatch currentMatch;
    private string deckId;

    /// <summary>
    /// The id of the local player
    /// </summary>
    public string LocalPlayerId => Session.UserId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CardDatabase.Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnApplicationQuit()
    {
#pragma warning disable CS4014 
        LeaveMatch();
#pragma warning restore CS4014 
    }


    /// <summary>
    /// Connects to the server 
    /// </summary>
    public async Task Connect()
    {
        // Connect to the Nakama server.
        Client = new Client(connection.Scheme, connection.Host, connection.Port, connection.ServerKey, UnityWebRequestAdapter.Instance);

        // Attempt to restore an existing user session.
        var authToken = PlayerPrefs.GetString(SESSION_PREF);
        var refreshToken = PlayerPrefs.GetString(REFRESH_SESSION_PREF);

        Session = null;

        if (!string.IsNullOrEmpty(authToken) && !string.IsNullOrEmpty(refreshToken))
        {
            Session = Nakama.Session.Restore(authToken, refreshToken);

            if (Session.IsExpired)
            {
                if (!Session.IsRefreshExpired)
                {
                    Session = await Client.SessionRefreshAsync(Session);
                }
                else
                {
                    Session = null;
                }
            }

            if (Session != null)
            {
                await OnSessionCreated();
            }
        }

        if (Session == null)
        {
            if (!string.IsNullOrEmpty(Address))
            {
                if (!string.IsNullOrEmpty(W3Token))
                {
                    await WalletLogin(Address, W3Token);
                }
                else if (!string.IsNullOrEmpty(AlturaToken))
                {
                    await WalletLogin(Address, authToken);
                }
            }
        }
    }


    /// <summary>
    /// Create the socket
    /// </summary>
    private async Task OnSessionCreated()
    {
        if (Session == null)
        {
            return;
        }

        // Open a new Socket for realtime communication.
        Socket = Client.NewSocket(true);

        await Socket.ConnectAsync(Session, true);

        try
        {
            var account = await Client.GetAccountAsync(Session);

            if (string.IsNullOrEmpty(account.CustomId))
            {
                if (!string.IsNullOrEmpty(Address))
                {
                    await LinkAddress();
                }
            }

            Username = account.User.Username;
            await RefreshAssets();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        SaveSession();

        if (UserMetadata != null && UserMetadata.CompletedTutorials <= 1)
        {
            try
            {
                await StartTutorialMatch(1);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
        {
            SceneManager.LoadScene(Constants.MenuScene);
        }
    }

    public async Task LinkAddress()
    {
        await Client.LinkCustomAsync(Session, Address);
    }

    public async Task WalletLogin(string address, string JWTToken)
    {
        Dictionary<string, string> accountDict = new Dictionary<string, string>()
        {
            {"JWT_TOKEN",JWTToken},
        };
        Session = await Client.AuthenticateCustomAsync(address, create: false, vars: accountDict);
        SaveSession();

        await OnSessionCreated();
    }

    private void SaveSession()
    {
        // Store the auth token that comes back so that we can restore the session later if necessary.
        PlayerPrefs.SetString(SESSION_PREF, Session.AuthToken);
        PlayerPrefs.SetString(REFRESH_SESSION_PREF, Session.RefreshToken);
    }

    /// <summary>
    /// Logout from current session
    /// </summary>
    public async Task Logout()
    {
        if (Session != null)
        {
            Web3AuthAPI.Instance.Logout();

            if (!string.IsNullOrEmpty(AlturaToken))
            {
                try
                {
                    await RevokeAlturaConnection();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            try
            {
                await Client.SessionLogoutAsync(Session);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            PlayerPrefs.DeleteAll();
        }
    }

    public async Task<List<IApiNotification>> ReadNotification()
    {
        var result = await Client.ListNotificationsAsync(Session, 10);
        return result.Notifications.ToList();
    }

    public async Task DeleteNotification(List<string> notificationIds)
    {
        await Client.DeleteNotificationsAsync(Session, notificationIds);
    }
    public void ClearURLWithoutRefresh()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval("window.history.replaceState({}, document.title, window.location.origin + window.location.pathname);");
        Debug.Log("URL aggiornato senza query params, senza ricaricare la pagina!");
#else
        Debug.Log("ClearURLWithoutRefresh chiamato, ma non in WebGL.");
#endif
    }

    /// <summary>
    /// Login or register with a mail
    /// </summary>
    public async Task DeviceLogin()
    {
        var deviceId = Guid.NewGuid().ToString();


        Session = await Client.AuthenticateDeviceAsync(deviceId);

        SaveSession();

        await OnSessionCreated();
    }

    public async Task UpdateUsername(string username)
    {
        await Client.UpdateAccountAsync(Session, username);
        Username = username;
        Session = await Client.SessionRefreshAsync(Session);
    }

    public async Task<bool> IAPBuy(string receipt, string store, bool isSubscription = false)
    {
        if (isSubscription)
        {
            if (store == "GooglePlay")
            {
                var response = await Client.ValidateSubscriptionGoogleAsync(Session, receipt);
                return response.ValidatedSubscription.Active;
            }
            if (store == "AppleAppStore")
            {
                var response = await Client.ValidateSubscriptionAppleAsync(Session, receipt);
                return response.ValidatedSubscription.Active;
            }
            return false;
        }

        IApiValidatePurchaseResponse purchaseResponse;

        if (store == "GooglePlay")
        {
            purchaseResponse = await Client.ValidatePurchaseGoogleAsync(Session, receipt);
        }
        else if (store == "AppleAppStore")
        {
            purchaseResponse = await Client.ValidatePurchaseAppleAsync(Session, receipt);
        }
        else
        {
            Debug.LogError($"Store not supported");
            return false;
        }
        foreach (var responseValidatedPurchase in purchaseResponse.ValidatedPurchases)
        {
            if (responseValidatedPurchase.SeenBefore == false)
                return true;
        }
        return false;
    }

    #region Match

    private class AIMatchPayload
    {
        [JsonProperty("isRanked")]
        public bool IsRanked;
    }

    /// <summary>
    /// Starts a match against the AI. You must select both the deck you want to use and the deck the AI will use
    /// </summary>
    public async Task StartAIMatch(string playerDeckId, bool isRanked)
    {
        AIMatchPayload payload = new()
        {
            IsRanked = isRanked
        };

        var matchCreated = await Socket.RpcAsync(AI_MATCH_RPC, JsonConvert.SerializeObject(payload));

        matchId = matchCreated.Payload;

        currentMatch = await Socket.JoinMatchAsync(matchId, new Dictionary<string, string>()
        {
            { PLAYER_DECK_METADATA, playerDeckId},
        });

        SceneManager.LoadScene(Constants.GameScene);

        StartMatch(currentMatch);
    }

    /// <summary>
    /// Starts the tutorial
    /// </summary>
    public async Task StartTutorialMatch(int tutorialNumber, string playerDeckId = "")
    {
        var request = new CreateTutorialMatchRequest()
        {
            TutorialStep = tutorialNumber,
        };

        var matchCreated = await Socket.RpcAsync(START_TUTORIAL_RPC, JsonConvert.SerializeObject(request));

        matchId = matchCreated.Payload;

        if (tutorialNumber == 1)
        {
            await TutorialStorageAPI.SaveTutorialStorage(false, false, 0, false, false);
            currentMatch = await Socket.JoinMatchAsync(matchId);

            SceneManager.LoadScene(Constants.GameScene);
            StartMatch(currentMatch, true);
        }
        else
        {
            currentMatch = await Socket.JoinMatchAsync(matchId, new Dictionary<string, string>
            {
                { PLAYER_DECK_METADATA, playerDeckId},
            });
            SceneManager.LoadScene(Constants.GameScene);
            StartMatch(currentMatch);
        }
    }

    /// <summary>
    /// Send a friendly match request to a friend and join the created match
    /// </summary>
    public async Task<string> SendFriendlyMatchRequest(string deckId, string friendId)
    {
        var request = new FriendlyMatchRequest
        {
            FriendID = friendId,
        };

        var matchCreated = await Socket.RpcAsync(FRIENDLY_MATCH_RPC, JsonConvert.SerializeObject(request));

        matchId = matchCreated.Payload;

        currentMatch = await Socket.JoinMatchAsync(matchId, new Dictionary<string, string>
        {
            { PLAYER_DECK_METADATA, deckId },
        });

        SceneManager.LoadScene(Constants.GameScene);

        StartMatch(currentMatch);

        return matchId;
    }

    /// <summary>
    /// Starts the matchmaking in a specified game mode
    /// </summary>
    public async Task FindMatch(string deckId, string gameMode = Constants.RankedMode)
    {
        this.deckId = deckId;

        var _matchmakingTicket = await Socket.AddMatchmakerAsync(
                                        query: "*",//"+label.open:1",
                                        minCount: 2,
                                        maxCount: 2,
                                        stringProperties: new Dictionary<string, string>()
                                        {
                                            { GAME_MODE_METADATA, gameMode},
                                        },
                                        numericProperties: null);

        if (gameMode == Constants.RankedMode)
        {
            IEnumerator timeout()
            {
                yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(30f, 60f));

                RankedMatchTimeout();
            }

            rankedMatchTimeout = StartCoroutine(timeout());
        }

        matchmakingTicket = _matchmakingTicket;
        Socket.ReceivedMatchmakerMatched += EnqueueMatchSearch;
    }

    private async void RankedMatchTimeout()
    {
        if (matchmakingTicket != null)
        {
            await CancelMatch();

            await StartAIMatch(deckId, true);
        }
    }

    public async Task ConcedeMatch()
    {
        if (string.IsNullOrEmpty(matchId))
            return;

        await NetworkGateway.SendConcedeRequest(new ConcedeRequest());
    }

    public async Task LeaveMatch()
    {
        if (string.IsNullOrEmpty(matchId))
            return;

        await Socket.LeaveMatchAsync(matchId);
    }

    public async Task CancelMatch()
    {
        Socket.ReceivedMatchmakerMatched -= EnqueueMatchSearch;

        await Socket.RemoveMatchmakerAsync(matchmakingTicket);
    }

    public async Task AcceptFriendlyMatch(string matchId, string deckId)
    {
        this.matchId = matchId;

        currentMatch = await Socket.JoinMatchAsync(matchId, new Dictionary<string, string>
        {
            { PLAYER_DECK_METADATA, deckId },
        });

        SceneManager.LoadScene(Constants.GameScene);

        StartMatch(currentMatch);
    }

    private void EnqueueMatchSearch(IMatchmakerMatched m)
    {
        OnMatchmakerMatchedAsync(m);
    }

    private async void OnMatchmakerMatchedAsync(IMatchmakerMatched matched)
    {
        if (rankedMatchTimeout != null)
        {
            StopCoroutine(rankedMatchTimeout);
            rankedMatchTimeout = null;
        }

        matchId = matched.MatchId;

        Socket.ReceivedMatchmakerMatched -= EnqueueMatchSearch;

        currentMatch = await Socket.JoinMatchAsync(matched.MatchId, new Dictionary<string, string>() { { PLAYER_DECK_METADATA, deckId }, { GAME_MODE_METADATA, matched.Self.StringProperties[GAME_MODE_METADATA] } });

        SceneManager.LoadScene(Constants.GameScene);
        IsInRankedMatch = matched.Self.StringProperties[GAME_MODE_METADATA] == Constants.RankedMode;

        StartMatch(currentMatch);
    }

    /// <summary>
    /// Starts a match 
    /// </summary>
    public void StartMatch(IMatch match, bool isTutorial = false)
    {
        var eventBusManager = new EventBusManager();
        EventDispatcher = new EventDispatcher(eventBusManager);
        NetworkGateway = new NetworkGateway(Socket, match, EventDispatcher);

        if (isTutorial)
        {
            GameInteractor = new TutorialGameInteractor(NetworkGateway);
        }
        else
        {
            GameInteractor = new GameInteractor(NetworkGateway);
        }

        eventBusManager.GameInteractor = GameInteractor;

        Socket.ReceivedMatchState += NetworkGateway.OnReceivedMatchState;
    }

    /// <summary>
    /// Check if the user is still on the match
    /// </summary>
    public async Task<bool> IsUserStillInMatchAsync()
    {
        try
        {
            var matches = await Client.ListMatchesAsync(Session, 0, 2, 1, currentMatch.Authoritative, currentMatch.Label, "");
            return matches.Matches.Any();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error checking match presence: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// End the match 
    /// </summary>
    public void EndMatch()
    {
        Socket.ReceivedMatchState -= NetworkGateway.OnReceivedMatchState;

        EventDispatcher = null;
        NetworkGateway = null;
        GameInteractor = null;

        currentMatch = null;

        SceneManager.LoadScene(Constants.MenuScene);
        IsInRankedMatch = false;
    }
    #endregion

    /// <summary>
    /// Loads all the assets of the player from the server
    /// </summary>
    public async Task RefreshAssets()
    {
        UserMetadata = await LoadUserMetadata();
        Wallet = await LoadWalletData();
        await BattlePassAPI.LoadBattlePass();
        await CardSetAPI.LoadSets(async ()=>
        {
            Decks = await LoadDecks();
        });
        DailyQuestAPI.ReadQuestsFromJson();
        ForgeAPI.LoadForgeCosts();
        await RankAPI.LoadPlayerRank();
        ShopAPI.LoadShopItemDatabase();
        await TutorialStorageAPI.ReadTutorialStorage();
        await DailyQuestAPI.GetUserDailyQuests();
        await TournamentAPI.SearchWeekendTournament();
    }

    public async Task RefreshWallet()
    {
        Wallet = await LoadWalletData();
    }

    public async Task RefreshSession()
    {
        Session = await Client.SessionRefreshAsync(Session);
    }


    /// <summary>
    /// Loads all the assets of the player from the server
    /// </summary>
    /* public async Task<UserDataModel> LoadUserData()
     {
         var storageObjectRead = new StorageObjectId()
         {
             Collection = USER_DATA_COLLECTION,
             Key = USER_DATA_COLLECTION,
             UserId = Session.UserId,
         };

         var storageList = await Client.ReadStorageObjectsAsync(Session, new StorageObjectId[1] { storageObjectRead });
         var userDataObject = storageList.Objects.Single();

         if (userDataObject != null)
         {
             var userDataDTO = UserDataDTO.Parser.ParseJson(userDataObject.Value);
             return new UserDataModel(userDataDTO);
         }

        return null;
    }*/

    public async Task<UserMetadataModel> LoadUserMetadata()
    {
        var account = await Client.GetAccountAsync(Session);
        var user = account.User;
        var userMetadata = JsonConvert.DeserializeObject<UserMetadataModel>(user.Metadata) ?? new UserMetadataModel();
        userMetadata.UnmarshalPlayerRank();
        return userMetadata;
    }


    /// <summary>
    /// Load the player decks from the storage
    /// </summary>
    public async Task<List<DeckModel>> LoadDecks()
    {
        var decks = new List<DeckModel>();

        var setCollection = await Client.ListUsersStorageObjectsAsync(Session, DECKS_COLLECTION, Session.UserId, 20);

        foreach (var obj in setCollection.Objects)
        {
            var deck = JsonConvert.DeserializeObject<DeckModel>(obj.Value);
            var incomplete = false;
            foreach (var setId in deck.CardSetIds)
            {
                if(!CardSetAPI.CardSets.Exists(cardSet=>cardSet.ItemId==setId) && CardSetAPI.PlayerSets.Count>0)
                    incomplete = true;
            }
            if(!incomplete) 
                decks.Add(deck);
            else
            {
                await DeleteDeck(deck);
            }
        }

        return decks;
    }

    /// <summary>
    /// loads the user wallet data
    /// </summary>
    private async Task<Dictionary<string, int>> LoadWalletData()
    {
        var account = await Client.GetAccountAsync(Session);
        Dictionary<string, int> currency = account.Wallet.FromJson<Dictionary<string, int>>();
        return currency;
    }

    /// <summary>
    /// Returns the top 10 records on the leaderboard
    /// </summary>
    public async Task<IApiLeaderboardRecordList> GetTopLeaderboardAsync(int recordNumber = 10, string leaderboardId = LEADERBOARD_ID)
    {
        return await Client.ListLeaderboardRecordsAsync(Session, leaderboardId, null, null, 10, null);
    }

    /// <summary>
    /// Returns the top 10 records on the leaderboard around the owner
    /// </summary>
    public async Task<IApiLeaderboardRecordList> GetOwnerLeaderboardAsync(int recordNumber = 10, string leaderboardId = LEADERBOARD_ID)
    {
        return await Client.ListLeaderboardRecordsAroundOwnerAsync(Session, leaderboardId, Session.UserId, null, recordNumber, null);
    }

    /// <summary>
    /// Get all friends even pending and blocked
    /// </summary>
    public async Task<IApiFriendList> GetFriends()
    {
        return await Client.ListFriendsAsync(Session, null, 100);
    }

    /// <summary>
    /// Adds a friend from the list
    /// </summary>
    public async void AddFriend(string username)
    {
        await Client.AddFriendsAsync(Session, new List<string>(), new List<string> { username });
    }

    private class SaveDeckRequest
    {
        public string name;
        public int[] cardSetIds;
    }

    /// <summary>
    /// Loads all the assets of the player from the server
    /// </summary>
    public async Task SaveDeck(DeckModel deck)
    {
        var request = new SaveDeckRequest()
        {
            name = deck.Name,
            cardSetIds = deck.CardSetIds.ToArray(),
        };
        Nakama.WriteStorageObject[] storageObjects = new WriteStorageObject[1];
        Nakama.WriteStorageObject storageObject = new WriteStorageObject();

        storageObject.Value = JsonConvert.SerializeObject(request);
        storageObject.Key = deck.Name;
        storageObject.Collection = DECKS_COLLECTION;
        storageObjects[0] = storageObject;
        try
        {
            //await Socket.RpcAsync(SAVE_DECK_RPC, JsonConvert.SerializeObject(request));
            await Client.WriteStorageObjectsAsync(Session, storageObjects);
            Decks.Add(deck);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task DeleteDeck(DeckModel deck)
    {
        try
        {
            await Client.DeleteStorageObjectsAsync(Session, new[]
            {
                new StorageObjectId
                {
                    Collection = DECKS_COLLECTION,
                    Key = deck.Name,
                }
            });
            Decks.Remove(deck);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task SavePlayerStepCompleted(DeckModel deck)
    {
        var request = new SaveDeckRequest()
        {
            name = deck.Name,
            cardSetIds = deck.CardSetIds.ToArray(),
        };
        Nakama.WriteStorageObject[] storageObjects = new WriteStorageObject[1];
        Nakama.WriteStorageObject storageObject = new WriteStorageObject();

        storageObject.Value = JsonConvert.SerializeObject(request);
        storageObject.Key = deck.Name;
        storageObject.Collection = DECKS_COLLECTION;
        storageObjects[0] = storageObject;
        try
        {
            //await Socket.RpcAsync(SAVE_DECK_RPC, JsonConvert.SerializeObject(request));
            await Client.WriteStorageObjectsAsync(Session, storageObjects);
            Decks.Add(deck);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }



    private class CreateTutorialMatchRequest
    {
        [JsonProperty("tutorialStep")]
        public int TutorialStep { get; set; }
    }

    public async Task<AddressResponse> AddAlturaConnection(string alturaGuardCode)
    {
        var request = new AddAlturaConnectionRequest()
        {
            Code = alturaGuardCode,
        };

        var alturaType = await Client.RpcAsync(connection.HttpKey, ADD_ALTURA_CONNECTION, JsonConvert.SerializeObject(request), new RetryConfiguration(0, 0));
        return JsonConvert.DeserializeObject<AddressResponse>(alturaType.Payload);

    }

    public async Task RevokeAlturaConnection()
    {
        var request = new RevokeAlturaConnectionRequest()
        {
            Token = AlturaToken,
        };

        await Client.RpcAsync(connection.HttpKey, REVOKE_ALTURA_CONNECTION, JsonConvert.SerializeObject(request), new RetryConfiguration(0, 0));
    }

    public class AddressResponse
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }

    private class AddAlturaConnectionRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    private class RevokeAlturaConnectionRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    private class FriendlyMatchRequest
    {
        [JsonProperty("friendId")]
        public string FriendID { get; set; }
    }

    public async Task WriteStorageObject(IApiWriteStorageObject[] storageObjects)
    {
        try
        {
            await Client.WriteStorageObjectsAsync(Session, storageObjects);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public async Task<IApiStorageObjects> ReadStorageObject(IApiReadStorageObjectId[] storageObjects)
    {
        try
        {
            return await Client.ReadStorageObjectsAsync(Session, storageObjects);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return null;
    }

    public async Task DeleteUser()
    {
        await Client.DeleteAccountAsync(Session);
    }
}
