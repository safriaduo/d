using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

[Serializable]
public class UnityDictionary<T> 
{
    public List<KeyValuePair<T>> keyValuePairs;

    public T GetItem(string id) 
    {
        var objs = keyValuePairs.Find(k => k.id == id);

        if (objs == null)
            return default(T);
        
        return objs.item;
    }

    public List<T> GetAllItems(string id)
    {
        var objs = keyValuePairs.FindAll(k => k.id == id);

        List<T> items = new List<T>();
        foreach (var obj in objs)
        {
            items.Add(obj.item);
        }

        return items;
    }
}

[Serializable]
public class KeyValuePair<T>
{
    public string id;
    public T item;
}
