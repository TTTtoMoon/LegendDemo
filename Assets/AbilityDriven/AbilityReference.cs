using System;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 能力引用
    /// </summary>
    [Serializable]
    public sealed class AbilityReference : IAbilityVariableTable
    {
        [SerializeField] private int                m_AbilityID;
        [SerializeField] private OverrideVariable[] m_OverrideVariables;

        public int AbilityID => m_AbilityID;

        public bool GetVariableValue(string uniqueName, out float value)
        {
            for (int i = 0, length = m_OverrideVariables.Length; i < length; i++)
            {
                if (m_OverrideVariables[i].Target == uniqueName)
                {
                    value = m_OverrideVariables[i].Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        [Serializable]
        public class OverrideVariable
        {
#if UNITY_EDITOR
            private string Label => $"[覆写] {Target}";
#endif

            public string Target;

            [VariableName("Target")] [LabelText("$Label")]
            public AbilityVariable Value;
        }
    }
}