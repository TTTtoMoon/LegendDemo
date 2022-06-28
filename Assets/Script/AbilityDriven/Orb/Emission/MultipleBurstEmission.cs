using System;
using System.Collections;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("发射效果器：多重迸发")]
    public class MultipleBurstEmission : OrbEmission
    {
        [Description("迸发模式")]
        public BurstMode UseMode;
        
        [Description("波次")]
        public Wave[] Waves = new Wave[0];
        
        protected override void Invoke(TargetCollection targets)
        {
            switch (UseMode)
            {
                case BurstMode.WorldPosition:
                    StartEmitTask(EmittingWorldPosition());
                    break;
                case BurstMode.ForeachTarget:
                    for (int i = 0; i < targets.Count; i++)
                    {
                        StartEmitTask(EmittingForeachTarget(targets[i]));
                    }

                    break;
            }

            IEnumerator EmittingWorldPosition()
            {
                for (int i = 0; i < Waves.Length; i++)
                {
                    yield return new WaitForSeconds(Waves[i].Delay);
                    for (int j = 0; j < Waves[i].Positions.Length; j++)
                    {
                        Emit(Waves[i].Positions[j], Quaternion.identity, Ability.Target, Waves[i].Positions[j]);
                    }
                }
            }
            
            IEnumerator EmittingForeachTarget(IAbilityTarget target)
            {
                for (int i = 0, length = Waves.Length; i < length; i++)
                {
                    yield return new WaitForSeconds(Waves[i].Delay);
                    if (target.IsValidAndActive() == false)
                    {
                        yield break;
                    }

                    for (int j = 0; j < Waves[i].Positions.Length; j++)
                    {
                        Vector3 position = target.Position + Waves[i].Positions[j];
                        Emit(position, Quaternion.identity, target, position);
                    }
                }
            }
        }

        [Serializable]
        [InlineProperty]
        public struct Wave
        {
            [Description("延时")] [Min(0f)] 
            public float Delay;

            [Description("点位")] 
            public Vector3[] Positions;
        }

        public enum BurstMode
        {
            [InspectorName("世界坐标")]
            WorldPosition,
            [InspectorName("对每个目标")]
            ForeachTarget,
        }
    }
}