using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当拥有者受到伤害时")]
    public sealed class OnOwnerTakeDamage : FeatureTrigger
    {
        protected override void OnEnable()
        {
            if (Ability.Owner is Actor actor)
            {
                actor.OnTakeDamage += Callback;
            }
        }

        protected override void OnDisable()
        {
            if (Ability.Owner is Actor actor)
            {
                actor.OnTakeDamage -= Callback;
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