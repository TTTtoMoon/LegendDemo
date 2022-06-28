using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using RogueGods.Gameplay.VFX;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// 效果器，诸如子弹、法阵等脱离了施放者的能力
    /// </summary>
    public sealed class Orb : MonoBehaviour, IOrb, IAbilityOwner, IPoolDisposable
    {
        private Collider              m_Collider;
        private CommonAbilityDirector m_AbilityDirector;
        private Ability               m_Ability;
        private OrbOrder              m_Order;
        private Coroutine             m_DelayTask;
        private List<VFX>             m_VFXList;

        public Vector3                Position            => transform.position;
        public Quaternion             Rotation            => transform.rotation;
        public IAbilityTarget         Target              => m_Ability.Target;
        public Vector3                TargetPosition      => m_Order.TargetPosition;
        public Ability                Ability             => m_Ability;
        public bool                   Enable              { get; private set; }
        public float                  Length              { get; private set; }
        public AbilityDirector        AbilityDirector     { get; private set; }
        public OrbMode                Mode                { get; private set; }
        public OrbWhenHitWall         WhenHitWall         { get; private set; }
        public Filter<IAbilityTarget> TargetFilter        { get; private set; }
        public float                  MoveSpeedMultiplier { get; set; }

        public bool IsValidAndActive()
        {
            return Ability != null && Ability.IsActive && this != null;
        }

        public OnCrash OnCrash { get; } = new OnCrash();

        public event Action<RaycastHit> CrashWallEvent;

        private void Awake()
        {
            AbilityDirector   = new AbilityDirector(this);
            m_AbilityDirector = new CommonAbilityDirector(AbilityDirector);
            AbilityDirector.OnDisable += (x) =>
            {
                if (AbilityDirector.Abilities.Count == 0)
                {
                    GameManager.OrbSystem.Destroy(this);
                }
            };
        }

        void IOrb.Initialize(OrbOrder order)
        {
            name                = order.AbilityID.ToString();
            m_Order             = order;
            MoveSpeedMultiplier = 1f;
            Length              = order.Length;
            Mode                = order.Mode.Clone();
            WhenHitWall         = order.WhenHitWall.Clone();
            TargetFilter        = order.TargetFilter;
            transform.SetPositionAndRotation(order.Position, order.Rotation);
            transform.localScale = order.Scale;
            m_VFXList            = ListPool<VFX>.Get();
            CommonAbilityDescriptor descriptor = CommonAbilityDescriptor.CreateOrbAbility(m_Order.AbilityID, m_Order.VariableTable);
            m_Ability  = m_AbilityDirector.Begin(descriptor, m_Order.Target, m_Order.Giver);
            m_Collider = m_Ability.TryGetComponent(out OrbInfo orbInfo) ? orbInfo.CreateTrigger(gameObject) : gameObject.AddComponent<SphereCollider>();
        }

        void IOrb.Enable()
        {
            Enable = true;
            gameObject.SetActive(true);
            if (m_Order.DelayMove)
            {
                m_DelayTask = StartCoroutine(Delay());
            }
            else
            {
                ActualEnable();
            }

            void ActualEnable()
            {
                Mode.Enable(this);
                WhenHitWall.Enable(this);
                CrashCast(m_Ability.Source.Position, Position, m_Order.CastImmediately);
            }

            IEnumerator Delay()
            {
                float   startTime      = Time.time;
                Vector3 offsetPosition = transform.position - Ability.Giver.Owner.Position;
                while (startTime + m_Order.DelayMoveTime > Time.time)
                {
                    if (m_Order.FollowDuringDelayTime) transform.position = Ability.Giver.Owner.Position + offsetPosition;
                    yield return null;
                }

                ActualEnable();
            }
        }

        void IOrb.Disable()
        {
            if (m_DelayTask != null)
            {
                StopCoroutine(m_DelayTask);
                m_DelayTask = null;
            }

            ListPool<VFX>.Release(m_VFXList);
            m_VFXList = null;
            Enable    = false;
            Mode.Disable();
            WhenHitWall.Disable();
            m_AbilityDirector.InterruptAll();
        }

        void IPoolDisposable.Dispose()
        {
            gameObject.SetActive(false);
            DestroyImmediate(m_Collider);
            OnCrash.Clear();
            CrashWallEvent = null;
            m_Collider     = null;
            m_Ability      = null;
            m_Order        = default;
        }

        public void TranslatePosition(Vector3 deltaPosition)
        {
            TranslatePositionAndRotation(deltaPosition, Quaternion.identity);
        }

        public void TranslateRotation(Quaternion deltaRotation)
        {
            TranslatePositionAndRotation(Vector3.zero, deltaRotation);
        }

        public void TranslatePositionAndRotation(Vector3 deltaPosition, Quaternion deltaRotation)
        {
            Vector3 oldPosition = Position;
            Vector3 newPosition = oldPosition + deltaPosition;
            CrashCast(oldPosition, newPosition, true);
            if (deltaRotation == Quaternion.identity)
            {
                transform.position = newPosition;
            }
            else
            {
                transform.SetPositionAndRotation(newPosition, transform.rotation * deltaRotation);
            }
        }

        public void AddVFX(VFXInstance instance, Vector3 localPosition, Vector3 localScale)
        {
            m_VFXList.Add(new VFX(instance, localPosition, localScale));
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
        }

        public void UpdateVFX(Vector3 position, Quaternion rotation, Vector3 scale, float height, bool rotateWithTangent)
        {
            for (int i = 0, length = m_VFXList.Count; i < length; i++)
            {
                VFX vfx = m_VFXList[i];
                vfx.Update(position, rotation, scale, height, rotateWithTangent);
                m_VFXList[i] = vfx;
            }
        }

        /// <summary>
        /// 碰撞检测
        /// </summary>
        /// <param name="startPosition">起始位置</param>
        /// <param name="endPosition">结束位置</param>
        /// <param name="castOther">是否对其他目标进行碰撞检测</param>
        /// <returns>返回是否击中墙</returns>
        private void CrashCast(Vector3 startPosition, Vector3 endPosition, bool castOther)
        {
            float hitWallDistance2 = float.MaxValue;
            startPosition.y = 0f;
            endPosition.y   = 0f;
            if (Physics.Linecast(startPosition, endPosition, out RaycastHit hit, LayerDefine.Obstacle.Mask | LayerDefine.Wall.Mask))
            {
                hitWallDistance2 = (hit.point - startPosition).sqrMagnitude;
            }

            if (castOther)
            {
                using CrashResult result = CrashCaster.Cast(m_Collider, startPosition, endPosition - startPosition);
                for (int i = 0; i < result.Count; i++)
                {
                    IAbilityTarget target         = result[i];
                    float          otherDistance2 = (target.Position - startPosition).sqrMagnitude;
                    if (otherDistance2 > hitWallDistance2)
                    {
                        break;
                    }

                    if (target.IsValidAndActive()   &&
                        TargetFilter.Verify(target) &&
                        OnCrash.Invoke(target))
                    {
                        break;
                    }
                }
            }

            if (hit.collider != null)
            {
                WhenHitWall.Handle(this, hit);
                CrashWallEvent?.Invoke(hit);
            }
        }

        public struct VFX
        {
            public VFX(VFXInstance vfx, Vector3 localPosition, Vector3 localScale)
            {
                m_Root          = vfx.transform;
                m_Effect        = vfx.EffectRoot;
                m_Shadow        = vfx.ShadowRoot;
                m_PrePosition   = m_Effect != null ? m_Effect.position : m_Root.position;
                m_LocalPosition = localPosition;
                m_LocalScale    = localScale;
            }

            private Transform m_Root;
            private Transform m_Effect;
            private Transform m_Shadow;
            private Vector3   m_PrePosition;
            private Vector3   m_LocalPosition;
            private Vector3   m_LocalScale;

            public void Update(Vector3 orbPosition, Quaternion orbRotation, Vector3 orbScale, float nowHeight, bool rotateWithTangent)
            {
                m_Root.position = orbPosition;
                m_Root.rotation = orbRotation;
                if (m_Effect)
                {
                    Vector3 position = m_Root.position + new Vector3(0f, nowHeight, 0f);
                    position.y =  nowHeight;
                    position   += m_LocalPosition;
                    Quaternion rotation = rotateWithTangent ? Quaternion.LookRotation(position - m_PrePosition) : orbRotation;
                    m_Effect.SetPositionAndRotation(position, rotation);
                    m_Effect.localScale = orbScale.Multiple(m_LocalScale);
                    m_PrePosition       = position;
                }

                if (m_Shadow)
                {
                    Vector3 position = m_Shadow.localPosition;
                    position.y             = -orbPosition.y + 0.01f;
                    m_Shadow.localPosition = position;
                }
            }
        }
    }
}