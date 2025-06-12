using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIView : MonoBehaviour
{
    public Transform collectionParent;
    public ShopItemView itemPrefab;

    private List<ShopItemView> spawnedItems = new List<ShopItemView>();

    public void SpawnShopItems(List<ShopItemModel> items, Action<ShopItemModel> onItemSelected)
    {
        foreach (var item in items)
        {
            var itemUI = Instantiate(itemPrefab, collectionParent);
            itemUI.Initialize(item, onItemSelected, item.ResourceSpent==Constants.Tethras);
            spawnedItems.Add(itemUI);
        }
    }

    public void DestroySpawnedItems()
    {
        foreach (var item in spawnedItems)
        {
            Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }

}