using UnityEngine;
using static Define;
using System;
using System.Collections.Generic;


[Serializable]
public class CardDataLoader : ILoader<int, Card>
{
    public List<Card> Cards = new();

    public Dictionary<int, Card> MakeDic()
    {
        Dictionary<int, Card> dic = new Dictionary<int, Card>();

        foreach (Card data in Cards) {
            if (string.IsNullOrEmpty(data.templateId)) {
                int lastKey = dic.Count - 1;
                if (dic.ContainsKey(lastKey)) {
                    dic[lastKey].effects.Add(data.effects[0]);
                }
                continue;
            }
                
            dic.Add(dic.Count, data);
        }
        
        Debug.Log("CardDataLoader: " + dic.Count);
        return dic;
    }

    public bool Validate()
    {
        return true;
    }
}