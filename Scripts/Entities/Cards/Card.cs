using System.Collections.Generic;
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
        public string Character { get; private set; }
        public CardType Type { get; private set; }
        public int Ap { get; private set; }
        public string Description { get; private set; }
        public bool IsTargetAll { get; private set; }
        public float Value { get; private set; }
        public Sprite Sprite { get; private set; }
        public List<Effect> Effects { get; private set; }
    
        public Card (int id, string templateId, string character, CardType type, int ap, string description, bool isTargetAll, float value, Sprite sprite, List<Effect> effects) {
            this.ID = id;
            this.TemplateId = templateId;
            this.Character = character;
            this.Type = type;
            this.Ap = ap;
            this.Description = description;
            this.IsTargetAll = isTargetAll;
            this.Value = value;
            this.Sprite = sprite;
            this.Effects = effects;
        }
    }
}