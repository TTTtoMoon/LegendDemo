using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 数据过滤器
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public interface IFilter<T>
    {
        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="arg">数据</param>
        /// <returns>返回是否满足过滤器条件</returns>
        bool Verify(in T arg);
    }

    /// <summary>
    /// 数据过滤器
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    [Serializable]
    public sealed class Filter<T> : ISerializationCallbackReceiver
    {
        [Serializable]
        [Description("并且组", "组员之间为并且关系")]
        public class AndGroup : IFilter<T>
        {
            [SerializeField] internal Item[] m_Items = new Item[0];

            public bool Verify(in T arg)
            {
                for (int i = 0, length = m_Items.Length; i < length; i++)
                {
                    if (m_Items[i].Verify(arg) == false)
                    {
                        return false;
                    }
                }

                return true;
            }

            public override string ToString()
            {
                string str = "(";
                for (int i = 0; i < m_Items.Length; i++)
                {
                    if (i > 0)
                    {
                        str += " & " + m_Items[i];
                    }
                    else
                    {
                        str += m_Items[i].ToString();
                    }
                }

                str += ")";
                return str;
            }
        }

        [Serializable]
        [Description("或者组", "组员之间为或者关系")]
        public class OrGroup : IFilter<T>
        {
            [SerializeField] internal Item[] m_Items = new Item[0];

            public bool Verify(in T arg)
            {
                for (int i = 0, length = m_Items.Length; i < length; i++)
                {
                    if (m_Items[i].Verify(arg))
                    {
                        return true;
                    }
                }

                return false;
            }

            public override string ToString()
            {
                string str = "(";
                for (int i = 0; i < m_Items.Length; i++)
                {
                    if (i > 0)
                    {
                        str += " | " + m_Items[i];
                    }
                    else
                    {
                        str += m_Items[i].ToString();
                    }
                }

                str += ")";
                return str;
            }
        }

        [Serializable]
        public struct Item
        {
            [SerializeField]     internal int    m_Index;
            [SerializeReference] internal object m_Value;
            [SerializeField]     internal bool   m_Reverse;

            [NonSerialized] [ShowInInspector]
            internal IFilter<T> m_Filter;

            public Item(IFilter<T> value)
            {
                m_Index   = 0;
                m_Value   = value;
                m_Reverse = false;
                m_Filter  = value;
            }

            public bool Verify(in T arg)
            {
                bool isOk = m_Filter == null || m_Filter.Verify(arg);
                if (m_Reverse)
                {
                    isOk = !isOk;
                }

                return isOk;
            }

            public override string ToString()
            {
                if (m_Filter == null)
                {
                    return "无筛选限制";
                }

                string str;
                switch (m_Filter)
                {
                    case AndGroup andGroup:
                    case OrGroup orGroup:
                        str = m_Filter.ToString();
                        break;
                    default:
                        str = m_Filter.GetType().GetCustomAttribute<DescriptionAttribute>()?.Name ?? m_Value.GetType().Name;
                        break;
                }

                if (m_Reverse)
                {
                    str = $"!{str}";
                }

                return str;
            }
        }

        [SerializeField] private Item       m_Item;
        [SerializeField] private AndGroup[] m_AndGroups;
        [SerializeField] private OrGroup[]  m_OrGroups;

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="arg">数据</param>
        /// <returns>返回是否满足过滤器条件</returns>
        public bool Verify(in T arg)
        {
            return m_Item.Verify(arg);
        }

        public override string ToString()
        {
            return m_Item.ToString();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            List<AndGroup> andGroups = new List<AndGroup>();
            List<OrGroup>  orGroups  = new List<OrGroup>();
            Serialize(ref m_Item);
            m_AndGroups = andGroups.ToArray();
            m_OrGroups  = orGroups.ToArray();
            
            void Serialize(ref Item item)
            {
                if (item.m_Filter is AndGroup andGroup)
                {
                    item.m_Index = andGroups.Count + 1;
                    item.m_Value = null;
                    andGroups.Add(andGroup);
                    for (int i = 0; i < andGroup.m_Items.Length; i++)
                    {
                        Serialize(ref andGroup.m_Items[i]);
                    }
                }
                else if (item.m_Filter is OrGroup orGroup)
                {
                    item.m_Index = -(orGroups.Count + 1);
                    item.m_Value = null;
                    orGroups.Add(orGroup);
                    for (int i = 0; i < orGroup.m_Items.Length; i++)
                    {
                        Serialize(ref orGroup.m_Items[i]);
                    }
                }
                else if(item.m_Filter != null)
                {
                    item.m_Value = item.m_Filter;
                }
                else
                {
                    item.m_Index = 0;
                }
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Deserialize(ref m_Item);
            for (int i = 0; i < m_AndGroups.Length; i++)
            {
                for (int j = 0; j < m_AndGroups[i].m_Items.Length; j++)
                {
                    Deserialize(ref m_AndGroups[i].m_Items[j]);
                }
            }

            for (int i = 0; i < m_OrGroups.Length; i++)
            {
                for (int j = 0; j < m_OrGroups[i].m_Items.Length; j++)
                {
                    Deserialize(ref m_OrGroups[i].m_Items[j]);
                }
            }
            
            void Deserialize(ref Item item)
            {
                int index = item.m_Index;
                if (index > 0)
                {
                    item.m_Filter = m_AndGroups[index - 1];
                }
                else if (index < 0)
                {
                    index         = -index;
                    item.m_Filter = m_OrGroups[index - 1];
                }
                else if(item.m_Value is IFilter<T> filter)
                {
                    item.m_Filter = filter;
                }
            }
        }
    }
}