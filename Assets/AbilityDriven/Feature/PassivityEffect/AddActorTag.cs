using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("添加角色标签")]
    public class AddActorTag : PassivityFeatureEffect
    {
        [Description("标签")]
        public ActorTag Tag;
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override EffectRevocation Invoke(IAbilityTarget target)
        {
            if (target is Actor actor)
            {
                actor.Tag.AddTag(Tag);

                return () => actor.Tag.RemoveTag(Tag);
            }

            return null;
        }
    }
}