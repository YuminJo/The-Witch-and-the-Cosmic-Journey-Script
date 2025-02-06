using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

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
    public int id { get; private set; }
    public string templateId { get; private set; }
    public string character { get; private set; }
    public CardType type { get; private set; }
    public int cost { get; private set; }
    public string description { get; private set; }
    public bool isTargetAll { get; private set; }
    public ValueType valueType { get; private set; }
    public float value { get; private set; }
    public Sprite sprite { get; private set; }
    public List<Effect> effects { get; private set; }
    
    public Card (int id, string templateId, string character, CardType type, int cost, string description, bool isTargetAll, ValueType valueType, float value, Sprite sprite, List<Effect> effects) {
        this.id = id;
        this.templateId = templateId;
        this.character = character;
        this.type = type;
        this.cost = cost;
        this.description = description;
        this.isTargetAll = isTargetAll;
        this.valueType = valueType;
        this.value = value;
        this.sprite = sprite;
        this.effects = effects;
    }
}

[System.Serializable]
public class Effect {
    public string type;
    public int range;
    public string value;
    public string stat;
    public int turn;
}