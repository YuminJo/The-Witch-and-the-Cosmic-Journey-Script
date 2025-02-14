using System.Collections.Generic;
using Global.Managers;
using UnityEngine;

namespace Entities.Cards {
    public enum CardType {
        Attack,
        Skill
    }

    public enum ValueType {
        Percent,
        Value
    }

    [System.Serializable]
    public class Card {
        public int ID { get; private set; }
        public string TemplateId { get; private set; }
        public string Character { get; private set; } // 나중에 Character 클래스로 변경
        public CardType Type { get; private set; }
        public int Ap { get; private set; }
        public string Description { get; private set; }
        public bool IsTargetAll { get; private set; }
        public float Value { get; private set; }
        public int Range { get; private set; }
        public Sprite Sprite { get; private set; }
        public List<CardDataLoader.EffectData> Effects { get; private set; }
    
        public Card (int id, string templateId, string character, CardType type, int ap, string description, bool isTargetAll, float value,int range, Sprite sprite, List<CardDataLoader.EffectData> effects) {
            ID = id;
            TemplateId = templateId;
            Character = character;
            Type = type;
            Ap = ap;
            Description = description;
            IsTargetAll = isTargetAll;
            Value = value;
            Range = range;
            Sprite = sprite;
            Effects = effects;
        }
    }
}