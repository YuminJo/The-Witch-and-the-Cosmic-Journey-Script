using UnityEngine;

public enum BuffType {
    None,                   // 없음
    Poison,                 // 중독
    Stun,                   // 기절
    Silence,                // 침묵
    Bleed,                  // 출혈
    Burn,                   // 화상
    Freeze,                 // 빙결
    Shock,                  // 감전
    Sleep,                  // 수면
    Confusion,              // 혼란
    Charm,                  // 매혹
    Fear,                   // 공포
    Taunt,                  // 도발
    Invincible,             // 무적
    Shield,                 // 보호막
    Regeneration,           // 재생
    Haste,                  // 가속
    Slow,                   // 둔화
    Disarm,                 // 무장 해제
    Blind,                  // 실명
    Curse,                  // 저주
    Hex,                    // 주술
    Mark,                   // 표식
    Stealth,                // 은신
    Counter,                // 반격
    Reflect,                // 반사
    Absorb,                 // 흡수
    LifeSteal,              // 생명 흡수
    ManaBurn,               // 마나 연소
    ManaShield,             // 마나 보호막
    DamageReduction,        // 피해 감소
    DamageIncrease,         // 피해 증가
    CriticalRateIncrease,   // 치명타 확률 증가
    CriticalDamageIncrease, // 치명타 피해 증가
    AccuracyIncrease,       // 명중률 증가
    EvasionIncrease,        // 회피율 증가
    AttackSpeedIncrease,    // 공격 속도 증가
    MovementSpeedIncrease,  // 이동 속도 증가
    DefenseIncrease,        // 방어력 증가
    MagicDefenseIncrease,   // 마법 방어력 증가
    AttackSpeedDecrease,    // 공격 속도 감소
    MovementSpeedDecrease,  // 이동 속도 감소
    DefenseDecrease,        // 방어력 감소
    MagicDefenseDecrease,   // 마법 방어력 감소
    CriticalRateDecrease,   // 치명타 확률 감소
    CriticalDamageDecrease, // 치명타 피해 감소
    AccuracyDecrease,       // 명중률 감소
    EvasionDecrease,        // 회피율 감소
    DamageOverTime,         // 지속 피해
    HealOverTime,           // 지속 회복
    ManaOverTime,           // 지속 마나 회복
    CooldownReduction,      // 재사용 대기시간 감소
    CooldownIncrease,       // 재사용 대기시간 증가
}

public class ItemBuff
{
    public BuffType Type { get; private set; }
    public int Value { get; private set; }
    public float Duration { get; private set; }
    public float RemainingTime { get; private set; }

    public ItemBuff(BuffType type, int value, float duration)
    {
        Type = type;
        Value = value;
        Duration = duration;
        RemainingTime = duration;
    }

    public void Update(float deltaTime)
    {
        RemainingTime -= deltaTime;
    }
}
