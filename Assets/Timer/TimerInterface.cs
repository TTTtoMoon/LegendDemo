namespace RogueGods.Gameplay
{
    public interface ITimerListener
    {
    }

    public interface ITimerUpdateListener : ITimerListener
    {
        /// <summary>
        /// 计时器更新事件
        /// </summary>
        void OnUpdate();
    }

    public interface ITimerIntervalListener : ITimerListener
    {
        /// <summary>
        /// 间隔时间单位秒
        /// </summary>
        float Interval { get; }

        /// <summary>
        /// 是否在开始时触发
        /// </summary>
        bool TriggerWhenStart { get; }

        /// <summary>
        /// 周期性事件
        /// </summary>
        void OnInterval();
    }

    public interface ITimerEndListener : ITimerListener
    {
        /// <summary>
        /// 计时器结束事件
        /// </summary>
        void OnEnd();
    }
}