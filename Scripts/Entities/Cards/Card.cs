using System.Collections.Generic;
using Global.Managers;
using UnityEngine;

namespace Entities.Cards {
    public enum CardType {
        Attack,
        Skill
    }

    public enum ValueType {
        None,
        Percent,
        Value
    }

    [System.Serializable]
    public class Card {
        public int ID { get; private set; }
        public string TemplateId { get; private set; }
        public CharacterName Character { get; private set; } // 나중에 Character 클래스로 변경
        public int Ap { get; private set; }
        public string Description { get; private set; }
        public bool IsUnique { get; private set; }
        public bool IsTargetMe { get; private set; }
        public int Range { get; private set; }
        public Sprite Sprite { get; private set; }
        public List<CardDataLoader.EffectData> Effects { get; private set; }
    
        public Card (int id, string templateId, CharacterName character, int ap, string description, bool isUnique, bool isTargetMe,
            float value,int range, Sprite sprite, List<CardDataLoader.EffectData> effects) {
            ID = id;
            TemplateId = templateId;
            Character = character;
            Ap = ap;
            Description = description;
            IsUnique = isUnique;
            IsTargetMe = isTargetMe;
            Range = range;
            Sprite = sprite;
            Effects = effects;
        }
        
        public bool IsTargetAll() => Range == -1;
    }
}