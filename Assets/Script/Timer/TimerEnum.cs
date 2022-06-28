namespace RogueGods.Gameplay
{
    public enum TimerState
    {
        /// <summary>
        /// 待机中
        /// </summary>
        Awaiting,

        /// <summary>
        /// 计时中
        /// </summary>
        Running,

        /// <summary>
        /// 暂停中
        /// </summary>
        Paused,
    }

    public enum TimerUpdater
    {
        Update,
        LateUpdate,
        FixedUpdate,
    }
}