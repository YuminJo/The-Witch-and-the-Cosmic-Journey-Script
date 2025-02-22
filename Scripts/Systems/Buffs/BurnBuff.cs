// 공격력 증가 버프

using System.Collections.Generic;
using Entities.Base;
using Entities.Cards;
using Global.Managers;
using UnityEngine;

namespace Systems.Buffs {
    public class BurnBuff : Effect
    {
        public BurnBuff(CardDataLoader.EffectData effectData, BaseEntity target) : 
            base(effectData.GetEffectType(), effectData.GetValueType(), effectData.value, effectData.turn, target)
        {
        
        }

        public override void ApplyEffect() {
            Target.OnDamage(Utils.GetValueByValueType(ValueType.Percent, 50 , 100));
        }

        public override void RemoveEffect()
        {

        }
    }
}