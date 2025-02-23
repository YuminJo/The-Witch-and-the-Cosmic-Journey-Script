using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Entities.Cards;
using Global.Managers;

public interface ILoader<TKey, TItem>
{
    Dictionary<TKey, TItem> MakeDic();
    bool Validate();
}
public class DataManager {
    public Dictionary<int, Card> Cards { get; private set; }
    
    public IEnumerator LoadData() {
        yield return LoadJson<CardDataLoader, int, Card>("CardData", (loader) => { Cards = loader.MakeDic(); });
    }
    
    public bool Loaded() {
        if (Cards == null) return false;
        return true;
    }

    private IEnumerator LoadJson<Loader, Key, Value>(string key, Action<Loader> callback) where Loader : ILoader<Key, Value>
    {
        bool isDone = false;
        var resourceManager = ServiceLocator.Get<IResourceManager>();
        resourceManager.LoadAsync<TextAsset>(key).ContinueWith(textAsset =>
        {
            Loader loader = JsonUtility.FromJson<Loader>(textAsset.text);
            callback?.Invoke(loader);
            isDone = true;
        }).Forget();

        while (!isDone)
        {
            yield return null;
        }
    }
}
