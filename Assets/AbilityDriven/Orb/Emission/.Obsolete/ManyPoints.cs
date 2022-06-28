using System;
using System.Collections;
using Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("发射法球：在目标位置发射多颗")]
    public sealed class ManyPoints : OrbEmission
    {
        [Description("多点发射时间间隔")] 
        public float TimeSpan;

        [Description("多点坐标")] 
        public Vector3[] Points = new Vector3[0];

        [Description("多点概率(x数量，y概率)")] [TableList] [InfoBox("点数超出了坐标数量，或概率总和不为1。", InfoMessageType.Error, "IsEmitManyError")]
        public CountProbability[] ManyPointCount = new CountProbability[0];

        protected override void Invoke(TargetCollection targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Task.Start(Emit(targets[i]));
            }
        }
        
        private IEnumerator Emit(IAbilityTarget target)
        {
            int   emitPointCount    = 0;
            float totalProbability  = 0f;
            float randomProbability = Random.Range(0f, 1f);
            for (int i = 0; i < ManyPointCount.Length && totalProbability <= randomProbability; i++)
            {
                totalProbability += ManyPointCount[i].Probability;
                emitPointCount   =  ManyPointCount[i].Count;
            }

            Vector3    emitPosition = EmitPosition.GetPosition(target);
            Quaternion emitRotation = target.Rotation;
            int        pointCount   = Points.Length;
            emitPointCount = Mathf.Min(emitPointCount, pointCount);
            for (int i = 0, counter = 0; i < pointCount && counter < emitPointCount; i++)
            {
                if (Random.Range(0f, 1f) > 1f * (emitPointCount - counter) / (pointCount - i)) continue;
                counter++;
                Vector3    position = emitPosition + emitRotation * Points[i];
                Quaternion rotation = emitRotation;
                Emit(target, position, rotation);

                if (TimeSpan > 0f)
                {
                    yield return TimeDefine.GetSeconds(TimeSpan);
                }
            }
        }

        [Serializable]
        public struct CountProbability
        {
            [Range(0, 1)] public float Probability;
            [Min(1)]      public int   Count;
        }
        
#if UNITY_EDITOR
        private bool IsEmitManyError
        {
            get
            {
                if (Points.Length == 0) return false;
                float total = 0f;
                for (int i = 0; i < ManyPointCount.Length; i++)
                {
                    total += ManyPointCount[i].Probability;
                    if(ManyPointCount[i].Count > Points.Length) return true;
                }
                
                return Math.Abs(total - 1f) > Mathf.Epsilon;
            }
        }
#endif
    }
}