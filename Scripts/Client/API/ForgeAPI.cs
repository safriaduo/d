using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawnshard.Network;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

public static class ForgeAPI
{
    private const string FORGE_RPC_CALL = "upgrade_set";
    private const string BURN_RPC_CALL = "burn_set";
    private const string FORGE_PATH = "Incandescense/incandescense";
    
    private static List<ForgeTransationModel> AllForgeCosts  { get; set; }

    public static async Task UpgradeSetRequest(CardSetModel model)
    {
        UpgradeCardSetRequest request = new UpgradeCardSetRequest();
        request.CardSetId = model.ItemId;
        request.Incandescense = Constants.IncandescenseStringToInt(model.IncandescenseLevel)+1;
        try
        {
            await GameController.Instance.Socket.RpcAsync(FORGE_RPC_CALL, JsonConvert.SerializeObject(request));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static async Task BurnSetRequest(CardSetModel model)
    {
        BurnCardSetRequest request = new BurnCardSetRequest();
        request.CardSetId = model.ItemId;
        request.Incandescense = Constants.IncandescenseStringToInt(model.IncandescenseLevel);
        try
        {
            await GameController.Instance.Socket.RpcAsync(BURN_RPC_CALL, JsonConvert.SerializeObject(request));
            var decksToDestroy=GameController.Instance.Decks.FindAll(deck => deck.CardSetIds.Contains(model.ItemId));
            foreach (var deck in decksToDestroy.ToList())
            {
                await GameController.Instance.DeleteDeck(deck);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public static void LoadForgeCosts()
    {
        var forgeFile = Resources.Load<TextAsset>(FORGE_PATH);
        
        IList jsonArray = JsonConvert.DeserializeObject<IList>(forgeFile.text);
        
        AllForgeCosts = new List<ForgeTransationModel>();

        foreach (var forgeCost in jsonArray)
        {
            var forgeCostModel = JsonConvert.DeserializeObject<ForgeTransationModel>(JsonConvert.SerializeObject(forgeCost));
            AllForgeCosts.Add(forgeCostModel);
        }
    }

    public static int GetUpgradeCost(CardSetModel model)
    {
         return AllForgeCosts.Find(forgeCosts=>
             forgeCosts.Incandescense == 
             Constants.IncandescenseIntToString(Constants.IncandescenseStringToInt(model.IncandescenseLevel)+1)).CostToUpgrade;
    }
    
    public static int GetBurnReward(CardSetModel model)
    {
        return AllForgeCosts.Find(forgeCosts=>
            forgeCosts.Incandescense == 
            Constants.IncandescenseIntToString(Constants.IncandescenseStringToInt(model.IncandescenseLevel))).RewardForBurn;
    }

    private class UpgradeCardSetRequest
    {
        [JsonProperty("cardSetId")]
        public int CardSetId  { get; set; }
        
        [JsonProperty("incandescense")]
        public int Incandescense  { get; set; }
    }

    private class BurnCardSetRequest
    {
        [JsonProperty("cardSetId")] public int CardSetId { get; set; }
        [JsonProperty("incandescense")] public int Incandescense { get; set; }
    }

    public class ForgeTransationModel
    {
        [JsonProperty("name")]
        public string Incandescense  { get; set; }
        [JsonProperty("costToUpgrade")]
        public int CostToUpgrade  { get; set; }
        [JsonProperty("rewardForBurn")]
        public int RewardForBurn  { get; set; }
    }
}
