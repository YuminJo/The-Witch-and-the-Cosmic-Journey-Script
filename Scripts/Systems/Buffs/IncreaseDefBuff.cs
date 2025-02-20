// 공격력 증가 버프

using System.Collections.Generic;
using Entities.Base;
using Entities.Cards;
using UnityEngine;
using static Global.Managers.CardDataLoader;

namespace Systems.Buffs {
    public class IncreaseDefBuff : Effect
    {
        public IncreaseDefBuff(EffectData effectData, BaseEntity target) : 
            base(effectData.GetEffectType(), effectData.GetValueType(), effectData.value, effectData.turn, target)
        {
            Target.OnIncreaseDef(Value);
        }

        public override void ApplyEffect() {
        }

        public override void RemoveEffect() {
            Target.OnDecreaseDef(Value);
        }
    }
}