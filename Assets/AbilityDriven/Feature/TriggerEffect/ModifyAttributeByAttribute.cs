using System;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("修改属性（基于属性）")]
    public sealed class ModifyAttributeByAttribute : TriggerFeatureEffect
    {
        [Description("目标属性")] 
        public AttributeType TargetAttribute;
        
        [Description("来源属性")] 
        public AttributeType BasedAttribute;
        
        [Description("来源属性转换率(正加负减)")] 
        public AbilityVariable AttributeConversionRote = new AbilityVariable(1f);

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
                    if (TargetAttribute == BasedAttribute)
                    {
                        actor.Attribute.ModifyMultiplyValue(TargetAttribute, AttributeConversionRote.Value);
                        AttributeConversionRote.OnValueChanged += OnVariableChanged;

                        void OnVariableChanged(float oldValue, float newValue)
                        {
                            actor.Attribute.ModifyMultiplyValue(TargetAttribute, newValue - oldValue);
                        }
                    }
                    else
                    {
                        actor.Attribute.ModifyAdditionValue(TargetAttribute, actor.Attribute[BasedAttribute] * AttributeConversionRote.Value);
                        actor.Attribute.AddOnValueChanged(BasedAttribute, OnAttributeChanged);
                        AttributeConversionRote.OnValueChanged += OnVariableChanged;

                        void OnAttributeChanged(float oldValue, float newValue)
                        {
                            actor.Attribute.ModifyAdditionValue(TargetAttribute, (newValue - oldValue) * AttributeConversionRote.Value);
                        }

                        void OnVariableChanged(float oldValue, float newValue)
                        {
                            actor.Attribute.ModifyAdditionValue(TargetAttribute, actor.Attribute[BasedAttribute] * (newValue - oldValue));
                        }
                    }
                }
            }
        }
    }
}