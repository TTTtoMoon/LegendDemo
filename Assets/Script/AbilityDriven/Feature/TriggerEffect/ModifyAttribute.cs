using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("修改属性")]
    public sealed class ModifyAttribute : TriggerFeatureEffect
    {
        [Description("目标属性")] public AttributeType TargetAttribute;

        [Description("修改值(正加负减)")] public AbilityVariable AttributeModifier = new AbilityVariable(0f);

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] is Actor actor)
                {
                    actor.Attribute.ModifyAdditionValue(TargetAttribute, AttributeModifier.Value);
                    AttributeModifier.OnValueChanged += OnAttributeModifierChanged;

                    void OnAttributeModifierChanged(float oldValue, float newValue)
                    {
                        actor.Attribute.ModifyAdditionValue(TargetAttribute, newValue - oldValue);
                    }
                }
            }
        }
    }
}
    