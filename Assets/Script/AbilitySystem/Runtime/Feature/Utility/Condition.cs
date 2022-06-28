using System;
using System.Reflection;
using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 条件
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// 验证条件
        /// </summary>
        /// <param name="ability">所属能力</param>
        /// <returns>返回是否满足过条件</returns>
        bool Verify(in Ability ability);
    }

    /// <summary>
    /// 条件
    /// </summary>
    [Serializable]
    public sealed class Condition
    {
        [Serializable]
        [Description("并且组", "组员之间为并且关系")]
        public class AndGroup : ICondition
        {
            [SerializeField] internal Item[] m_Items = new Item[0];

            public bool Verify(in Ability ability)
            {
                for (int i = 0, length = m_Items.Length; i < length; i++)
                {
                    if (m_Items[i].Verify(ability) == false)
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
        public class OrGroup : ICondition
        {
            [SerializeField] internal Item[] m_Items = new Item[0];

            public bool Verify(in Ability ability)
            {
                for (int i = 0, length = m_Items.Length; i < length; i++)
                {
                    if (m_Items[i].Verify(ability))
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
            [SerializeReference] internal ICondition m_Value;
            [SerializeField]     internal bool       m_Reverse;

            public Item(ICondition value)
            {
                m_Value   = value;
                m_Reverse = false;
            }

            public bool Verify(in Ability ability)
            {
                bool isOk = m_Value == null || m_Value.Verify(ability);
                if (m_Reverse)
                {
                    isOk = !isOk;
                }

                return isOk;
            }

            public override string ToString()
            {
                if (m_Value == null)
                {
                    return "无条件限制";
                }

                string str;
                switch (m_Value)
                {
                    case AndGroup andGroup:
                    case OrGroup orGroup:
                        str = m_Value.ToString();
                        break;
                    default:
                        str = m_Value.GetType().GetCustomAttribute<DescriptionAttribute>()?.Name ?? m_Value.GetType().Name;
                        break;
                }

                if (m_Reverse)
                {
                    str = $"!{str}";
                }

                return str;
            }
        }

        [SerializeField] private Item m_Item;

        // To SerializeReference
        [SerializeField] [SerializeReference] private AndGroup[] m_AndGroups = new AndGroup[0];
        [SerializeField] [SerializeReference] private OrGroup[]  m_OrGroups  = new OrGroup[0];

        /// <summary>
        /// 验证条件
        /// </summary>
        /// <param name="ability">所属能力</param>
        /// <returns>返回是否满足过条件</returns>
        public bool Verify(in Ability ability)
        {
            return m_Item.Verify(ability);
        }

        public override string ToString()
        {
            return m_Item.ToString();
        }
    }
}