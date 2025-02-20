using System.Collections.Generic;
using Entities.Base;
using UnityEngine.Serialization;

namespace Entities.Cards {
    public enum EffectType {
        None,
        Attack,
        Heal,
        Burn,
        IncreaseDefense
    }

    [System.Serializable]
    public abstract class Effect {
        public EffectType type;
        protected ValueType ValueType { get; private set; }
        protected int Value { get; private set; }
        public int turn;
        
        protected BaseEntity Target;
        protected Effect(EffectType type, ValueType valueType, int value, int turn, BaseEntity target) {
            this.type = type;
            ValueType = valueType;
            Value = value;
            this.turn = turn;
            Target = target;
        }

        public abstract void ApplyEffect();
        public abstract void RemoveEffect();
    }
}