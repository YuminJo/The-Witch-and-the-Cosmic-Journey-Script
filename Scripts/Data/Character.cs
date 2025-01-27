using UnityEngine;
using UnityEngine.Serialization;

public enum CharacterType {
    Tanker,
    Dealer,
    Supporter
}

[System.Serializable]
public class Character {
    public string TemplateId { get; private set; }
    public int Hp { get; private set; }
    public int Mp { get; private set; }
    public int Atk { get; private set; }
    public int Exp { get; private set; }
    public int Agi { get; private set; }
    public int StartAP { get; private set; }
    public CharacterType Type { get; private set; }

    public Character(string templateId, int hp, int mp, int atk, int exp, int agi, int startAP, CharacterType type) {
        TemplateId = templateId;
        Hp = hp;
        Mp = mp;
        Atk = atk;
        Exp = exp;
        Agi = agi;
        StartAP = startAP;
        Type = type;
    }
}