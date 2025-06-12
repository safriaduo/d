using Dawnshard.Network;
using MoreMountains.Feel;
using Nakama;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlturaNFT;
using AlturaNFT.Internal;
using Unity.VisualScripting;
using UnityEngine;

public static class CardSetAPI
{
    public static List<CardSetModel> CardSets { get; private set; }
    public static List<CardSetModel> BaseSets { get; private set; }
    public static List<CardSetModel> PlayerSets { get; private set; }
    
    private const string OPEN_PACK_RPC = "open_pack";
    private const string SETS_COLLECTION = "generated_sets";
#if TEST_ENV
    private const string NFT_COLLECTION = "0x476132743721e1b5083f79d0a29e836ee9fdf9d3";
#else
    private const string NFT_COLLECTION = "0xa169f45552a437f0b45e2d5e4804709a0e8e573e";
#endif

    private static bool setLoaded = false;
    
    private class OpenPackRequest
    {
        [JsonProperty("packId")]
        public string PackId { get; set; }
        [JsonProperty("params")]
        public string Params { get; set; }
    }
    
    private class OpenPackResponse
    {
        [JsonProperty("tokenId")]
        public int tokenId { get; set; }
    }


    /// <summary>
    /// Load the player sets from the storage
    /// </summary>
    public static async Task LoadSets(Action OnComplete = null)
    {
        CardSets = new List<CardSetModel>();
        BaseSets = new List<CardSetModel>();
        PlayerSets = new List<CardSetModel>();
        
        await LoadBaseSets("00000000-0000-0000-0000-000000000000"); //Default Sets

        LoadCardSetsFromUserNFTs(GameController.Instance.Address, OnComplete);
        
        CardSets.AddRange(BaseSets);
    }

    private static async Task LoadBaseSets(string userId)
    {
        var setCollections = await GameController.Instance.Client.ListUsersStorageObjectsAsync(GameController.Instance.Session, SETS_COLLECTION, userId, 100);
    
        foreach (var obj in setCollections.Objects)
        {
            CardSetModel cardSetModel = JsonConvert.DeserializeObject<CardSetModel>(obj.Value);
            BaseSets.Add(cardSetModel);
        }
    }
    
    private static void LoadCardSetsFromUserNFTs(string address, Action OnComplete = null, int page = 0)
    {
        GetUsersItems
            .Initialize(destroyAtEnd: false)
            .SetAddress(
                account_address: address
            )
            .filter( //optional
                collectionAddress: NFT_COLLECTION,
                perPage: "50",
                page: page.ToString()
            )
            .OnError(error =>
            {
                Debug.Log(error);
                setLoaded = true;
                OnComplete?.Invoke();
            })
            .OnComplete(
                result =>
                {
                    if(result.items == null)
                    {
                        setLoaded = true;
                        return;
                    }
                    foreach (var item in result.items)
                    {
                        PlayerSets.Add(
                            new CardSetModel(
                                item.tokenId,
                                item.name,
                                item.properties.First(property => property.name == Constants.WorldIdProperty).value,
                                item.properties.First(property => property.name == Constants.IncandescenseProperty)
                                    .value,
                                item.properties.ToList()
                                    .FindAll(property => property.name.Contains(Constants.CardProperty))
                                    .ConvertAll(property => property.value)
                            )
                        );
                    }
                    CardSets.AddRange(PlayerSets);
                    if(!result.pagination.hasNext)
                        OnComplete?.Invoke();
                    else{
                        LoadCardSetsFromUserNFTs(address, OnComplete, result.pagination.next);
                    }
                })
            .Run();
    }

    /// <summary>
    /// Opens a card pack and returns the set contained as a model. 
    /// No set refresh required.
    /// </summary>
    public static async Task<CardSetModel> OpenPack(string packId, string parameters)
    {
        var packRequest = new OpenPackRequest
        {
            PackId = packId,
            Params = parameters
        };
        try
        {
            var response = await GameController.Instance.Socket.RpcAsync(OPEN_PACK_RPC, JsonConvert.SerializeObject(packRequest));
            
            CardSetModel cardSetModel = JsonConvert.DeserializeObject<CardSetModel>(response.Payload);
            CardSets.Add(cardSetModel);

            return cardSetModel;
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return null;
    }

    public static void ChangeSetName(CardSetModel cardSetModel, string newName)
    {
        // UpdateProperty
        //     .Initialize(destroyAtEnd: true)
        //     .SetParameters(
        //         address: GameController.Instance.Address,
        //         token_id: cardSetModel.ItemId,
        //         property_name:"Health",
        //         property_value:"100"
        //     )
        //     .OnError(error => Debug.Log(error))
        //     .OnComplete(result => Debug.Log(result))
        //     .Run();
    } 
}
