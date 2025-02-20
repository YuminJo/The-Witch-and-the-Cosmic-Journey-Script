using Entities.Base;
using Entities.Cards;
using static Global.Managers.CardDataLoader;

namespace Systems.Buffs {
    public class HealBuff : Effect
    {
        public HealBuff(EffectData effectData, BaseEntity target) : 
            base(effectData.GetEffectType(), effectData.GetValueType(), effectData.value, effectData.turn, target)
        {
        
        }

        public override void ApplyEffect() {
            Target.OnHeal(Utils.GetValueByValueType(ValueType, 50 , Value));
        }

        public override void RemoveEffect()
        {

        }
    }
}