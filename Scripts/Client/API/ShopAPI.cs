using System;
using Dawnshard.Network;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;


public static class ShopAPI
{
    public class ShopModel
    {
        [JsonProperty("Items")]
        public List<ShopItemModel> Items { get; set; }
    }
    
    private class BuyItemRequest
    {
        [JsonProperty("SKU")]
        public string SKU  { get; set; }
        
        [JsonProperty("Amount")]
        public int Amount  { get; set; }
    }
    
    private class ReceiptData
    {
        [JsonProperty("Payload")]
        public string Payload  { get; set; }
        [JsonProperty("Store")]
        public string Store { get; set; }
        [JsonProperty("TransactionID")]
        public string TransactionID { get; set; }
    }
    
    private const string BUY_ITEM_RPC = "buy_item";
    private const string VALIDATE_PURCHASE_RPC = "validate_purchase";
    private const string SHOP_PATH = "Shop/shop_items";

    [JsonProperty("Items")] public static ShopModel ItemList { get; private set; }

    public static void LoadShopItemDatabase()
    {
        var shopItems = Resources.Load<TextAsset>(SHOP_PATH);
        
        ItemList = JsonConvert.DeserializeObject<ShopModel>(shopItems.text);
    }
    
    public static async Task<bool> BuyShopItem(ShopItemModel shopItem)
    {
        var buyRequest = new BuyItemRequest()
        {
            SKU = shopItem.SKU,
            Amount = shopItem.Amount
        };
        try
        {
            var response = await GameController.Instance.Socket.RpcAsync(BUY_ITEM_RPC, JsonConvert.SerializeObject(buyRequest));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public static async Task<bool> ValidateRecipe(string receipt)
    {
        var receiptData = JsonConvert.DeserializeObject<ReceiptData>(receipt);
        
        try
        {
            var response = await GameController.Instance.Socket.RpcAsync(VALIDATE_PURCHASE_RPC, receipt);
            Debug.Log(response);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error during rpc: " + e);
            return false;
        }

    }
}

