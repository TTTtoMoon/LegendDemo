using System.Collections.Generic;

namespace Abilities
{
    /// <summary>
    /// 公共数据黑板
    /// </summary>
    public sealed class Blackboard
    {
        internal readonly Ability Ability;

        private List<BlackboardData> m_DataList = new List<BlackboardData>();

        internal Blackboard(Ability ability)
        {
            Ability = ability;
        }

        /// <summary>
        /// 新增数据变量监听
        /// </summary>
        /// <param name="listener"></param>
        /// <typeparam name="TData"></typeparam>
        public void AddDataChangeListener<TData>(BlackboardDataChangeDelegate<TData> listener)
            where TData : BlackboardData, new()
        {
            for (int i = 0, length = m_DataList.Count; i < length; i++)
            {
                if (m_DataList[i].m_Listener is BlackboardDataChangeListener<TData> cache)
                {
                    cache.Add(listener);
                    return;
                }
            }

            m_DataList.Add(new TData()
            {
                m_Blackboard       = this,
                m_ReferenceCounter = 0,
                m_Listener         = new BlackboardDataChangeListener<TData>(listener),
            });
        }

        /// <summary>
        /// 移除数据变化监听
        /// </summary>
        /// <param name="listener"></param>
        /// <typeparam name="TData"></typeparam>
        public void RemoveDataChangeListener<TData>(BlackboardDataChangeDelegate<TData> listener)
            where TData : BlackboardData, new()
        {
            for (int i = 0, length = m_DataList.Count; i < length; i++)
            {
                if (m_DataList[i].m_Listener is BlackboardDataChangeListener<TData> cache)
                {
                    cache.Remove(listener);
                    return;
                }
            }
        }

        /// <summary>
        /// 创建数据，同一类型的数据有且仅有一份，再次创建只会增加其引用计数
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData Create<TData>() where TData : BlackboardData, new()
        {
            for (int i = 0, length = m_DataList.Count; i < length; i++)
            {
                if (m_DataList[i] is TData result)
                {
                    if (result.m_ReferenceCounter == 0)
                    {
                        result.OnCreate();
                    }

                    result.m_ReferenceCounter++;
                    return result;
                }
            }

            TData data = new TData()
            {
                m_Blackboard       = this,
                m_ReferenceCounter = 1,
                m_Listener         = new BlackboardDataChangeListener<TData>(),
            };
            data.OnCreate();
            m_DataList.Add(data);
            return data;
        }

        /// <summary>
        /// 销毁数据，当数据引用计数为0时才会真正销毁
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        public void Destroy<TData>() where TData : BlackboardData, new()
        {
            for (int i = 0, length = m_DataList.Count; i < length; i++)
            {
                if (m_DataList[i] is TData result)
                {
                    result.m_ReferenceCounter--;
                    if (result.m_ReferenceCounter <= 0)
                    {
                        result.OnDestroy();
                        m_DataList.RemoveAt(i);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 尝试获取数据，不会增加引用计数
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public bool TryGet<TData>(out TData data) where TData : BlackboardData, new()
        {
            for (int i = 0, length = m_DataList.Count; i < length; i++)
            {
                if (m_DataList[i] is TData result && result.m_ReferenceCounter > 0)
                {
                    data = result;
                    return true;
                }
            }

            data = default;
            return false;
        }

        /// <summary>
        /// 尝试获取数据，不会增加引用计数
        /// </summary>
        public TData Get<TData>() where TData : BlackboardData, new()
        {
            TryGet(out TData data);
            return data;
        }
        
        /// <summary>
        /// 尝试获取数据，不存在则创建
        /// </summary>
        public TData GetOrCreate<TData>() where TData : BlackboardData, new()
        {
            return TryGet(out TData data) ? data : Create<TData>();
        }

        /// <summary>
        /// 情况数据
        /// </summary>
        public void Clear()
        {
            for (int i = 0, length = m_DataList.Count; i < length; i++)
            {
                if (m_DataList[i].m_ReferenceCounter > 0)
                    m_DataList[i].OnDestroy();
            }

            m_DataList.Clear();
        }
    }
}