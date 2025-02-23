using Entities.Base;
using Entities.Cards;
using UnityEngine;
using static Global.Managers.CardDataLoader;

public class Increase : Effect
{
    public Increase(EffectData effectData, BaseEntity target) : 
        base(effectData.GetEffectType(), effectData.GetValueType(), effectData.value, effectData.turn, target)
    {
        Target.OnIncreaseCrit(Value);
    }

    public override void ApplyEffect() {
    }

    public override void RemoveEffect() {
        Target.OnDecreaseCrit(Value);
    }
}
