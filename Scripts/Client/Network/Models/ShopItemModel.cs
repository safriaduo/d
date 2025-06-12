using System.Collections;
using System.Collections.Generic;
using Dawnshard.Database;
using Newtonsoft.Json;
using UnityEngine;

public class ShopItemModel
{
    /// <summary>
    /// The current resource identifier.
    /// </summary>
    public static int CurrentId { get; set; }
    /// <summary>
    /// Name of the shop item
    /// </summary>
    [JsonProperty("Name")]
    public string Name { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("Description")]
    public string Description{ get; set; }

    /// <summary>
    /// Name of the resource rewarded
    /// </summary>
    [JsonProperty("ResourceObtained")]
    public string ResourceObtained{ get; set; }
    
    /// <summary>
    /// Amount of resoure spent
    /// </summary>
    [JsonProperty("Cost")]
    public float Cost{ get; set; }
    

    /// <summary>
    /// The resource
    /// </summary>
    [JsonProperty("ResourceSpent")]
    public string ResourceSpent{ get; set; }

    /// <summary>
    /// Amount of the resource obtained
    /// </summary>
    [JsonProperty("AmountGained")]
    public int Amount{ get; set; }
    
    [JsonProperty("SKU")]
    public string SKU{ get; set; }


    /// <summary>
    /// Constructor.
    /// </summary>
    public ShopItemModel(int cost, string resourceSpent, string name, string description, string resourceObtained, string rewardIconId, int amount)
    {
        this.Cost = cost;
        this.ResourceSpent = resourceSpent;
        this.Name = name;
        this.Description = description;
        this.ResourceObtained = resourceObtained; 
        this.Amount = amount;
    }
    
    public ShopItemModel() { }

    public bool CanBePurchased()
    {
        return GameController.Instance.Wallet.ContainsKey(ResourceSpent) && GameController.Instance.Wallet[ResourceSpent] >= Cost;
    }

    public Sprite LoadIcon() => AssetDatabase.Instance.GetRewardRecord(ResourceObtained).rewardIcon;
}

