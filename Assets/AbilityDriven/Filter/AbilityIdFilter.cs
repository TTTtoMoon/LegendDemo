using System;
using System.Linq;
using Abilities;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using AbilityEditor;
#endif

namespace RogueGods.Gameplay.AbilityDriven.Filter
{
    [Serializable]
    [Description("筛选AbilityID")]
    public class AbilityIdFilter : IFilter<Ability>
    {
        [Description("有效ID")] [ValueDropdown("Abilities", IsUniqueList = true)]
        public int[] ValidID = new int[0];

        public bool Verify(in Ability arg)
        {
            return Array.IndexOf(ValidID, arg.ConfigurationID, 0) >= 0;
        }

#if UNITY_EDITOR
        private ValueDropdownList<int> Abilities
        {
            get
            {
                ValueDropdownList<int> list = new ValueDropdownList<int>();
                list.AddRange(AbilityPreference.Instance.Abilities.Select(x =>
                    new ValueDropdownItem<int>($"[{x.Ability.ConfigurationID}]{x.CustomName}", x.Ability.ConfigurationID)));
                return list;
            }
        }
#endif
    }
}