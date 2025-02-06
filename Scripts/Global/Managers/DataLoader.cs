using UnityEngine;
using static Define;
using System;
using System.Collections.Generic;
using UniRx;
using System.Linq;

[Serializable]
public class CardDataLoader : ILoader<int, Card> {
    public List<CardDTO> Cards = new();
    [Serializable]
    public class CardDTO {
        public int id;
        public string templateId;
        public string character;
        public CardType type;
        public int cost;
        public string description;
        public bool isTargetAll;
        public ValueType valueType;
        public float value;
        public Sprite sprite;
        public List<Effect> effects;

        public Card ToCard() {
            return new Card(id, templateId, character, type, cost, description, isTargetAll, valueType, value, sprite, effects);
        }
    }

    public Dictionary<int, Card> MakeDic() {
        Dictionary<int, Card> dic = new();
        
        Cards.ToObservable()
            .Where(data => !string.IsNullOrEmpty(data.templateId) || dic.Count > 0)
            .Subscribe(data => {
                    if (string.IsNullOrEmpty(data.templateId)) {
                        int lastKey = dic.Count - 1;
                        if (dic.ContainsKey(lastKey)) { dic[lastKey].effects.Add(data.effects[0]); } }
                    else { dic.Add(dic.Count, data.ToCard()); }
                },
                () => Debug.Log($"CardDataLoader: {dic.Count}"));
        return dic;
    }

    public bool Validate() => true;
    
}