// 공격력 증가 버프

using System.Collections.Generic;
using Entities.Base;
using Entities.Cards;
using UnityEngine;
using static Global.Managers.CardDataLoader;

namespace Systems.Buffs {
    public class ShieldUpBuff : Effect
    {
        private readonly int _amount;
        public ShieldUpBuff(EffectData effectData, BaseEntity target) : 
            base(effectData.GetEffectType(), effectData.GetValueType(), effectData.value, effectData.turn, target)
        {
            _amount = Utils.GetValueByValueType(ValueType, target.Shield.Value , Value);
            Target.IncreaseShield(_amount);
        }

        public override void ApplyEffect() {
        }

        public override void RemoveEffect() {
            Target.OnDecreaseDef(_amount);
        }
    }
}