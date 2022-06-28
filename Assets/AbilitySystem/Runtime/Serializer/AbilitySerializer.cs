using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public static class AbilitySerializer
    {
        private static List<AbilityVariable> s_VariableCollector = new List<AbilityVariable>();
        private static bool                   s_IsSerializing;

        internal static void PushVariable(AbilityVariable variable)
        {
            if (s_IsSerializing == false) return;
            s_VariableCollector.Add(variable);
        }

        public static Ability DeSerialize(string json)
        {
            s_IsSerializing = true;
            Ability ability = FromJson(json);
            s_IsSerializing = false;

            ability.m_Variables = s_VariableCollector.ToArray();
            s_VariableCollector.Clear();
            return ability;
        }

        public static string ToJson(Ability ability)
        {
            return JsonUtility.ToJson(ability, true);
        }

        public static Ability FromJson(string json)
        {
            Ability ability = ScriptableObject.CreateInstance<Ability>();
            JsonUtility.FromJsonOverwrite(json, ability);
            return ability;
        }
    }
}