using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public partial class Timer
    {
        public const float MinDuration = 1f / 30f;
        public const float MinInterval = 1f / 30f;

        private float                  m_PreviousTime;
        private float                  m_Time;
        private float                  m_TimeScale;
        private int                    m_IntervalTickCounter;
        private float                  m_IntervalTicker;
        private float                  m_Duration;
        private TimerUpdater           m_Updater;
        private TimerState             m_State;
        private ITimerUpdateListener   m_UpdateListener;
        private ITimerIntervalListener m_IntervalListener;
        private ITimerEndListener      m_EndListener;

        /// <summary>
        /// 创建计时器
        /// </summary>
        /// <param name="listener">监听者</param>
        /// <param name="duration">时长</param>
        /// <param name="timeScale">时间缩放</param>
        /// <param name="updater">更新方式，默认Update</param>
        public Timer(ITimerListener listener, TimerUpdater updater = TimerUpdater.Update, float duration = MinDuration, float timeScale = 1f)
        {
            m_PreviousTime        = -1f;
            m_Time                = 0f;
            m_TimeScale           = timeScale;
            m_IntervalTickCounter = 0;
            m_IntervalTicker      = 0f;
            m_Duration            = duration < 0f ? duration : Mathf.Max(MinDuration, duration);
            m_Updater             = updater;
            m_State               = TimerState.Awaiting;

            if (listener is ITimerUpdateListener updateListener)
            {
                m_UpdateListener = updateListener;
            }

            if (listener is ITimerIntervalListener intervalListener)
            {
                m_IntervalListener = intervalListener;
            }

            if (listener is ITimerEndListener endListener)
            {
                m_EndListener = endListener;
            }
        }

        #region API

        /// <summary>
        /// 当前时间
        /// </summary>
        public float Time => m_Time;

        /// <summary>
        /// 当前归一化时间
        /// </summary>
        public float NormalizedTime => m_Time / m_Duration;

        /// <summary>
        /// 作为IntervalTicker时的Tick次数
        /// </summary>
        public int TickCounter => m_IntervalTickCounter;

        /// <summary>
        /// 时长
        /// </summary>
        public float Duration
        {
            get => m_Duration;
            set => m_Duration = value < 0 ? value : Mathf.Max(MinDuration, value);
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public TimerState State => m_State;

        /// <summary>
        /// 是否在暂停中
        /// </summary>
        public bool IsAwaiting => m_State == TimerState.Awaiting;

        /// <summary>
        /// 当前是否正在计时
        /// </summary>
        public bool IsRunning => m_State == TimerState.Running;

        /// <summary>
        /// 是否在暂停中
        /// </summary>
        public bool IsPausing => m_State == TimerState.Paused;

        /// <summary>
        /// 开始计时器
        /// </summary>
        /// <param name="duration">覆写时长</param>
        /// <param name="timeScale">覆写时间缩放</param>
        public void Start(float? duration = null, float? timeScale = null)
        {
            if (m_State != TimerState.Awaiting)
            {
                Debugger.LogError("计时器已经启动，无法再次启动，若想重新开始请调用ReStart");
                return;
            }

            ReStart(duration, timeScale);
        }

        /// <summary>
        /// 重新开始计时器
        /// </summary>
        /// <param name="duration">覆写时长</param>
        /// <param name="timeScale">覆写时间缩放</param>
        public void ReStart(float? duration = null, float? timeScale = null)
        {
            Stop(false);
            if (duration  != null) Duration    = duration.Value;
            if (timeScale != null) m_TimeScale = Mathf.Max(MinDuration, timeScale.Value);
            m_State          = TimerState.Running;
            m_PreviousTime   = -1f;
            m_Time           = 0f;
            m_IntervalTicker = 0f;
            switch (m_Updater)
            {
                case TimerUpdater.Update:
                    m_UpdateTimer.Add(this);
                    break;
                case TimerUpdater.LateUpdate:
                    m_FixedUpdateTimer.Add(this);
                    break;
                case TimerUpdater.FixedUpdate:
                    m_LateUpdateTimer.Add(this);
                    break;
            }

            // 开始时若不触发，则直接将定时器调至下一个时刻
            if (m_IntervalListener                  != null &&
                m_IntervalListener.TriggerWhenStart == false)
            {
                m_IntervalTicker += m_IntervalListener.Interval;
            }
        }

        /// <summary>
        /// 暂停(仅Running时有效)
        /// </summary>
        public void Pause()
        {
            if (m_State != TimerState.Running) return;
            m_State = TimerState.Paused;
        }

        /// <summary>
        /// 继续(仅Paused时有效)
        /// </summary>
        public void Resume()
        {
            if (m_State != TimerState.Paused) return;
            m_State = TimerState.Running;
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="triggerEndEvent">是否触发结束事件</param>
        public void Stop(bool triggerEndEvent)
        {
            if (m_State == TimerState.Awaiting) return;

            m_State = TimerState.Awaiting;
            switch (m_Updater)
            {
                case TimerUpdater.Update:
                    m_UpdateTimer.Remove(this);
                    break;
                case TimerUpdater.LateUpdate:
                    m_FixedUpdateTimer.Remove(this);
                    break;
                case TimerUpdater.FixedUpdate:
                    m_LateUpdateTimer.Remove(this);
                    break;
            }

            if (triggerEndEvent)
            {
                m_EndListener?.OnEnd();
            }

            m_PreviousTime   = -1f;
            m_Time           = 0f;
            m_IntervalTicker = 0f;
        }

        /// <summary>
        /// 当前帧是否经历了时间点time
        /// </summary>
        /// <param name="time">时间点 单位秒</param>
        public bool AtTime(float time)
        {
            return m_PreviousTime < time && m_Time >= time;
        }

        #endregion

        #region Update

        private void Update(float deltaTime)
        {
            if (m_State != TimerState.Running) return;

            m_Time += deltaTime * m_TimeScale;
            IntervalUpdate();
            m_UpdateListener?.OnUpdate();
            m_PreviousTime = m_Time;
            if (m_Time >= m_Duration && m_Duration > 0f)
            {
                Stop(true);
            }
        }

        private void IntervalUpdate()
        {
            float interval;
            if (m_IntervalListener                       != null &&
                (interval = m_IntervalListener.Interval) > MinInterval)
            {
                while (m_IntervalTicker <= m_Time)
                {
                    m_IntervalTicker += interval;
                    m_IntervalTickCounter++;
                    m_IntervalListener.OnInterval();
                }
            }
        }

        #endregion
    }
}