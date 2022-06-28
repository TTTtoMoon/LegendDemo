using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("修改Ability的DamagePower")]
    public class ModifyAbilityDamagePower : PassivityFeatureEffect
    {
        [Description("Ability过滤")]
        public Filter<Ability> AbilityFilter = new Filter<Ability>();

        [Description("DamagePower修改值")]
        public AbilityVariable DamagePowerModifier = new AbilityVariable(0f);

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override EffectRevocation Invoke(IAbilityTarget target)
        {
            GameManager.DamageSystem.DamageModifier += DamageSystemOnDamageModifier;
            return () => { GameManager.DamageSystem.DamageModifier -= DamageSystemOnDamageModifier; };

            void DamageSystemOnDamageModifier(in DamageRequest damageRequest, ref Attacker attacker, ref Defender defender)
            {
                if(damageRequest.DamageMaker != target || AbilityFilter.Verify(damageRequest.Ability) == false) return;
                attacker.DamagePower += DamagePowerModifier.Value;
            }
        }
    }
}