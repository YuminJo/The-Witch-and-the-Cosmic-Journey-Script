using Entities.Cards;
using UnityEngine;

public class CardEffects
{
    /// <summary>
    /// 공격하는 효과
    /// </summary>
    /// <param name="atkStat">기본 공격력</param>
    /// <param name="selectedEnemy">적 View</param>
    /// <param name="currentEffect"></param>
    public static void OnDamage(int atkStat,GameEntity selectedEnemy, Effect currentEffect) {
        int damage = Utils.GetDamageByValueType(currentEffect.ValueType, atkStat, currentEffect.Value);
        selectedEnemy.OnDamage(damage);
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