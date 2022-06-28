using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.Feature
{
    [Serializable]
    [Description("当怪物死亡时")]
    public sealed class OnMonsterDead : FeatureTrigger
    {
        protected override void OnEnable()
        {
            Actor.Events.OnDead += Callback;
        }

        protected override void OnDisable()
        {
            Actor.Events.OnDead -= Callback;
        }

        protected override void OnUpdate()
        {
        }

        private void Callback(Actor actor)
        {
            InvokeEffect(actor);
        }
    }
}