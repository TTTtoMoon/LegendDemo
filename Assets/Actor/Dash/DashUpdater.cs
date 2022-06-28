using System.Collections.Generic;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public partial class DashAgent
    {
        private static List<DashAgent> m_DashAgents = new List<DashAgent>();
        private static List<DashAgent> m_Temp       = new List<DashAgent>();

        static DashAgent()
        {
            GameManager.Instance.RegisterUpdate(Update, order: GameManager.UpdateMonoOrder.Pre);
        }

        public static void Begin(
            Actor          actor,
            Vector3        direction,
            float          distance,
            float          duration)
        {
            int       id    = actor.GetInstanceID();
            DashAgent agent = m_DashAgents.Find(x => x.m_ID == id) ?? ObjectPool<DashAgent>.Get();
            agent.Initialize(id, actor, direction, distance, duration);
        }

        public static void ForceStop(Actor actor)
        {
            int       id    = actor.GetInstanceID();
            DashAgent agent = m_DashAgents.Find(x => x.m_ID == id);
            agent?.Stop();
        }

        public static void Update()
        {
            m_Temp.Clear();
            m_Temp.AddRange(m_DashAgents);
            for (int i = m_Temp.Count - 1; i >= 0; i--)
            {
                m_Temp[i].UpdatePosition();
            }
        }
    }
}