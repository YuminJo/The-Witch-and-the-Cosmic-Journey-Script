using System;
using System.Collections.Generic;
using Entities.Cards;
using R3;
using UnityEngine;

namespace Entities.Base {
    [Serializable]
    public abstract class BaseEntity {
        public string TemplateId { get; private set; }
    
        public ReactiveProperty<int> Hp { get; private set; }
        public int MaxHp { get; private set; }
        public ReactiveProperty<int> Shield { get; private set; } 
    
        public int Atk { get; private set; } // Attack
        public int Def { get; private set; } // Defense
        public int Crit { get; private set; } // Critical hit chance
        public int Agi { get; private set; } // Agility

        private List<Effect> _activeEffects = new();

        public BaseEntity(string templateId, int hp, int atk) {
            TemplateId = templateId;
            Hp = new ReactiveProperty<int>(hp);
            MaxHp = hp;
            Shield = new ReactiveProperty<int>(0);
            Atk = atk;
        }
    
        /// <summary>
        /// Apply effects to the entity
        /// </summary>
        public void InvokeEffects() {
            foreach (Effect effect in _activeEffects) {
                Debug.Log($"Applying effect: {effect.type}");
                effect.ApplyEffect();
                effect.turn--;
                if (effect.turn == 0) { RemoveEffectAfterTurn(effect); }
            }
        }
    
        public void ApplyBuff(Effect effect) {
            _activeEffects.Add(effect);
        }
    
        private void RemoveEffectAfterTurn(Effect effect) {
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

        public void OnDamage(int damage) {
            Debug.Log($"Damaged with value: {damage}");
            Hp.Value = Mathf.Max(Hp.Value - damage, 0);
        }

        public void OnHeal(int heal) {
            Hp.Value = Mathf.Min(Hp.Value + heal, MaxHp);
        }
    
        public void AddShield(int shield) {
            Shield.Value += shield;
        }
    
        public void OnIncreaseDef(int def) {
            Def += def;
        }
    
        public void OnDecreaseDef(int def) {
            Def -= def;
            if (Def < 0) { Def = 0; }
        }
    }
}