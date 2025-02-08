using System;
using System.Collections.Generic;
using Entities.Cards;
using R3;
using UnityEngine;
public class BuffManager {
    private List<Effect> _buffs = new();
    public IEnumerable<Effect> Buffs => _buffs;

    public void AddBuff(Effect buff) {
        _buffs.Add(buff);
    }
}

[Serializable]
public class GameEntity {
    public string TemplateId { get; private set; }
    
    public ReactiveProperty<int> Hp { get; private set; }
    public int MaxHp { get; private set; }
    
    public ReactiveProperty<int> Shield { get; private set; } 
    public ReactiveProperty<int> Mp { get; private set; }
    public int MaxMp { get; private set; }
    
    public int Atk { get; private set; }
    public int StartAP { get; private set; }

    private const int MIN_VALUE = 0;
    private BuffManager _buffManager;
    public IEnumerable<Effect> Buffs => _buffManager.Buffs;

    public GameEntity(string templateId, int hp, int mp, int atk, int startAP) {
        TemplateId = templateId;
        Hp = new ReactiveProperty<int>(hp);
        MaxHp = hp;
        Mp = new ReactiveProperty<int>(mp);
        MaxMp = mp;
        Shield = new ReactiveProperty<int>(0);
        Atk = atk;
        StartAP = startAP;
        _buffManager = new BuffManager();
    }

    public void AddBuff(Effect buff) => _buffManager.AddBuff(buff);

    public void IncreaseHp(int amount) {
        Hp.Value = Mathf.Min(Hp.Value + amount, MaxHp);
    }

    public void IncreaseMp(int amount) {
        Mp.Value = Mathf.Min(Mp.Value + amount, MaxMp);
    }

    public void OnDamage(int damage) {
        Hp.Value = Mathf.Max(Hp.Value - damage, MIN_VALUE);
    }

    public void OnHeal(int heal) {
        Hp.Value = Mathf.Min(Hp.Value + heal, MaxHp);
    }
    
    public void AddShield(int shield) {
        Shield.Value += shield;
    }

    public void OnUseMp(int useMp) {
        Mp.Value = Mathf.Max(Mp.Value - useMp, MIN_VALUE);
    }

    public void OnRecoverMp(int recoverMp) {
        Mp.Value = Mathf.Min(Mp.Value + recoverMp, MaxMp);
    }
}