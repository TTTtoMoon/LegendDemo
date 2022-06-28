using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.VFX;
using RogueGods.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    /// <summary>
    /// 环绕效果基类
    /// </summary>
    [Serializable]
    public abstract class AroundEffect : PassivityFeatureEffect
    {
        [Description("环绕编号", "从MiscConfiguration配置")]
        public int AroundConfigIndex;

        [Description("环绕高度")] 
        public float AroundHeight;

        [Description("环绕特效")] 
        public GameObject AroundVFX;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected sealed override EffectRevocation Invoke(IAbilityTarget target)
        {
            AroundAgent agent = CreateAgent(target);
            AroundGroup.AddAgent(target, AroundConfigIndex, agent);
            return () => { AroundGroup.RemoveAgent(agent); };
        }

        protected abstract AroundAgent CreateAgent(IAbilityTarget target);

        #region Arounder

        protected class AroundGroup
        {
            public static void AddAgent(IAbilityTarget target, int configID, AroundAgent agent)
            {
                AroundGroup group = GetGroup();
                group.m_Agents.Add(agent);

                AroundGroup GetGroup()
                {
                    for (int i = 0, length = s_AroundGroups.Count; i < length; i++)
                    {
                        AroundGroup temp = s_AroundGroups[i];
                        if (ReferenceEquals(temp.m_Target, target) &&
                            temp.m_ConfigID == configID)
                        {
                            return temp;
                        }
                    }

                    AroundGroup newGroup = new AroundGroup(target, configID);
                    s_AroundGroups.Add(newGroup);
                    return newGroup;
                }
            }

            public static void RemoveAgent(AroundAgent agent)
            {
                agent.Dispose();
                for (int i = 0, length = s_AroundGroups.Count; i < length; i++)
                {
                    AroundGroup temp = s_AroundGroups[i];
                    if (temp.m_Agents.Remove(agent))
                    {
                        if (temp.m_Agents.Count == 0)
                        {
                            temp.Dispose();
                            s_AroundGroups.RemoveAt(i);
                        }

                        break;
                    }
                }
            }

            private static List<AroundGroup> s_AroundGroups = new List<AroundGroup>(8);

            private IAbilityTarget    m_Target;
            private int               m_ConfigID;
            private AroundConfig      m_Config;
            private float             m_ActiveTime;
            private List<AroundAgent> m_Agents = new List<AroundAgent>(16);

            private AroundGroup(IAbilityTarget target, int configID)
            {
                m_Target     = target;
                m_ConfigID   = configID;
                m_Config     = MiscConfiguration.GetAroundConfig(configID);
                m_ActiveTime = Time.time;

                Update();
                GameManager.Instance.RegisterLateUpdate(Update, order: GameManager.LateUpdateMonoOrder.LateUpdateTransform);
            }

            private void Dispose()
            {
                GameManager.Instance.UnRegisterLateUpdate(Update);
            }

            private void Update()
            {
                const float RoundAngle = 360f;

                float   deltaTime      = Time.time - m_ActiveTime;
                float   currentAngle   = (deltaTime * m_Config.Speed) % RoundAngle;
                float   angleOffset1   = RoundAngle                   / m_Config.Count;
                float   angleOffset2   = angleOffset1                 / m_Agents.Count;
                Vector3 centerPosition = m_Target.Position;
                Vector3 deltaPosition  = new Vector3(0f, 0f, m_Config.Radius);
                for (int i = 0, length = m_Agents.Count; i < length; i++)
                {
                    AroundAgent agent = m_Agents[i];
                    deltaPosition.y = agent.AroundHeight;
                    Vector3[] prePositions = ArrayPool<Vector3>.Get(m_Config.Count);
                    for (int j = 0; j < m_Config.Count; j++)
                    {
                        Quaternion rotation        = Quaternion.Euler(0f, currentAngle + angleOffset2 * i + angleOffset1 * j, 0);
                        Transform  vfxTransform    = agent.VFX[j].transform;
                        Vector3    currentPosition = centerPosition + rotation * deltaPosition;
                        prePositions[j]       = vfxTransform.position;
                        vfxTransform.position = currentPosition;
                        vfxTransform.rotation = rotation;
                    }

                    agent.OnUpdate(prePositions);
                    ArrayPool<Vector3>.Release(prePositions);
                }
            }
        }

        protected abstract class AroundAgent : IEquatable<AroundAgent>
        {
            public AroundAgent(AroundEffect effect)
            {
                AroundHeight = effect.AroundHeight;
                VFX          = CreateVFX();

                VFXInstance[] CreateVFX()
                {
                    AroundConfig  config = MiscConfiguration.GetAroundConfig(effect.AroundConfigIndex);
                    VFXInstance[] result = new VFXInstance[config.Count];
                    for (int i = 0; i < config.Count; i++)
                    {
                        result[i] = GameManager.VFXSystem.CreateInstance(effect.AroundVFX);
                    }

                    return result;
                }
            }

            public readonly float         AroundHeight;
            public readonly VFXInstance[] VFX;

            public void Dispose()
            {
                for (int i = 0, length = VFX.Length; i < length; i++)
                {
                    GameManager.VFXSystem.DestroyInstance(VFX[i]);
                }

                OnDispose();
            }

            protected abstract void OnDispose();

            public abstract void OnUpdate(Vector3[] prePositions);

            public bool Equals(AroundAgent other)
            {
                return ReferenceEquals(this, other);
            }
        }

        #endregion
    }

    [Serializable]
    public struct AroundConfig
    {
        [LabelText("环绕半径")] public float Radius;
        [LabelText("环绕速度")] public float Speed;
        [LabelText("环绕数量")] public int   Count;

#if UNITY_EDITOR
        public string Title => $"半径({Radius}米) 速度({Speed}度/秒) 数量({Count}个)";
#endif
    }
}