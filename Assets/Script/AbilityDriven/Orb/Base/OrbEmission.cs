using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.AbilityDriven.TriggerEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 效果器发射器
    /// </summary>
    [Serializable]
    public abstract class OrbEmission : TriggerFeatureEffect
    {
        [Description("效果器缩放")]
        public AbilityVariable OrbScale = new AbilityVariable(1f);

        [Description("创建时立即碰撞检测？")]
        public bool CastImmediately = true;
        
        [Description("切换场景时停止？")]
        public bool DestroyWhenChangeScene = true;
        
        [Description("技能失效时停止发射?")]
        public bool InterruptEmitWhenDisable;

        [Description("技能失效时销毁已创建的效果器?")]
        public bool DestroyOrbWhenDisable;
        
        [Description("效果器长度")]
        public float OrbLength;

        [Description("延迟移动？")]
        public bool DelayMove;

        [Description("延迟移动时跟随创建者？")] [ShowIf("DelayMove")]
        public bool FollowDuringDelayTime;
        
        [Description("延迟多久移动")] [ShowIf("DelayMove")]
        public float DelayMoveTime;

        [Description("效果器模式")] [SerializeReference] [ReferenceDropdown]
        public OrbMode OrbMode = new StaticMode();

        [Description("效果器撞墙处理")] [SerializeReference] [ReferenceDropdown]
        public OrbWhenHitWall WhenHitWall = new DoNothingWhenHitWall();

        [Description("目标筛选")] [PropertyOrder(99f)]
        public Filter<IAbilityTarget> TargetFilter = new Filter<IAbilityTarget>();

        [Description("效果器能力")] [PropertyOrder(100f)]
        public AbilityReference AbilityReference;

        // 发射任务集合
        private List<Coroutine> m_EmitTasks = new List<Coroutine>();
        
        // 已发射效果器
        private List<Orb> m_EmitOrbs = new List<Orb>();
        
        protected sealed override void OnEnable()
        {
        }

        protected sealed override void OnDisable()
        {
            if (InterruptEmitWhenDisable)
            {
                for (int i = 0; i < m_EmitTasks.Count; i++)
                {
                    if(m_EmitTasks[i] == null) continue;
                    GameManager.Instance.StopCoroutine(m_EmitTasks[i]);
                    Ability.UnReference();
                }
            }
            
            m_EmitTasks.Clear();

            if (DestroyOrbWhenDisable)
            {
                for (int i = 0; i < m_EmitOrbs.Count; i++)
                {
                    if (m_EmitOrbs[i].isActiveAndEnabled)
                    {
                        GameManager.OrbSystem.Destroy(m_EmitOrbs[i]);
                    }
                }
            }
            
            m_EmitOrbs.Clear();
        }

        protected void StartEmitTask(IEnumerator task)
        {
            Ability.Reference();
            Coroutine coroutine = GameManager.Instance.StartCoroutine(Running(Ability, task, m_EmitTasks));
            m_EmitTasks.Add(coroutine);
        }
        
        private static IEnumerator Running(Ability ability, IEnumerator task, List<Coroutine> tasks)
        {
            int index = tasks.Count;
            yield return task;
            ability.UnReference();
            tasks[index] = null;
        }

        public void Emit(Vector3 position, Quaternion rotation, IAbilityTarget target, Vector3 targetPosition)
        {
            OrbOrder order = new OrbOrder()
            {
                AbilityID              = AbilityReference.AbilityID,
                Position               = position,
                Rotation               = rotation,
                Scale                  = OrbScale * Vector3.one,
                Length                 = OrbLength,
                CastImmediately        = CastImmediately,
                DestroyWhenChangeScene = DestroyWhenChangeScene,
                DelayMove              = DelayMove,
                FollowDuringDelayTime  = FollowDuringDelayTime,
                DelayMoveTime          = DelayMoveTime,
                Mode                   = OrbMode,
                WhenHitWall            = WhenHitWall,
                TargetFilter           = TargetFilter,
                Target                 = target,
                TargetPosition         = targetPosition,
                Giver                  = Ability,
                VariableTable          = AbilityReference,
            };

            Orb instance = GameManager.OrbSystem.Create(order);
            m_EmitOrbs.Add(instance);
        }

        public virtual void PreviewTrajectory(List<PreviewData> previewList)
        {
        }
    }
}