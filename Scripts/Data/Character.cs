using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BuffManager {
    private List<ItemBuff> _buffs = new();

    public IEnumerable<ItemBuff> Buffs => _buffs;

    public void AddBuff(ItemBuff buff) {
        if (!_buffs.Contains(buff)) {
            _buffs.Add(buff);
        }
    }

    public void RemoveBuff(ItemBuff buff) {
        _buffs.Remove(buff);
    }
}

public enum CharacterType {
    Tanker,
    Dealer,
    Supporter
}

[System.Serializable]
public class Character {
    public string TemplateId { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; } // Added
    public int Mp { get; private set; }
    public int MaxMp { get; private set; } // Added
    public int Atk { get; private set; }
    public int Exp { get; private set; }
    public int Agi { get; private set; }
    public int StartAP { get; private set; }
    public CharacterType Type { get; private set; }

    private const int MIN_VALUE = 0;
    private BuffManager _buffManager; 
    public IEnumerable<ItemBuff> Buffs => _buffManager.Buffs;
    public void AddBuff(ItemBuff buff) => _buffManager.AddBuff(buff);

    public Character(string templateId, int hp, int mp, int atk, int exp, int agi, int startAP, CharacterType type) {
        TemplateId = templateId;
        Hp = hp;
        MaxHp = hp; // Initialize MaxHp
        Mp = mp;
        MaxMp = mp; // Initialize MaxMp
        Atk = atk;
        Exp = exp;
        Agi = agi;
        StartAP = startAP;
        Type = type;
        _buffManager = new BuffManager(); // Initialize BuffManager
    }

    public void IncreaseHp(int amount) {
        Hp = Mathf.Min(Hp + amount, MaxHp); // Clamped to MaxHp
    }

    public void IncreaseMp(int amount) {
        Mp = Mathf.Min(Mp + amount, MaxMp); // Clamped to MaxMp
    }

    public void OnDamage(int damage) {
        Hp = Mathf.Max(Hp - damage, MIN_VALUE);
    }

    public void OnHeal(int heal) {
        Hp = Mathf.Min(Hp + heal, MaxHp); // Now uses MaxHp
    }

    public void OnUseMp(int useMp) {
        Mp = Mathf.Max(Mp - useMp, MIN_VALUE);
    }

    public void OnRecoverMp(int recoverMp) {
        Mp = Mathf.Min(Mp + recoverMp, MaxMp); // Now uses MaxMp
    }
}