using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum CharacterType {
    Tanker,
    Dealer,
    Supporter
}

[System.Serializable]
public class Character {
    public string TemplateId { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Mp { get; private set; }
    public int MaxMp { get; private set; }
    public int Atk { get; private set; }
    public int Exp { get; private set; }
    public int Agi { get; private set; }
    public int StartAP { get; private set; }
    public CharacterType Type { get; private set; }
    
    private List<ItemBuff> _buffs = new();
    public IEnumerable<ItemBuff> Buffs => _buffs;

    private const int MIN_VALUE = 0;

    public Character(string templateId, int hp, int mp, int atk, int exp, int agi, int startAP, CharacterType type) {
        TemplateId = templateId;
        Hp = hp;
        MaxHp = hp; // 초기화
        Mp = mp;
        MaxMp = mp; // 초기화
        Atk = atk;
        Exp = exp;
        Agi = agi;
        StartAP = startAP;
        Type = type;
    }
    
    public void SetMaxHp(int maxHp) {
        MaxHp = maxHp;
    }
    
    public void SetMaxMp(int maxMp) {
        MaxMp = maxMp;
    }

    public void AddBuff(ItemBuff buff) {
        if (!_buffs.Contains(buff)) {
            _buffs.Add(buff);
        }
    }
    
    public void OnDamage(int damage) {
        Hp = Mathf.Max(Hp - damage, MIN_VALUE);
    }
    
    public void OnHeal(int heal) {
        Hp = Mathf.Min(Hp + heal, MaxHp);
    }
    
    public void OnUseMp(int useMp) {
        Mp = Mathf.Max(Mp - useMp, MIN_VALUE);
    }
    
    public void OnRecoverMp(int recoverMp) {
        Mp = Mathf.Min(Mp + recoverMp, MaxMp);
    }
}