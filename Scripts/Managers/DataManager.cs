using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using System.Linq;
using Microsoft.CodeAnalysis;

public interface ILoader<Key, Item>
{
    Dictionary<Key, Item> MakeDic();
    bool Validate();
}
public class DataManager
{
    public Dictionary<int, Card> Cards { get; private set; }
    
    public IEnumerator LoadData() {
        yield return LoadJson<CardDataLoader, int, Card>("CardData", (loader) => { Cards = loader.MakeDic(); });
    }
    
    public bool Loaded()
    {
        if (Cards == null) return false;
        return true;
    }

    private IEnumerator LoadJson<Loader, Key, Value>(string key, Action<Loader> callback) where Loader : ILoader<Key, Value>
    {
        bool isDone = false;
        Managers.Resource.LoadAsync<TextAsset>(key, (textAsset) =>
        {
            Loader loader = JsonUtility.FromJson<Loader>(textAsset.text);
            callback?.Invoke(loader);
            isDone = true;
        });

        while (!isDone)
        {
            yield return null;
        }
    }
}
