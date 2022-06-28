using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.AbilityDriven.TriggerEffect;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("发射效果器：玩家普攻式")]
    public sealed class ShotEmission : OrbEmission
    {
        private static Quaternion Forward  = Quaternion.LookRotation(Vector3.forward);
        private static Quaternion Backward = Quaternion.LookRotation(Vector3.back);
        private static Quaternion Left     = Quaternion.LookRotation(Vector3.left);
        private static Quaternion Right    = Quaternion.LookRotation(Vector3.right);
        
        [Description("前向箭增量", "加1表示前方加一根箭")]
        public AbilityVariable ForwardCount = new AbilityVariable(0);
        
        [Description("斜向箭增量", "加1表示左右各加一根箭")]
        public AbilityVariable SlantCount = new AbilityVariable(0);
        
        [Description("侧向箭增量", "加1表示左右各加一根箭")]
        public AbilityVariable LateralCount = new AbilityVariable(0);
        
        [Description("后向箭增量", "加1表示后方加一根箭")]
        public AbilityVariable BackwardCount = new AbilityVariable(0);

        [Description("多重数量")]
        public AbilityVariable MultipleCount = new AbilityVariable(0);

        [Description("发射点高度偏移(米)")]
        public float EmitOffsetY;
        
        [Description("发射点距离偏移(米)")]
        public float EmitOffsetZ;
        
        [Description("并列间距(米)")]
        public float AbreastOffset = 0.2f;
        
        [Description("斜向角度(度)")]
        public float SlantAngle = 120f;

        [Description("多重间隔(秒)")]
        public float WaveTimeSpan = 0.2f;

        protected override void Invoke(TargetCollection targets)
        {
            int multipleCount = 1 + MultipleCount.IntValue;
            if (multipleCount > 1)
            {
                StartEmitTask(ShotMultipleWave());

                IEnumerator ShotMultipleWave()
                {
                    for (int i = 0; i < multipleCount; i++)
                    {
                        ShotWave();
                        if (WaveTimeSpan > 0f)
                        {
                            yield return new WaitForSeconds(WaveTimeSpan);
                        }
                    }
                }
            }
            else
            {
                ShotWave();
            }
        }

        public override void PreviewTrajectory(List<PreviewData> previewList)
        {
            ShotWave(previewList);
        }

        /// <summary>
        /// 发射一波
        /// </summary>
        private void ShotWave(List<PreviewData> previewList = null)
        {
            ForwardShot(previewList);
            SlantShot(previewList);
            LateralShot(previewList);
            BackwardShot(previewList);
        }

        /// <summary>
        /// 发射一支
        /// </summary>
        /// <param name="position">相对于拥有者的起始点位置</param>
        /// <param name="rotation">相对于拥有者的朝向</param>
        /// <param name="previewList">预览列表</param>
        private void ShotArrow(Vector3 position, Quaternion rotation, List<PreviewData> previewList)
        {
            Vector3    shotPosition = Ability.Owner.Position;
            Quaternion shotRotation = Ability.Owner.Rotation;
            shotPosition = shotPosition + shotRotation * position;
            shotRotation = shotRotation * rotation;
            if (previewList == null)
            {
                Emit(shotPosition, shotRotation, null, Vector3.zero);
            }
            else
            {
                previewList.Add(new PreviewData(shotPosition, shotRotation));
            }
        }

        /// <summary>
        /// 前向发射
        /// </summary>
        private void ForwardShot(List<PreviewData> previewList)
        {
            int   shotCount = 1 + ForwardCount.IntValue;
            float width     = (shotCount - 1) * AbreastOffset;

            Vector3 shotPosition = new Vector3(0f, EmitOffsetY, EmitOffsetZ);
            shotPosition.x = -width / 2f;
            for (int i = 0; i < shotCount; i++)
            {
                ShotArrow(shotPosition, Forward, previewList);
                shotPosition.x += AbreastOffset;
            }
        }

        /// <summary>
        /// 斜向发射
        /// </summary>
        private void SlantShot(List<PreviewData> previewList)
        {
            int shotCount = SlantCount.IntValue;
            if (shotCount <= 0) return;

            float angleOffset = SlantAngle / shotCount;

            float leftAngle = -SlantAngle;
            for (int i = 0; i < shotCount; i++)
            {
                Quaternion shotDirection = Quaternion.Euler(0f, leftAngle, 0f);
                ShotArrow(shotDirection * new Vector3(0f, EmitOffsetY, EmitOffsetZ), shotDirection, previewList);
                leftAngle += angleOffset;
            }
            
            float rightAngle = SlantAngle;
            for (int i = 0; i < shotCount; i++)
            {
                Quaternion shotDirection = Quaternion.Euler(0f, rightAngle, 0f);
                ShotArrow(shotDirection * new Vector3(0f, EmitOffsetY, EmitOffsetZ), shotDirection, previewList);
                rightAngle -= angleOffset;
            }
        }

        /// <summary>
        /// 侧向发射
        /// </summary>
        private void LateralShot(List<PreviewData> previewList)
        {
            int shotCount = LateralCount.IntValue;
            if (shotCount <= 0) return;

            float width = (shotCount - 1) * AbreastOffset;

            Vector3 leftShotPosition = new Vector3(-EmitOffsetZ, EmitOffsetY, 0f);
            leftShotPosition.z = -width / 2f;
            for (int i = 0; i < shotCount; i++)
            {
                ShotArrow(leftShotPosition, Left, previewList);
                leftShotPosition.z += AbreastOffset;
            }

            Vector3 rightShotPosition = new Vector3(EmitOffsetZ, EmitOffsetY, 0f);
            rightShotPosition.z = -width / 2f;
            for (int i = 0; i < shotCount; i++)
            {
                ShotArrow(rightShotPosition, Right, previewList);
                rightShotPosition.z += AbreastOffset;
            }
        }

        /// <summary>
        /// 后向发射
        /// </summary>
        private void BackwardShot(List<PreviewData> previewList)
        {
            int shotCount = BackwardCount.IntValue;
            if (shotCount <= 0) return;

            float width = (shotCount - 1) * AbreastOffset;

            Vector3 shotPosition = new Vector3(0f, EmitOffsetY, -EmitOffsetZ);
            shotPosition.x = -width / 2f;
            for (int i = 0; i < shotCount; i++)
            {
                ShotArrow(shotPosition, Backward, previewList);
                shotPosition.x += AbreastOffset;
            }
        }
    }
}