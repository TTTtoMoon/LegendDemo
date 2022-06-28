using System.Collections.Generic;
using Abilities;

namespace RogueGods.Gameplay.AbilityDriven
{
    public class AbilityVariableTable : IAbilityVariableTable
    {
        private const string InvalidName = "0";

        private Dictionary<string, float> m_VariableMap = new Dictionary<string, float>();

        public void Clear()
        {
            m_VariableMap.Clear();
        }

        public bool GetVariableValue(string uniqueName, out float value)
        {
            if (string.IsNullOrWhiteSpace(uniqueName) || uniqueName == InvalidName)
            {
                value = default;
                return false;
            }

            return m_VariableMap.TryGetValue(uniqueName, out value);
        }

        public void SetVariableValue(string uniqueName, float value)
        {
            if (string.IsNullOrWhiteSpace(uniqueName) || uniqueName == InvalidName) return;
            m_VariableMap[uniqueName] = value;
        }
    }
}