using System;
using UnityEngine;
/*#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics;
#endif*/

namespace RogueGods.Gameplay
{
    public sealed class ActorTagStack
    {
        // === Built In Combination ===

        /// <summary>
        /// 出生时 透明、无法移动、沉默
        /// </summary>
        public const ActorTag Birth = ActorTag.Transparency | ActorTag.Unmovable | ActorTag.SilenceAbility | ActorTag.CantCrash;

        /// <summary>
        /// 死亡时 透明、无法移动、沉默
        /// </summary>
        public const ActorTag Death = ActorTag.Transparency | ActorTag.Unmovable | ActorTag.SilenceAbility | ActorTag.CantCrash;

        /// <summary>
        /// 重生时 透明、无法移动、沉默
        /// </summary>
        public const ActorTag Relive = ActorTag.Transparency | ActorTag.Unmovable | ActorTag.SilenceAbility | ActorTag.CantCrash;

        private const int TagMaxCount = 32;

/*#if UNITY_EDITOR
        public List<KeyValuePair<string, string>> Stack = new List<KeyValuePair<string, string>>();

        private void AddStack(string action)
        {
            StackTrace stackTrace = new StackTrace();
            Stack.Add(new KeyValuePair<string, string>($"After {action} => {(ActorTag)this}", stackTrace.ToString()));
        }
#endif*/

        private byte[] m_TagCounter = new byte[TagMaxCount];

        public bool HasTag(ActorTag tag)
        {
            int flag = (int)tag;
            for (int i = 0; i < TagMaxCount; i++)
            {
                int temp = 1 << i;
                if ((flag & temp) == temp && m_TagCounter[i] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasAnyTag(ActorTag tag)
        {
            int flag = (int)tag;
            for (int i = 0; i < TagMaxCount; i++)
            {
                int temp = 1 << i;
                if ((flag & temp) == temp && m_TagCounter[i] > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddTag(ActorTag tag)
        {
            int flag = (int)tag;
            for (int i = 0; i < TagMaxCount; i++)
            {
                int temp = 1 << i;
                if ((flag & temp) == temp && m_TagCounter[i] < byte.MaxValue)
                {
                    m_TagCounter[i]++;
                }
            }

/*#if UNITY_EDITOR
            AddStack("AddTag");
#endif*/
        }

        public void RemoveTag(ActorTag tag)
        {
            int flag = (int)tag;
            for (int i = 0; i < TagMaxCount; i++)
            {
                int temp = 1 << i;
                if ((flag & temp) == temp && m_TagCounter[i] > 0)
                {
                    m_TagCounter[i]--;
                }
            }

/*#if UNITY_EDITOR
            AddStack("RemoveTag");
#endif*/
        }

        public void Clear()
        {
            for (int i = 0; i < TagMaxCount; i++)
            {
                m_TagCounter[i] = 0;
            }

/*#if UNITY_EDITOR
            Stack.Clear();
#endif*/
        }

        public static implicit operator ActorTag(ActorTagStack actorTagStack)
        {
            if (actorTagStack == null) return ActorTag.None;
            int flag = 0;
            for (int i = 0; i < TagMaxCount; i++)
            {
                if (actorTagStack.m_TagCounter[i] > 0)
                {
                    flag |= 1 << i;
                }
            }

            return (ActorTag)flag;
        }

        public override string ToString()
        {
            return ((ActorTag)this).ToString();
        }
    }

    /// <summary>
    /// 角色Tag
    /// 请勿删除任何Tag，只能新增
    /// </summary>
    [Flags]
    public enum ActorTag
    {
        [InspectorName("无")] 
        None = 0,

        [InspectorName("无敌")] 
        Unbeatable = 1 << 0,

        [InspectorName("无法移动")] 
        Unmovable = 1 << 1,

        [InspectorName("沉默(无法使用技能)")] 
        SilenceAbility = 1 << 2,

        [InspectorName("无法造成碰撞伤害")] 
        CantCrash = 1 << 3,

        [InspectorName("透明")] 
        Transparency = 1 << 4,

        [InspectorName("无")] 
        All = ~None,
    }
}