using System;
using System.Collections;
using Abilities;
using RogueGods.Utility;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("发射效果器：多点迸发")]
    public class ManyPointsBurstEmission : OrbEmission
    {
        [Description("随机范围半径")]
        public Vector2 AreaRadius;

        [Description("发射数量")]
        public Vector2Int EmitCount;

        [Description("发射间隔时间")]
        public Vector2 EmitTimeSpan;

        protected override void Invoke(TargetCollection targets)
        {
            StartEmitTask(Bursting());
        }
        
        IEnumerator Bursting()
        {
            int emitCount = Random.Range(EmitCount.x, EmitCount.y + 1);
            NavMeshQueryFilter filter = new NavMeshQueryFilter()
            {
                areaMask = -1,
            };
            for (int i = 0; i < emitCount; i++)
            {
                Vector3 position = Utilities.RandomNavPositionInRing(Ability.Owner.Position, AreaRadius.x, AreaRadius.y);
                Emit(position, Quaternion.identity, null, position);
                if (EmitTimeSpan.x > 0 || EmitTimeSpan.y > 0)
                {
                    float timeSpan = Random.Range(EmitTimeSpan.x, EmitTimeSpan.y);
                    yield return new WaitForSeconds(timeSpan);
                }
            }
        }
    }
}