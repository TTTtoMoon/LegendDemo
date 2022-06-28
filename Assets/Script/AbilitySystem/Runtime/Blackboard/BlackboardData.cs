namespace Abilities
{
    /// <summary>
    /// 黑板数据
    /// </summary>
    public abstract class BlackboardData
    {
        internal Blackboard                   m_Blackboard;
        internal int                          m_ReferenceCounter;
        internal BlackboardDataChangeListener m_Listener;

        protected Ability Ability => m_Blackboard?.Ability;
        
        internal void OnCreate()
        {
            m_Listener.Invoke(BlackboardDataChangeType.JustAdd, this);
        }

        internal void OnDestroy()
        {
            m_Listener.Invoke(BlackboardDataChangeType.WantToRemove, this);
            m_Listener.Clear();
            m_ReferenceCounter = 0;
            m_Listener         = null;
        }

        /// <summary>
        /// 应用变化
        /// </summary>
        protected void ApplyChange()
        {
            m_Listener.Invoke(BlackboardDataChangeType.Modify, this);
        }
    }

    public static class BlackboardExtension
    {
        /// <summary>
        /// 新增数据变量监听
        /// </summary>
        public static void AddDataChangeListener<TData>(this TData data, BlackboardDataChangeDelegate<TData> listener)
            where TData : BlackboardData, new()
        {
            data.m_Blackboard.AddDataChangeListener(listener);
        }

        /// <summary>
        /// 移除数据变化监听
        /// </summary>
        public static void RemoveDataChangeListener<TData>(this TData data, BlackboardDataChangeDelegate<TData> listener)
            where TData : BlackboardData, new()
        {
            data.m_Blackboard.RemoveDataChangeListener(listener);
        }
    }
}