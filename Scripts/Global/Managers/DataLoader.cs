using UnityEngine;
using static Define;
using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Cards;
using R3;
using ValueType = Entities.Cards.ValueType;

[Serializable]
public class CardDataLoader : ILoader<int, Card> {
    public List<CardData> Cards = new();
    [Serializable]
    public class CardData {
        public int id;
        public string templateId;
        public string character;
        public CardType type;
        public int ap;
        public string description;
        public bool isTargetAll;
        public float value;
        public Sprite sprite;
        public List<EffectData> effects;
        
        //DATA SCHEMA
        //JSON은 변형을 가하면 안됨
        //얘내 DATA라고 부여고
        //get private set으로 접근 제한
        //ScriptableObject로 만들어서 Resources에 저장
        //Serialize Field로 접근 제한
        //생성자한테 파라미터로 몰아주고 초기화 하는것도 방법인데.. 불변형을 표현을 하기 위해서는 새로운 접근이 필요
        //get internal set 동일 어셈블리 내에서만 접근 가능 책임 구분
        public Card ToCard() {
            List<Effect> dtoToEffects = new();
            dtoToEffects.AddRange(this.effects.Select(effect => effect.ToEffect()));
            
            return new Card(id, templateId, character, type, ap, description, isTargetAll, value, sprite, dtoToEffects);
        }
    }
    
    [Serializable]
    public class EffectData {
        public EffectType type;
        public int range;
        public ValueType valueType;
        public int value;
        public string stat;
        public int turn;

        public Effect ToEffect() {
            return new Effect(type, range, valueType, value, stat, turn);
        }
    }

    public Dictionary<int, Card> MakeDic() {
        Dictionary<int, Card> dic = new();

        Cards.ToObservable()
            .Where(data => !string.IsNullOrEmpty(data.templateId) || dic.Count > 0)
            .Subscribe(data => {
                //TODO: 스프라이트 로드 추가
                if (string.IsNullOrEmpty(data.templateId)) {
                    int lastKey = dic.Count - 1;
                    if (dic.ContainsKey(lastKey)) {
                        dic[lastKey].Effects.Add(data.effects[0].ToEffect());
                    }
                }
                else { dic.Add(dic.Count, data.ToCard()); } });
        return dic;
    }

    public bool Validate() => true;
    
}