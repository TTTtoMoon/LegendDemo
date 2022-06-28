using System;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 效果器撞墙处理
    /// </summary>
    [Serializable]
    public abstract class OrbWhenHitWall
    {
        public Orb Orb { get; private set; }

        public void Enable(Orb orb)
        {
            if (Orb != null) return;
            Orb = orb;
            OnEnable();
        }

        public void Disable()
        {
            if (Orb == null) return;
            OnDisable();
            Orb = null;
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();


        public abstract void Handle(Orb orb, RaycastHit hit);
        
        public OrbWhenHitWall Clone()
        {
            return (OrbWhenHitWall)MemberwiseClone();
        }
    }
}