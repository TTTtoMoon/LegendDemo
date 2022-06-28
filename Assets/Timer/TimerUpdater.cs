using System.Collections.Generic;
using RogueGods.Utility;

namespace RogueGods.Gameplay
{
    partial class Timer
    {
        private static readonly List<Timer> m_Temp             = new List<Timer>();
        private static readonly List<Timer> m_UpdateTimer      = new List<Timer>();
        private static readonly List<Timer> m_FixedUpdateTimer = new List<Timer>();
        private static readonly List<Timer> m_LateUpdateTimer  = new List<Timer>();

        static Timer()
        {
            GameManager.Instance.RegisterUpdate(Update);
            GameManager.Instance.RegisterFixedUpdate(FixedUpdate);
            GameManager.Instance.RegisterLateUpdate(LateUpdate);
        }

        private static void Update()
        {
            Update(m_UpdateTimer, UnityEngine.Time.deltaTime);
        }

        private static void FixedUpdate()
        {
            Update(m_FixedUpdateTimer, UnityEngine.Time.fixedDeltaTime);
        }

        private static void LateUpdate()
        {
            Update(m_LateUpdateTimer, UnityEngine.Time.deltaTime);
        }

        private static void Update(List<Timer> timers, float deltaTime)
        {
            m_Temp.AddRange(timers);
            for (int i = 0, length = m_Temp.Count; i < length; i++)
            {
                m_Temp[i].Update(deltaTime);
            }

            m_Temp.Clear();
        }
    }
}