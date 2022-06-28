using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbilityEditor
{
    [CreateAssetMenu]
    public class AbilityPreference : ScriptableObject
    {
        private static AbilityPreference m_Instance;

        public static AbilityPreference Instance
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance;
                }

                string asset = AssetDatabase.FindAssets("t:AbilityEditor.AbilityPreference").FirstOrDefault();
                if (string.IsNullOrEmpty(asset))
                {
                    throw new Exception("工程中无法找到AbilityPreference！");
                }

                asset      = AssetDatabase.GUIDToAssetPath(asset);
                m_Instance = AssetDatabase.LoadAssetAtPath<AbilityPreference>(asset);
                return m_Instance;
            }
        }

        public string AbilityConfigurationFolder =>
            AssetDatabase.GetAssetPath(Instance).Replace($"{Instance.name}.asset", "Configuration");

        [SerializeField] [FolderPath] public string AbilitySaveFolder;

        [SerializeField] [ReadOnly] public AbilityConfiguration[] Abilities;

        public Ability GetAbility(int id)
        {
            var configuration = GetAbilityConfiguration(id);
            return configuration != null ? configuration.Ability : null;
        }

        public AbilityConfiguration GetAbilityConfiguration(int id)
        {
            return Abilities?.FirstOrDefault(x => x.Ability.ConfigurationID == id);
        }
    }
}