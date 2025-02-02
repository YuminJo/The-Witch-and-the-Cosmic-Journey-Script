using System;
using System.Collections.Generic;
using UnityEngine;
public class BuffManager {
    private List<ItemBuff> _buffs = new List<ItemBuff>();
    public IEnumerable<ItemBuff> Buffs => _buffs;

    public void AddBuff(ItemBuff buff) {
        _buffs.Add(buff);
    }
}

[System.Serializable]
public class GameEntity {
    public string TemplateId { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Mp { get; private set; }
    public int MaxMp { get; private set; }
    public int Atk { get; private set; }
    public int Agi { get; private set; }
    public int StartAP { get; private set; }

    private const int MIN_VALUE = 0;
    private BuffManager _buffManager;
    public IEnumerable<ItemBuff> Buffs => _buffManager.Buffs;

    public GameEntity(string templateId, int hp, int mp, int atk, int agi, int startAP) {
        TemplateId = templateId;
        Hp = hp;
        MaxHp = hp;
        Mp = mp;
        MaxMp = mp;
        Atk = atk;
        Agi = agi;
        StartAP = startAP;
        _buffManager = new BuffManager();
    }

    public void AddBuff(ItemBuff buff) => _buffManager.AddBuff(buff);

    public void IncreaseHp(int amount) {
        Hp = Mathf.Min(Hp + amount, MaxHp);
    }

    public void IncreaseMp(int amount) {
        Mp = Mathf.Min(Mp + amount, MaxMp);
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