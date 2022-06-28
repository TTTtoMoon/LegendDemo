using System;
using System.Collections.Generic;

namespace Abilities
{
    /// <summary>
    /// 目标收集器
    /// </summary>
    public readonly struct TargetCollector : IDisposable
    {
        #region Pool

        private static Stack<List<IAbilityTarget>> s_Stack = new Stack<List<IAbilityTarget>>();

        private static List<IAbilityTarget> Get()
        {
            return s_Stack.Count > 0 ? s_Stack.Pop() : new List<IAbilityTarget>();
        }

        private static void Release(List<IAbilityTarget> list)
        {
            list.Clear();
            s_Stack.Push(list);
        }

        #endregion

        public static TargetCollector Collect<TArg>(
            List<IAbilityTarget> cacheTargetList,
            Ability              ability,
            FeatureTarget[]      targets,
            IAbilityTarget       triggerTarget,
            in TArg              arg)
            where TArg : struct, IFeatureTriggerArg
        {
            TargetCollector collector = new TargetCollector(cacheTargetList);

            foreach (FeatureTarget target in targets)
            {
                IAbilityTarget origin = target.Origin switch
                {
                    FeatureTargetOrigin.Owner          => ability.Owner,
                    FeatureTargetOrigin.Source         => ability.Source,
                    FeatureTargetOrigin.TriggerTarget  => triggerTarget,
                    FeatureTargetOrigin.SearchedTarget => ability.Target,
                    _                                  => null,
                };

                if (origin.IsNull() == false)
                {
                    if (arg is EmptyArg == false && target is FeatureTarget<TArg> targetWithArg)
                    {
                        targetWithArg.Collect(origin, arg, collector);
                    }
                    else
                    {
                        target.Collect(origin, collector);
                    }
                }
            }

            return collector;
        }

        private TargetCollector(List<IAbilityTarget> cacheTargetList)
        {
            m_CacheTargetList = cacheTargetList;
            NewTargetList     = Get();
            OldTargetList     = Get();
            MissingTargetList = Get();

            MissingTargetList.AddRange(cacheTargetList);
        }

        private readonly  List<IAbilityTarget> m_CacheTargetList;
        internal readonly List<IAbilityTarget> NewTargetList;
        internal readonly List<IAbilityTarget> OldTargetList;
        internal readonly List<IAbilityTarget> MissingTargetList;

        public int Count => NewTargetList.Count + OldTargetList.Count;

        void IDisposable.Dispose()
        {
            m_CacheTargetList.Clear();
            m_CacheTargetList.AddRange(OldTargetList);
            m_CacheTargetList.AddRange(NewTargetList);
            Release(MissingTargetList);
            Release(OldTargetList);
            Release(NewTargetList);
        }

        /// <summary>
        /// 添加目标
        /// </summary>
        /// <param name="target">目标实例</param>
        /// <exception cref="ArgumentNullException">target不能为空或已销毁的unity对象</exception>
        public void Append(IAbilityTarget target)
        {
            if (target.IsNull()) throw new ArgumentNullException(nameof(target));

            if (OldTargetList.Contains(target) || NewTargetList.Contains(target))
            {
                return;
            }

            for (int i = 0; i < MissingTargetList.Count; i++)
            {
                if (ReferenceEquals(MissingTargetList[i], target))
                {
                    OldTargetList.Add(target);
                    MissingTargetList.RemoveAt(i);
                    return;
                }
            }

            NewTargetList.Add(target);
        }

        /// <summary>
        /// 是否为上次就已经存在的目标
        /// </summary>
        public bool IsOldTarget(IAbilityTarget target)
        {
            return m_CacheTargetList.Contains(target);
        }
    }
}