using System;
using System.Collections.Generic;
using UnityEngine;

namespace Abilities
{
    public delegate void OnAbilityVariableChange(float oldValue, float newValue);

    /// <summary>
    /// 变量
    /// </summary>
    [Serializable]
    public sealed class AbilityVariable : ISerializationCallbackReceiver
    {
        public const int MaxVariableCount = 100;

        private struct Key : IEquatable<Key>
        {
            public Key(string name, int index)
            {
                Name  = name;
                Index = index;
            }

            public readonly string Name;
            public readonly int    Index;

            public bool Equals(Key other)
            {
                return Name == other.Name && Index == other.Index;
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Index;
                }
            }
        }

        private static Dictionary<Key, string> s_Map = new Dictionary<Key, string>();

        public static string GetVariableName(string name, int index)
        {
            if (index >= MaxVariableCount)
            {
                throw new IndexOutOfRangeException();
            }

            string result;
            Key    key = new Key(name, index);
            if (s_Map.TryGetValue(key, out result) == false)
            {
                if (name.StartsWith("m_"))
                {
                    name = name.Substring(2);
                }
                else if (name.StartsWith("_"))
                {
                    name = name.Substring(1);
                }
                else if (Char.IsLower(name[0]))
                {
                    name = Char.ToUpper(name[0]) + name.Substring(1);
                }

                result = index == 0 ? name : $"{name}{index:00}";
                s_Map.Add(key, result);
            }

            return result;
        }

        public AbilityVariable(float value)
        {
            m_Value = value;
            m_IsInt = false;
        }
        
        public AbilityVariable(int value)
        {
            m_Value = value;
            m_IsInt = true;
        }

        // 配置数据
        [SerializeField] internal string m_UniqueName;
        [SerializeField] internal float  m_Value;
        [SerializeField] internal bool   m_IsInt;

        // 运行时数据
        [NonSerialized] internal float                          m_CurrentValue;
        [NonSerialized] internal List<IAbilityVariableModifier> m_Modifiers;

        /// <summary>
        /// 数值变化事件
        /// </summary>
        public event OnAbilityVariableChange OnValueChanged;

        /// <summary>
        /// 唯一名称
        /// </summary>
        public string UniqueName => m_UniqueName;

        /// <summary>
        /// 最终值
        /// </summary>
        public float Value
        {
            get
            {
                float result = m_CurrentValue;
                for (int i = 0, length = m_Modifiers.Count; i < length; i++)
                {
                    m_Modifiers[i].Apply(m_CurrentValue, ref result);
                }

                return m_IsInt ? (int)result : result;
            }
        }

        public int IntValue => (int) Value;

        /// <summary>
        /// 添加修改器
        /// </summary>
        public void AddModifier(IAbilityVariableModifier modifier)
        {
            float oldValue = Value;
            m_Modifiers.Add(modifier);
            OnValueChanged?.Invoke(oldValue, Value);
        }

        /// <summary>
        /// 移除修改器
        /// </summary>
        /// <param name="modifier"></param>
        public void RemoveModifier(IAbilityVariableModifier modifier)
        {
            float oldValue = Value;
            m_Modifiers.Remove(modifier);
            OnValueChanged?.Invoke(oldValue, Value);
        }

        public static implicit operator float(AbilityVariable variable)
        {
            return variable != null ? variable.Value : default;
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        /// <param name="variableTable">变量表</param>
        internal void InitializeValue(IAbilityVariableTable variableTable)
        {
            if (variableTable.GetVariableValue(m_UniqueName, out float value))
            {
                float oldValue = Value;
                m_CurrentValue = m_IsInt ? (int)value : value;
                OnValueChanged?.Invoke(oldValue, Value);
            }
        }

        /// <summary>
        /// 还原
        /// </summary>
        internal void Reset()
        {
            m_CurrentValue = m_Value;
            m_Modifiers.Clear();
            OnValueChanged = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_Modifiers = new List<IAbilityVariableModifier>();
            AbilitySerializer.PushVariable(this);
            m_CurrentValue = m_Value;
        }
    }
}