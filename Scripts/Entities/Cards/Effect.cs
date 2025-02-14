using System.Collections.Generic;

namespace Entities.Cards {
    public enum EffectType {
        Attack,
        Heal,
        Burn,
        IncreaseDefense
    }

    [System.Serializable]
    public abstract class Effect {
        protected EffectType Type;
        protected ValueType ValueType { get; private set; }
        protected int Value { get; private set; }
        protected int Turn { get; private set; }
        
        protected GameEntity Target;
        protected Effect(EffectType type, ValueType valueType, int value, int turn, GameEntity target) {
            Type = type;
            ValueType = valueType;
            Value = value;
            Turn = turn;
            Target = target;
        }

        public abstract void ApplyEffect();
        public abstract void RemoveEffect();

    }
}