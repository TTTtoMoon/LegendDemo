using System;
using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("发射效果器：在目标点直接创建", "会对每个目标都进行一次全量发射。")]
    public sealed class AtPointBurstEmission : OrbEmission
    {
        [Description("中心点偏移")]
        public Vector3 CenterOffset;

        [Description("发射点(即发射数量)")] 
        public Vector3[] Points = new Vector3[] { Vector3.zero };
        
        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Burst(targets[i]);
            }
        }

        private void Burst(IAbilityTarget target)
        {
            Vector3 center = target.Position + CenterOffset;
            for (int i = 0; i < Points.Length; i++)
            {
                Emit(center + Points[i], target.Rotation, target, Vector3.zero);
            }
        }
    }
}