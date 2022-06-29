using System;
using Abilities;
using RogueGods.Gameplay.AbilityDriven;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay
{
    [RequireComponent(typeof(Animator), typeof(SlotPoint), typeof(NavMeshAgent))]
    public sealed class Actor : MonoBehaviour,
        IAbilityOwner, IAbilityTarget, IFilterTarget,
        IDamageMaker, IDamageTaker
    {
        public static class Events
        {
            public static event Action<Actor>  OnDead;
            public static event DamageListener OnMadeDamage;
            public static event DamageListener OnTakeDamage;
        }

        public bool IsValidAndActive()
        {
            return this != null && gameObject.activeSelf;
        }

        [SerializeField] private TargetFilter       m_Type;
        [SerializeField] private BodyMaterial       m_BodyMaterial;
        [SerializeField] private BodyType           m_BodyType;
        [SerializeField] private int                m_NormalAttack;
        [SerializeField] private int                m_EnergySkill;
        [SerializeField] private LocomotionProperty m_Locomotion;
        [NonSerialized]  private SlotPoint          m_SlotPoint;
        [NonSerialized]  private NavMeshAgent       m_NavMeshAgent;
        [NonSerialized]  private float              m_CurrentHealth;
        [NonSerialized]  private float              m_CurrentEnergy;

        public Vector3            Position        => transform.position;
        public Quaternion         Rotation        => transform.rotation;
        public TargetFilter       Type            => m_Type;
        public BodyMaterial       Material        => m_BodyMaterial;
        public BodyType           BodyType        => m_BodyType;
        public Vector3            HitPosition     => transform.position;
        public LocomotionProperty Locomotion      => m_Locomotion;
        public SlotPoint          SlotPoint       => m_SlotPoint;
        public NavMeshAgent       NavMeshAgent    => m_NavMeshAgent;
        public SkillDescriptor    NormalAttack    { get; private set; }
        public SkillDescriptor    EnergySkill     { get; private set; }
        public AbilityDirector    AbilityDirector { get; private set; }
        public SkillDirector      SkillDirector   { get; private set; }
        public BuffDirector       BuffDirector    { get; private set; }
        public AnimatorDirector   Animator        { get; private set; }
        public ActorTagStack      Tag             { get; private set; }
        public AttributeProperty  Attribute       { get; private set; }

        public event AttributeChangeDelegate OnCurrentHealthChanged;
        public event AttributeChangeDelegate OnCurrentEnergyChanged;
        public event Action                  OnDead;
        public event DamageListener          OnMadeDamage;
        public event DamageListener          OnTakeDamage;

        public float CurrentHealth
        {
            get => m_CurrentHealth;
            set
            {
                if (value > m_CurrentHealth)
                {
                    IncreaseCurrentHealth(value - m_CurrentHealth);
                }
                else if (value < m_CurrentHealth)
                {
                    DecreaseCurrentHealth(m_CurrentHealth - value);
                }
            }
        }

        private void Awake()
        {
            m_SlotPoint     = GetComponent<SlotPoint>();
            m_NavMeshAgent  = GetComponent<NavMeshAgent>();
            NormalAttack    = new SkillDescriptor(m_NormalAttack);
            EnergySkill     = new SkillDescriptor(m_EnergySkill);
            AbilityDirector = new AbilityDirector(this);
            SkillDirector   = new SkillDirector(this, AbilityDirector);
            BuffDirector    = new BuffDirector(AbilityDirector);
            Animator        = new AnimatorDirector(GetComponent<Animator>());
            Tag             = new ActorTagStack();
            Attribute       = new AttributeProperty();
        }

        public void EnableRootMotion()
        {
        }

        public void DisableRootMotion()
        {
        }

        /// <summary>
        /// 增加当前血量
        /// </summary>
        public void IncreaseCurrentHealth(float modify)
        {
            if (modify < 0)
            {
                modify = -modify;
            }

            if (modify < 1f)
            {
                return;
            }

            SetCurrentHealth(m_CurrentHealth + modify);
        }

        /// <summary>
        /// 扣除当前血量
        /// </summary>
        public void DecreaseCurrentHealth(float modify)
        {
            if (modify < 0)
            {
                modify = -modify;
            }

            if (modify < 1f)
            {
                return;
            }

            SetCurrentHealth(m_CurrentHealth - modify);
        }
        
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="movement"></param>
        /// <param name="animationSpeed"></param>
        public void Move(Vector3 movement, float animationSpeed)
        {
            if (Tag.HasTag(ActorTag.Unmovable) || movement == Vector3.zero)
            {
                StopMove();
            }
            else
            {
                NavMeshAgent.Move(movement);
                SetMoveAnimationSpeed(animationSpeed);
            }
        }

        /// <summary>
        /// 设置移动动画速度
        /// </summary>
        /// <param name="animationSpeed"></param>
        public void SetMoveAnimationSpeed(float animationSpeed)
        {
            Animator.SetFloat(AnimationDefinition.Parameter.MovementSpeed, animationSpeed);
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        public void StopMove()
        {
            SetMoveAnimationSpeed(0f);
        }

        /// <summary>
        /// 设置当前血量值
        /// </summary>
        private bool SetCurrentHealth(float value)
        {
            float old = m_CurrentHealth;
            m_CurrentHealth = Mathf.Clamp(value, 0, Attribute[AttributeType.MaxHealth]);
            m_CurrentHealth = AttributeType.MaxHealth.Fix(m_CurrentHealth);
            if (Mathf.Abs(m_CurrentHealth - old) >= 1f)
            {
                OnCurrentHealthChanged?.Invoke(old, m_CurrentHealth);
                return true;
            }

            return false;
        }

        bool IDamageMaker.CanMakeDamage(in DamageRequest request)
        {
            throw new NotImplementedException();
        }

        void IDamageMaker.PrepareMakeDamage(in DamageRequest request, ref Attacker attacker)
        {
            throw new NotImplementedException();
        }

        void IDamageMaker.MadeDamage(in DamageResponse response)
        {
            throw new NotImplementedException();
        }

        bool IDamageTaker.CanTakeDamage(in DamageRequest request)
        {
            throw new NotImplementedException();
        }

        void IDamageTaker.PrepareTakeDamage(in DamageRequest request, ref Defender defender)
        {
            throw new NotImplementedException();
        }

        void IDamageTaker.TakeDamage(in DamageResponse response)
        {
            throw new NotImplementedException();
        }
    }
}