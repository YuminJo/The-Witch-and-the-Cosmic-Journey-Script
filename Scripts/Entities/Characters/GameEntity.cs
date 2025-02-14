using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Entities.Cards;
using ObservableCollections;
using R3;
using UnityEngine;

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
    private List<Effect> _activeEffects = new();

    public GameEntity(string templateId, int hp, int mp, int atk, int startAP) {
        TemplateId = templateId;
        Hp = new ReactiveProperty<int>(hp);
        MaxHp = hp;
        Mp = new ReactiveProperty<int>(mp);
        MaxMp = mp;
        Shield = new ReactiveProperty<int>(0);
        Atk = atk;
        StartAP = startAP;
    }
    
    public void ApplyBuff(Effect effect) {
        effect.ApplyEffect();
        _activeEffects.Add(effect);
    }
    
    private void RemoveEffectAfterTurn(Effect effect)
    {
        effect.RemoveEffect();
        _activeEffects.Remove(effect);
    }

    public void RemoveAllEffects()
    {
        foreach (Effect effect in _activeEffects)
        {
            effect.RemoveEffect();
        }
        _activeEffects.Clear();
    }

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