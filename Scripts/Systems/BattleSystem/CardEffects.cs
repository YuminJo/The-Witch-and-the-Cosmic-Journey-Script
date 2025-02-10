using Entities.Cards;
using UnityEngine;

public class CardEffects
{
    /// <summary>
    /// 공격하는 효과
    /// </summary>
    /// <param name="targetEntity">선택된 적</param>
    /// <param name="atkStat">공격력</param>
    /// <param name="valueType">값의 타입</param>
    /// <param name="value">값</param>
    public static void OnDamage(GameEntity targetEntity, int atkStat, ValueType valueType, int value) {
        int damage = Utils.GetDamageByValueType(valueType, atkStat, value);
        targetEntity.OnDamage(damage);
        Debug.Log($"Damaged with value: {damage}");
    }

    // 회복하는 효과
    public static void OnHeal(int atkStat,GameEntity selectedEnemy, Effect currentEffect) {
        int heal = Utils.GetDamageByValueType(currentEffect.ValueType, atkStat, currentEffect.Value);
        selectedEnemy.OnHeal(heal);
        Debug.Log($"Healed with value: {heal}");
        // 실제 회복 처리 로직
    }

    // 버프 효과
    public static void OnBuff(int value) {
        Debug.Log($"Buff applied with value: {value}");
        // 실제 버프 처리 로직
    }

    // 디버프 효과
    public static void OnDebuff(int value) {
        Debug.Log($"Debuff applied with value: {value}");
        // 실제 디버프 처리 로직
    }
}