using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 能力委托
    /// </summary>
    public delegate void AbilityDelegate(Ability ability);

    /// <summary>
    /// 能力
    /// </summary>
    [Serializable]
    public sealed class Ability : ScriptableObject, ISerializationCallbackReceiver
    {
        private Ability()
        {
            m_Blackboard = new Blackboard(this);
        }

        #region Static

        internal static          IAbilityFactory s_Factory;
        internal static readonly List<Ability>   s_ActiveAbilities    = new List<Ability>();
        private static readonly  List<Ability>   s_TempAbilities      = new List<Ability>();
        private static readonly  List<Ability>   s_ToRecycleAbilities = new List<Ability>();

        public static string GetAbilityName(int abilityID)
        {
            return $"Ability{abilityID}";
        }

        /// <summary>
        /// 更新
        /// </summary>
        public static void Update()
        {
            UpdateAbilities();
            RecycleAbilities();

            void RecycleAbilities()
            {
                for (int i = s_ToRecycleAbilities.Count - 1; i >= 0; i--)
                {
                    if (s_ToRecycleAbilities[i].m_ReferenceCount <= 0)
                    {
                        s_ToRecycleAbilities[i].Dispose();
                        s_Factory.Recycle(s_ToRecycleAbilities[i]);
                        s_ToRecycleAbilities.RemoveAt(i);
                    }
                }
            }

            void UpdateAbilities()
            {
                if (s_TempAbilities.Capacity < s_ActiveAbilities.Count)
                {
                    s_TempAbilities.Capacity = s_ActiveAbilities.Count;
                }

                s_TempAbilities.AddRange(s_ActiveAbilities);
                for (int i = 0, length = s_TempAbilities.Count; i < length; i++)
                {
                    Ability temp = s_TempAbilities[i];
                    if (s_ActiveAbilities.Contains(temp))
                    {
                        for (int j = 0; j < temp.m_Features.Length; j++)
                        {
                            temp.m_Features[j].OnUpdate();
                        }
                        
                        if (temp.Duration >= 0 && temp.Time >= temp.Duration)
                        {
                            temp.Director.Disable(temp);
                        }
                    }
                }

                s_TempAbilities.Clear();
            }
        }

        /// <summary>
        /// 分配一个Ability
        /// Ability失效后切记Recycle
        /// </summary>
        /// <param name="descriptor">Ability描述符</param>
        /// <returns>Ability实例</returns>
        public static Ability Allocate(IAbilityDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentException("能力描述符不能为空。");
            }

            try
            {
                Ability ability = s_Factory.Allocate(descriptor.AbilityID);
                ability.InitializeVariables(descriptor.VariableTable);
                ability.AddTag(ability.m_Tag);
                ability.Duration = ability.m_Duration;
                return ability;
            }
            catch (Exception ex)
            {
                throw new Exception($"分配Ability({descriptor.AbilityID})失败！原因如下:\n{ex}");
            }
        }

        /// <summary>
        /// 回收Ability
        /// </summary>
        /// <param name="ability"></param>
        public static void Recycle(Ability ability)
        {
            s_ToRecycleAbilities.Add(ability);
        }

        #endregion

        #region Reference

        /// <summary>
        /// 引用计数+1
        /// </summary>
        public void Reference()
        {
            m_ReferenceCount++;
            if (m_Giver != null) m_Giver.Reference();
        }

        /// <summary>
        /// 释放能力，引用计数-1，若计数为0，则清空数据
        /// </summary>
        public void UnReference()
        {
            m_ReferenceCount--;
            if (m_Giver != null) m_Giver.UnReference();
        }

        private void Dispose()
        {
            for (int i = 0; i < m_Components.Length; i++)
            {
                m_Components[i].OnDispose();
            }
            
            for (int i = 0; i < m_Features.Length; i++)
            {
                m_Features[i].OnDispose();
            }
            
            m_InstanceID     = -1;
            m_CurrentTag     = default;
            m_Director       = null;
            m_Giver          = null;
            m_Target = null;
            m_Blackboard.Clear();
            for (int i = 0; i < AbilityTag.TagLength; i++)
            {
                m_TagStack[i] = 0;
            }

            for (int i = 0, length = m_Variables.Length; i < length; i++)
            {
                m_Variables[i].Reset();
            }
            
            OnDispose?.Invoke();
            OnDispose = null;
        }

        #endregion

        #region SerializeField

        [SerializeField] [Description("配置ID", "应保证唯一性")]
        internal int m_ConfigurationID;

        [SerializeField] [Description("标签")] 
        internal AbilityTag m_Tag;

        [SerializeField] [Description("有效时长", "-1:永久 0:瞬时 单位秒")]
        internal AbilityVariable m_Duration = new AbilityVariable(-1f);

        [SerializeReference] [Description("组件集合")]
        internal AbilityComponent[] m_Components = new AbilityComponent[0];

        [SerializeReference] [Description("功能集合")]
        internal IAbilityFeature[] m_Features = new IAbilityFeature[0];

        #endregion

        #region Field

        /// <summary>
        /// 实例ID
        /// </summary>
        [NonSerialized] internal int m_InstanceID = -1;

        /// <summary>
        /// 标签栈
        /// 存在越界风险，每个tag最多加255次。但一般不会出现这种情况
        /// </summary>
        [NonSerialized] internal byte[] m_TagStack = new byte[AbilityTag.TagLength];

        /// <summary>
        /// 当前标签
        /// </summary>
        [NonSerialized] internal AbilityTag m_CurrentTag = default;

        /// <summary>
        /// 引用计数
        /// </summary>
        [NonSerialized] internal int m_ReferenceCount = 0;

        /// <summary>
        /// 持有者
        /// </summary>
        [NonSerialized] internal AbilityDirector m_Director = null;

        /// <summary>
        /// 来源链
        /// </summary>
        [NonSerialized] internal Ability m_Giver = null;

        /// <summary>
        /// 获取时间点
        /// </summary>
        [NonSerialized] internal float m_EnableAtTime = -1f;

        /// <summary>
        /// 时间流速
        /// </summary>
        [NonSerialized] internal float m_TimeScale = 1f;

        /// <summary>
        /// 数据黑板
        /// </summary>
        [NonSerialized] internal Blackboard m_Blackboard = null;

        /// <summary>
        /// 指定目标
        /// </summary>
        [NonSerialized] internal IAbilityTarget m_Target = null;

        /// <summary>
        /// 已有变量
        /// </summary>
        [NonSerialized] internal AbilityVariable[] m_Variables = Array.Empty<AbilityVariable>();

        #endregion

        #region Readonly Field

        /// <summary>
        /// 是否为激活态
        /// </summary>
        public bool IsActive => m_InstanceID >= 0;

        /// <summary>
        /// 配置ID
        /// </summary>
        public int ConfigurationID => m_ConfigurationID;

        /// <summary>
        /// 标签
        /// </summary>
        public AbilityTag Tag => m_CurrentTag;

        /// <summary>
        /// 实例ID
        /// </summary>
        public int InstanceID => m_InstanceID;

        /// <summary>
        /// 引用计数
        /// </summary>
        public int ReferenceCount => m_ReferenceCount;

        /// <summary>
        /// 驱动
        /// </summary>
        public AbilityDirector Director => m_Director;

        /// <summary>
        /// 持有者
        /// </summary>
        public IAbilityOwner Owner => m_Director.Owner;

        /// <summary>
        /// 来源者
        /// </summary>
        public IAbilityOwner Source => RootGiver == null ? Owner : RootGiver.Owner;
        
        /// <summary>
        /// 指定目标
        /// </summary>
        public IAbilityTarget Target => m_Target;

        /// <summary>
        /// 直接给予者
        /// </summary>
        public Ability Giver => m_Giver;

        /// <summary>
        /// 最初的给予者
        /// </summary>
        public Ability RootGiver
        {
            get
            {
                Ability giver = m_Giver;
                while (giver != null)
                {
                    if (giver.m_Giver == null)
                    {
                        return giver;
                    }

                    giver = giver.m_Giver;
                }

                return null;
            }
        }

        /// <summary>
        /// 已流逝时长
        /// </summary>
        public float Time => (UnityEngine.Time.time - m_EnableAtTime) * m_TimeScale;

        /// <summary>
        /// 总持续时长
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// 数据黑板
        /// </summary>
        public Blackboard Blackboard => m_Blackboard;

        /// <summary>
        /// 功能集合
        /// </summary>
        public IReadOnlyList<IAbilityFeature> Features => m_Features;

        /// <summary>
        /// 当能力被释放回池子时
        /// </summary>
        public event Action OnDispose;

        #endregion

        #region Tag

        /// <summary>
        /// 新增标签
        /// </summary>
        /// <param name="tag">目标标签</param>
        public void AddTag(AbilityTag tag)
        {
            for (int i = 0; i < AbilityTag.TagLength; i++)
            {
                long flag = 1L << i;
                if ((tag.Tag & flag) != flag)
                {
                    continue;
                }

                if (m_TagStack[i] == byte.MaxValue)
                {
                    throw new OverflowException("能力标签栈溢出");
                }

                m_TagStack[i]++;
            }

            m_CurrentTag |= tag;
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="tag">目标标签</param>
        public void RemoveTag(AbilityTag tag)
        {
            long result = 0;
            for (int i = 0; i < AbilityTag.TagLength; i++)
            {
                long flag = 1L << i;
                if ((tag.Tag & flag) == flag && m_TagStack[i] > 0)
                {
                    m_TagStack[i]--;
                }

                if (m_TagStack[i] > 0)
                {
                    result |= flag;
                }
            }

            m_CurrentTag = new AbilityTag(result);
        }

        #endregion

        #region Variable

        /// <summary>
        /// 设置变量初始值
        /// </summary>
        /// <param name="variableTable">变量表</param>
        public void InitializeVariables(IAbilityVariableTable variableTable)
        {
            if (variableTable == null) return;
            for (int i = 0, length = m_Variables.Length; i < length; i++)
            {
                m_Variables[i].InitializeValue(variableTable);
            }
        }

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="uniqueName">变量名</param>
        /// <param name="variable">变量实例</param>
        /// <returns>是否成功找到变量</returns>
        public bool TryGetVariable(string uniqueName, out AbilityVariable variable)
        {
            for (int i = 0, length = m_Variables.Length; i < length; i++)
            {
                if (m_Variables[i].UniqueName == uniqueName)
                {
                    variable = m_Variables[i];
                    return true;
                }
            }

            variable = null;
            return false;
        }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <param name="uniqueName">变量名称</param>
        /// <param name="fallback">找不到变量时的返回值</param>
        /// <returns>变量值，无法找到时返回fallback</returns>
        public float GetVariableValue(string uniqueName, float fallback = default)
        {
            return TryGetVariable(uniqueName, out AbilityVariable variable) ? variable.Value : fallback;
        }

        /// <summary>
        /// 添加修改器
        /// </summary>
        public bool AddVariableModifier(string uniqueName, IAbilityVariableModifier modifier)
        {
            if (TryGetVariable(uniqueName, out AbilityVariable variable))
            {
                variable.AddModifier(modifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除修改器
        /// </summary>
        public bool RemoveVariableModifier(string uniqueName, IAbilityVariableModifier modifier)
        {
            if (TryGetVariable(uniqueName, out AbilityVariable variable))
            {
                variable.RemoveModifier(modifier);
                return true;
            }
            
            return false;
        }

        #endregion

        #region Component

        /// <summary>
        /// 尝试获取组件
        /// </summary>
        /// <param name="component">组件实例</param>
        /// <typeparam name="TComponent">组件类型</typeparam>
        /// <returns>返回是否找到组件</returns>
        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : AbilityComponent
        {
            for (int i = 0, length = m_Components.Length; i < length; i++)
            {
                if (m_Components[i] is TComponent result)
                {
                    component = result;
                    return true;
                }
            }

            component = null;
            return false;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="TComponent">组件类型</typeparam>
        /// <returns>返回组件，未找到则返回空</returns>
        public TComponent GetComponent<TComponent>() where TComponent : AbilityComponent
        {
            TryGetComponent(out TComponent component);
            return component;
        }

        #endregion

        #region Feature

        /// <summary>
        /// 查找首个类型符合的功能
        /// </summary>
        /// <param name="feature">功能实例</param>
        /// <typeparam name="TFeature">功能类型</typeparam>
        /// <returns>返回是否找到了功能</returns>
        public bool FindFeature<TFeature>(out TFeature feature) where TFeature : IAbilityFeature
        {
            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                if (m_Features[i] is TFeature result)
                {
                    feature = result;
                    return true;
                }
            }

            feature = default;
            return false;
        }
        
        /// <summary>
        /// 查找首个类型符合的功能
        /// </summary>
        /// <typeparam name="TFeature">功能类型</typeparam>
        /// <returns>返回功能实例，未找到则返回空</returns>
        public TFeature FindFeature<TFeature>() where TFeature : IAbilityFeature
        {
            for (int i = 0, length = m_Features.Length; i < length; i++)
            {
                if (m_Features[i] is TFeature result)
                {
                    return result;
                }
            }

            return default;
        }

        #endregion

        #region Serialization Callback

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            AddTag(m_Tag);
        }

        #endregion
    }
}