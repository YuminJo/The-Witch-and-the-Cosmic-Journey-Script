namespace Entities.Cards {
    public enum EffectType {
        Attack,
        Heal,
        Burn,
        IncreaseDefense
    }

    [System.Serializable]
    public class Effect {
        public EffectType Type { get; private set; }
        public int Range { get; private set; }
        public ValueType ValueType { get; private set; }
        public int Value { get; private set; }
        public string Stat { get; private set; }
        public int Turn { get; private set; }
        
        public Effect(EffectType type, int range, ValueType valueType, int value, string stat, int turn) {
            Type = type;
            Range = range;
            ValueType = valueType;
            Value = value;
            Stat = stat;
            Turn = turn;
        }
    }
}