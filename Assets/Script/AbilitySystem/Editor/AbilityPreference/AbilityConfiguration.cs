using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Abilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AbilityEditor
{
    [ExecuteAlways]
    public class AbilityConfiguration : ScriptableObject
    {
        private static List<string> ExistGroups
        {
            get
            {
                List<string> groups = new List<string>(AbilityPreference.Instance.Abilities.Length);
                for (int i = 0; i < AbilityPreference.Instance.Abilities.Length; i++)
                {
                    string group = AbilityPreference.Instance.Abilities[i].Group;
                    if (string.IsNullOrWhiteSpace(group) || groups.Contains(group))
                    {
                        continue;
                    }

                    groups.Add(group);
                }

                groups.Sort();
                return groups;
            }
        }

        private void Awake()
        {
            if (m_Ability == null) m_Ability = CreateInstance<Ability>();
        }

        [SerializeField] private string m_CustomName;

        [SerializeField] [ValueDropdown("ExistGroups", AppendNextDrawer = true)]
        private string m_Group;

        [SerializeField] private string m_Description;

        [SerializeField] private Ability m_Ability;

        [SerializeField] private AbilityTip[] m_Tips;

        [SerializeField] public bool IsDirty;

        public string CustomName
        {
            get => m_CustomName;
            set => m_CustomName = value;
        }

        public string Group
        {
            get => m_Group;
            set => m_Group = value;
        }

        public string Description
        {
            get => m_Description;
            set => m_Description = value;
        }

        public Ability Ability => m_Ability;

        public void Save(bool refreshImmediately = true)
        {
            CustomName = m_CustomName;
            string abilityName       = Ability.GetAbilityName(m_Ability.ConfigurationID);
            string configurationPath = $"{AbilityPreference.Instance.AbilityConfigurationFolder}/{name}.asset";
            string abilityPath       = $"{AbilityPreference.Instance.AbilitySaveFolder}/{abilityName}.asset";
            if (AssetDatabase.Contains(this) == false)
            {
                if (File.Exists(abilityPath))
                {
                    EditorUtility.DisplayDialog("错误", $"ID为{m_Ability.ConfigurationID}的能力已存在，请检查ID。", "ok");
                    return;
                }

                if (Directory.Exists(AbilityPreference.Instance.AbilitySaveFolder) == false)
                {
                    Directory.CreateDirectory(AbilityPreference.Instance.AbilitySaveFolder);
                }

                if (Directory.Exists(AbilityPreference.Instance.AbilityConfigurationFolder) == false)
                {
                    Directory.CreateDirectory(AbilityPreference.Instance.AbilityConfigurationFolder);
                }

                name           = abilityName;
                m_Ability.name = abilityName;
                AssetDatabase.CreateAsset(this,      configurationPath);
                AssetDatabase.CreateAsset(m_Ability, abilityPath);
            }
            else
            {
                if (name != abilityName)
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), abilityName);
                }
                
                if (m_Ability.name != abilityName)
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(m_Ability), abilityName);
                }
            }

            IsDirty = false;
            EditorUtility.SetDirty(m_Ability);
            EditorUtility.SetDirty(this);
            if (refreshImmediately)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public void Destroy()
        {
            if (m_Ability != null) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(m_Ability));
            if (AssetDatabase.IsSubAsset(this)) AssetDatabase.RemoveObjectFromAsset(this);
            DestroyImmediate(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public bool CheckValid()
        {
            return CheckValid(out _);
        }

        public bool CheckValid(out string error)
        {
            StringBuilder      sb         = AbilityEditorUtility.GetSB();
            AbilityComponent[] components = m_Ability.GetFieldValue<AbilityComponent[]>("m_Components");
            IAbilityFeature[]  features   = m_Ability.GetFieldValue<IAbilityFeature[]>("m_Features");
            bool               valid      = true;

            for (int i = 0, length = features.Length; i < length; i++)
            {
                CheckNeedComponent(features[i]);
                switch (features[i])
                {
                    case AbilityPassivityFeature passivityFeature:
                        CheckPassivityFeature(passivityFeature);
                        break;
                    case AbilityTriggerFeature triggerFeature:
                        CheckTriggerFeature(triggerFeature);
                        break;
                    case AbilityTimelineFeature timelineFeature:
                        CheckTimelineFeature(timelineFeature);
                        break;
                }
            }

            error = sb.ToString();
            return valid;

            void CheckNeedComponent(object obj)
            {
                Type[] needComponents = obj.GetType().GetAttribute<NeedComponentAttribute>()?.Components;
                if (needComponents == null) return;

                for (int i = 0; i < needComponents.Length; i++)
                {
                    Type needComponent = needComponents[i];
                    if (components.All(x => x.GetType() != needComponent))
                    {
                        valid = false;
                        sb.AppendLine($"{obj.GetType().GetName()}缺少所需Component => {needComponent.GetName()}({needComponent.Name})");
                    }
                }
            }

            void CheckPassivityFeature(AbilityPassivityFeature passivityFeature)
            {
                FeatureTarget[]          targets   = passivityFeature.Targets;
                PassivityFeatureEffect[] effects   = passivityFeature.Effects;
                bool                     anyTarget = targets.Length > 0;

                for (int i = 0; i < targets.Length; i++)
                {
                    CheckNeedComponent(targets[i]);
                    CheckTriggerArg(Array.Empty<FeatureTrigger>(), targets[i]);
                }

                for (int i = 0; i < effects.Length; i++)
                {
                    CheckNeedComponent(effects[i]);
                    CheckTriggerArg(Array.Empty<FeatureTrigger>(), effects[i]);
                    CheckWithoutTarget(anyTarget, effects[i]);
                }
            }

            void CheckTriggerFeature(AbilityTriggerFeature triggerFeature)
            {
                FeatureTrigger[]       triggers  = triggerFeature.Triggers;
                FeatureTarget[]        targets   = triggerFeature.Targets;
                TriggerFeatureEffect[] effects   = triggerFeature.Effects;
                bool                   anyTarget = targets.Length > 0;

                for (int i = 0; i < targets.Length; i++)
                {
                    CheckNeedComponent(targets[i]);
                    CheckTriggerArg(triggers, targets[i]);
                }

                for (int i = 0; i < effects.Length; i++)
                {
                    CheckNeedComponent(effects[i]);
                    CheckTriggerArg(triggers, effects[i]);
                    CheckWithoutTarget(anyTarget, effects[i]);
                }
            }

            void CheckTimelineFeature(AbilityTimelineFeature timelineFeature)
            {
                AbilityTimelineFeature.Frame[] frames = timelineFeature.Frames;
                for (int i = 0; i < frames.Length; i++)
                {
                    AbilityTimelineFeature.Frame frame = frames[i];
                    for (int j = 0; j < frame.Targets.Length; j++)
                    {
                        CheckTriggerArg(Array.Empty<FeatureTrigger>(), frame.Targets[j]);
                    }

                    bool anyTarget = frame.Targets.Length > 0;
                    for (int j = 0; j < frame.Effects.Length; j++)
                    {
                        CheckTriggerArg(Array.Empty<FeatureTrigger>(), frame.Effects[j]);
                        CheckWithoutTarget(anyTarget, frame.Effects[j]);
                    }
                }
            }

            void CheckWithoutTarget(bool anyTarget, object obj)
            {
                bool needTarget = obj.GetType().GetAttribute<EffectWithoutTargetAttribute>() == null;
                if (needTarget && anyTarget == false)
                {
                    valid = false;
                    sb.AppendLine($"{obj.GetType().GetName()}须指定至少一个[目标选择]！");
                }
            }

            void CheckTriggerArg(FeatureTrigger[] triggers, object obj)
            {
                Type genericType = obj.GetType();
                while (genericType != null)
                {
                    if (genericType.IsGenericType)
                    {
                        Type genericTypeDefinition = genericType.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(TriggerFeatureEffect<>) ||
                            genericTypeDefinition == typeof(FeatureTarget<>))
                        {
                            genericType = genericType.GetGenericArguments()[0];
                            break;
                        }
                    }

                    genericType = genericType.BaseType;
                }

                if (genericType == null) return;

                Type genericTriggerType = typeof(FeatureTrigger<>).MakeGenericType(genericType);
                for (int i = 0; i < triggers.Length; i++)
                {
                    Type triggerType = triggers[i].GetType().BaseType;
                    while (triggerType != null)
                    {
                        if (triggerType == genericTriggerType)
                        {
                            return;
                        }

                        triggerType = triggerType.BaseType;
                    }
                }

                valid = false;
                sb.AppendLine($"{obj.GetType().GetName()}缺少带参{genericType.Name}的触发器");
            }
        }

        public void BeginFieldTip()
        {
            for (int i = 0; i < m_Tips.Length; i++)
            {
                m_Tips[i].IsObsolete = true;
            }
        }

        public string GetFieldTip(object target)
        {
            for (int i = 0, length = m_Tips.Length; i < length; i++)
            {
                if (m_Tips[i].Target == target)
                {
                    m_Tips[i].IsObsolete = false;
                    return m_Tips[i].Text;
                }
            }

            return string.Empty;
        }

        public void SetFieldTip(object target, string tip)
        {
            for (int i = 0, length = m_Tips.Length; i < length; i++)
            {
                if (m_Tips[i].Target == target)
                {
                    m_Tips[i].Text       = tip;
                    m_Tips[i].IsObsolete = false;
                    return;
                }
            }

            Array.Resize(ref m_Tips, m_Tips.Length + 1);
            m_Tips[m_Tips.Length - 1] = new AbilityTip()
            {
                Target     = target,
                Text       = tip,
                IsObsolete = false,
            };
        }

        public void EndFieldTip()
        {
            int count = 0;
            for (int i = 0; i < m_Tips.Length; i++)
            {
                if (m_Tips[i].IsObsolete == false)
                {
                    count++;
                }
            }

            AbilityTip[] tips = new AbilityTip[count];
            for (int i = 0, j = 0; i < m_Tips.Length; i++)
            {
                if (m_Tips[i].IsObsolete == false)
                {
                    tips[j] = m_Tips[i];
                    j++;
                }
            }

            m_Tips = tips;
        }

        [Serializable]
        struct AbilityTip
        {
            [SerializeReference] public object Target;
            [SerializeField]     public string Text;

            [NonSerialized] public bool IsObsolete;
        }
    }
}