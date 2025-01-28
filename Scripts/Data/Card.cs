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
    public int id;
    public string templateId;
    public CardType type;
    public int cost;
    public string description;
    public int range;
    public ValueType valueType;
    public float value;
    public Sprite sprite;
    public List<Effect> effects;
}

[System.Serializable]
public class Effect {
    public string type;
    public string value;
    public string stat;
    public int turn;
}