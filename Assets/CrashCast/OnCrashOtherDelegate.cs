using System;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 当碰撞其他目标的委托
    /// </summary>
    public delegate bool OnCrashOtherDelegate(IAbilityTarget target);

    /// <summary>
    /// 当碰撞其他目标
    /// </summary>
    public sealed class OnCrash : AbstractNiceDelegate<OnCrashOtherDelegate>
    {
        public bool Invoke(IAbilityTarget target)
        {
            BeginInvoke();
            bool result = false;
            foreach (PriorityNode<OnCrashOtherDelegate> actionNode in m_Delegates)
            {
                OnCrashOtherDelegate action = actionNode.Item;
                if (action == null) continue;
                try
                {
                    result |= action.Invoke(target);
                }
                catch (Exception ex)
                {
                    Debugger.LogException(new NiceDelegateException(action.Target.GetType().Name, action.Method.Name, ex));
                }
            }

            EndInvoke();
            return result;
        }
    }
}