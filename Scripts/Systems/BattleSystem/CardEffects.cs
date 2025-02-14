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
        public static void OnDamage(GameEntity targetEntity, int atkStat, ValueType valueType, int value) {
            int damage = Utils.GetValueByValueType(valueType, atkStat, value);
            targetEntity.OnDamage(damage);
            Debug.Log($"Damaged with value: {damage}");
        }
        
        public static void OnBuff(GameEntity targetEntity, CardDataLoader.EffectData effectData) {
            switch (effectData.type) {
                case EffectType.Attack:
                    OnDamage(targetEntity, targetEntity.Atk, effectData.valueType, effectData.value);
                    break;
                case EffectType.Heal:
                    targetEntity.ApplyBuff(new HealBuff(effectData, targetEntity));
                    break;
                case EffectType.Burn:
                    targetEntity.ApplyBuff(new BurnBuff(effectData, targetEntity));
                    break;
            }
        }
    }
}