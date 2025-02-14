/*// 공격력 증가 버프

using System.Collections.Generic;
using Entities.Cards;
using UnityEngine;

public class AttackBuff : Effect
{
    private float _attackIncrease;
a
    public AttackBuff(int range, ValueType valueType, int value, string stat, int turn) : base(EffectType.Attack, range, valueType, value, stat, turn)
    {
        _attackIncrease = value;
    }

    public override void ApplyEffect()
    {
        // 공격력을 증가시킴
        // 예시로 Debug.Log를 사용하였지만, 실제로 공격력을 증가시켜야 함
        Debug.Log("Attack buff applied: +" + _attackIncrease + " Attack");
    }

    public override void RemoveEffect()
    {
        // 예시로 Debug.Log를 사용하였지만, 실제로 공격력을 복원해야 함
        Debug.Log("Attack buff removed: -" + _attackIncrease + " Attack");
    }
}*/