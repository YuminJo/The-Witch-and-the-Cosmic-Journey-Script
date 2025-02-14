// 공격력 증가 버프

using System.Collections.Generic;
using Entities.Cards;
using Global.Managers;
using UnityEngine;

namespace Systems.Buffs {
    public class BurnBuff : Effect
    {
        public BurnBuff(CardDataLoader.EffectData effectData, GameEntity target) : 
            base(effectData.type, effectData.valueType, effectData.value, effectData.turn, target)
        {
        
        }

        public override void ApplyEffect() {
            Debug.Log("BurnBuff");
            Target.OnDamage(Utils.GetValueByValueType(ValueType, 50 , Value));
        }

        public override void RemoveEffect()
        {

        }
    }
}