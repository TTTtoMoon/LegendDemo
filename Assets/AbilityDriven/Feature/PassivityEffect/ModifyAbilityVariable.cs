using System;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Sirenix.OdinInspector;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("修改能力变量数值")]
    public class ModifyAbilityVariable : PassivityFeatureEffect, IAbilityVariableModifier
    {
        [Description("能力筛选")]
        public Filter<Ability> Filter;

        [Description("变量名")] [ValueDropdown("VariableNameDropdown")]
        public string VariableName;

        [Description("修改值")]
        public AbilityVariable VariableModifier = new AbilityVariable(0f);
        
        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override EffectRevocation Invoke(IAbilityTarget target)
        {
            EffectRevocation revocation = null;
            if (target is IAbilityOwner abilityOwner)
            {
                IReadOnlyList<Ability> abilities = abilityOwner.AbilityDirector.Abilities;
                for (int i = 0, length = abilities.Count; i < length; i++)
                {
                    Ability ability = abilities[i];
                    if (Filter.Verify(ability))
                    {
                        AddModifier(ability);
                        revocation += () => { RemoveModifier(ability); };
                    }
                }

                AbilityDirector.OnEnable += ModifyVariable;
                revocation += () =>
                {
                    AbilityDirector.OnEnable -= ModifyVariable;
                };
            }

            return revocation;
            
            void ModifyVariable(Ability ability)
            {
                if (ability.Source == Ability.Owner && Filter.Verify(ability))
                {
                    AddModifier(ability);
                }
            }
            
            void AddModifier(Ability ability)
            {
                for (int i = 0; i < AbilityVariable.MaxVariableCount; i++)
                {
                    if (ability.AddVariableModifier(AbilityVariable.GetVariableName(VariableName, i), this) == false)
                    {
                        break;
                    }
                }
            }

            void RemoveModifier(Ability ability)
            {
                for (int i = 0; i < AbilityVariable.MaxVariableCount; i++)
                {
                    if (ability.RemoveVariableModifier(AbilityVariable.GetVariableName(VariableName, i), this) == false)
                    {
                        break;
                    }
                }
            }
        }

        void IAbilityVariableModifier.Apply(float sourceValue, ref float value)
        {
            value += VariableModifier;
        }
        
#if UNITY_EDITOR
        private string[] VariableNameDropdown()
        {
            return AbilityEditor.AbilityEditorUtility.AllVariables;
        }
#endif
    }
}