namespace Abilities
{
    public enum BlackboardDataChangeType
    {
        /// <summary>
        /// 新增
        /// </summary>
        JustAdd,
        /// <summary>
        /// 修改
        /// </summary>
        Modify,
        /// <summary>
        /// 移除
        /// </summary>
        WantToRemove,
    }

    public delegate void BlackboardDataChangeDelegate<TData>(BlackboardDataChangeType changeType, TData data) where TData : BlackboardData, new();

    public abstract class BlackboardDataChangeListener
    {
        public abstract bool IsEmpty { get; }
        public abstract void Invoke(BlackboardDataChangeType changeType, BlackboardData data);
        public abstract void Clear();
    }

    public sealed class BlackboardDataChangeListener<TData> : BlackboardDataChangeListener where TData : BlackboardData, new()
    {
        private BlackboardDataChangeDelegate<TData> m_Listener;

        public override bool IsEmpty => m_Listener == null;

        public BlackboardDataChangeListener(BlackboardDataChangeDelegate<TData> initListener = null)
        {
            m_Listener = initListener;
        }

        public override void Invoke(BlackboardDataChangeType changeType, BlackboardData data)
        {
            if (data is TData tData)
            {
                m_Listener?.Invoke(changeType, tData);
            }
        }

        public override void Clear()
        {
            m_Listener = null;
        }

        public void Add(BlackboardDataChangeDelegate<TData> listener)
        {
            m_Listener += listener;
        }

        public void Remove(BlackboardDataChangeDelegate<TData> listener)
        {
            m_Listener -= listener;
        }
    }
}