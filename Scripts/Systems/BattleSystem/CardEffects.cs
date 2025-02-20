using Entities.Base;
using Entities.Cards;
using Global.Managers;
using Systems.Buffs;
using UnityEngine;

namespace Systems.BattleSystem {
    public static class CardEffects
    {
        /// <summary>
        /// 공격하는 효과
        /// </summary>
        /// <param name="targetEntity">선택된 적</param>
        /// <param name="atkStat">공격력</param>
        /// <param name="valueType">값의 타입</param>
        /// <param name="value">값</param>
        public static void OnDamage(BaseEntity targetEntity, int atkStat, ValueType valueType, int value) {
            int damage = Utils.GetValueByValueType(valueType, atkStat, value);
            targetEntity.OnDamage(damage);
        }
        
        public static void OnBuff(BaseEntity attacker,BaseEntity target, CardDataLoader.EffectData effectData) {
            switch (effectData.GetEffectType()) {
                case EffectType.Attack:
                    OnDamage(target, attacker.Atk, effectData.GetValueType(), effectData.value);
                    break;
                case EffectType.Heal:
                    target.ApplyBuff(new HealBuff(effectData, target));
                    break;
                case EffectType.Burn:
                    Debug.Log("Apply Burn");
                    target.ApplyBuff(new BurnBuff(effectData, target));
                    break;
            }
        }
    }
}