using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当拥有者造成伤害时")]
    public sealed class OnOwnerMakeDamage : FeatureTrigger
    {
        protected override void OnEnable()
        {
            if (Ability.Owner is Actor actor)
            {
                actor.OnMadeDamage += Callback;
            }
        }

        protected override void OnDisable()
        {
            if (Ability.Owner is Actor actor)
            {
                actor.OnMadeDamage -= Callback;
            }
        }

        protected override void OnUpdate()
        {
        }

        private void Callback(DamageResponse damageResult)
        {
            InvokeEffect(damageResult.DamageTaker);
        }
    }
}