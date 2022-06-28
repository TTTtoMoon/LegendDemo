using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [Description("传送目标至随机位置", "仅对玩家和怪物有效")]
    public sealed class TransferToRandomPosition : TriggerFeatureEffect
    {
        [Description("最大范围半径")] 
        public AbilityVariable MaxRange = new AbilityVariable(5f);

        [Description("最小范围半径")] 
        public AbilityVariable MinRange = new AbilityVariable(2f);

        [Description("起点特效")]
        public GameObject StartVFX;

        [Description("终点特效")]
        public GameObject EndVFX;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0, length = targets.Count; i < length; i++)
            {
                if (targets[i] is Actor actor)
                {
                    Transfer(actor);
                }
            }
        }

        const int   SplitCount = 72;
        const float SplitAngle = 360f / SplitCount;

        private void Transfer(Actor target)
        {
            float min = MinRange;
            float max = MaxRange;

            List<int> randomAngles = ListPool<int>.Get();
            randomAngles.Capacity = Mathf.Max(randomAngles.Capacity, SplitCount);
            for (int i = 0; i < SplitCount; i++)
            {
                randomAngles.Add(i);
            }

            while (randomAngles.Count > 0)
            {
                int randomIndex = Random.Range(0, randomAngles.Count);
                int randomAngle = randomAngles[randomIndex];
                randomAngles.RemoveAt(randomIndex);
                if (TryTransfer(target, randomAngle * SplitAngle, max, min))
                {
                    break;
                }
            }

            ListPool<int>.Release(randomAngles);
        }

        private bool TryTransfer(Actor target, float randomAngle, float max, float min)
        {
            Vector3 randomDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * Vector3.forward;
            if (Physics.Linecast(target.Position, max * randomDirection, out RaycastHit hit) == false ||
                hit.distance                                                                 >= min)
            {
                float   randomDistance = Random.Range(min, max);
                float   offset         = Mathf.Max(randomDistance - min, max - randomDistance);
                float   sampleRadius   = 0.4f /*target.CC.radius*/;
                Vector3 samplePosition = randomDistance * randomDirection + target.Position;
                while (sampleRadius < offset)
                {
                    if (NavMesh.SamplePosition(samplePosition, out NavMeshHit navHit, sampleRadius, target.NavMeshAgent.areaMask))
                    {
                        GameManager.VFXSystem.CreateInstance(StartVFX, target.Position, Quaternion.identity);
                        GameManager.VFXSystem.CreateInstance(EndVFX,   navHit.position, Quaternion.identity);
                        target.NavMeshAgent.Warp(navHit.position);
                        return true;
                    }

                    sampleRadius += target.NavMeshAgent.radius;
                }
            }

            return false;
        }
    }
}
