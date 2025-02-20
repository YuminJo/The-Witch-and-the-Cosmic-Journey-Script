using System;
using System.Collections.Generic;
using Entities.Cards;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using ValueType = Entities.Cards.ValueType;

namespace Global.Managers {
    [Serializable]
    public class CardDataLoader : ILoader<int, Card> {
        public List<CardData> Cards = new();
        [Serializable]
        public class CardData {
            public int id;
            public string templateId;
            public string character;
            public int ap;
            public string description;
            public bool isUnique;
            public bool isTargetMe;
            public float value;
            public int range;
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
                CharacterName name = Enum.TryParse(character, out CharacterName result) ? result : CharacterName.None;
                return new Card(id, templateId, name, ap, description, isUnique,isTargetMe, value,range, sprite, effects);
            }
        }
    
        [Serializable]
        public class EffectData {
            public string type;
            public string valueType;
            public int value;
            public int turn;
            
            public EffectType GetEffectType() {
                return Enum.TryParse(type, out EffectType result) ? result : EffectType.None;
            }

            public ValueType GetValueType() {
                return Enum.TryParse(valueType, true, out ValueType result) ? result : ValueType.None;
            }
        }

        public Dictionary<int, Card> MakeDic() {
            Dictionary<int, Card> dic = new();

            foreach (CardData data in Cards) {
                if (string.IsNullOrEmpty(data.templateId)) {
                    //TODO: 스프라이트 로드 추가
                    int lastKey = dic.Count - 1;
                    dic[lastKey].Effects.Add(data.effects[0]);
                } else { dic.Add(dic.Count, data.ToCard()); }
            }
            return dic;
        }

        public bool Validate() => true;
    
    }
}