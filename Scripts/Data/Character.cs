using UnityEngine;
using UnityEngine.Serialization;

public enum CharacterType {
    Tanker,
    Dealer,
    Supporter
}
[System.Serializable]
public class Character {
    public string templateId;
    public int hp;
    public int mp;
    public int atk;
    public int exp;
    public int agi;
    public int startAP;
    public CharacterType type;
}
