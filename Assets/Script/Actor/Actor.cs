using System;
using System.Collections;
using System.Globalization;
using Abilities;
using RogueGods.Gameplay.AbilityDriven;
using RogueGods.Gameplay.Blood;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RogueGods.Gameplay
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SlotPoint))]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class Actor : MonoBehaviour,
        IAbilityOwner, IAbilityTarget, IFilterTarget,
        IDamageMaker, IDamageTaker
    {
        public static class Events
        {
            public static DamageListener OnDead;
            public static DamageListener OnMadeDamage;
            public static DamageListener OnTakeDamage;
        }

        [Serializable]
        public struct AttributeConfig
        {
            public AttributeType Type;
            public float         Value;
        }

        public bool IsValidAndActive()
        {
            return this != null && gameObject.activeSelf;
        }

        [SerializeField] private TargetFilter       m_Type;
        [SerializeField] private BodyMaterial       m_BodyMaterial;
        [SerializeField] private BodyType           m_BodyType;
        [SerializeField] private LocomotionProperty m_Locomotion;
        [SerializeField] private AttributeConfig[]  m_Attributes;
        [SerializeField] private HeaderBlood        m_BloodPrefab;
        [SerializeField] private TextMeshPro        m_DamageLabel;
        [SerializeField] private Vector3            m_DamageLabelOffset;
        [SerializeField] private AudioClip          m_DeathAudio;
        [NonSerialized]  private HeaderBlood        m_BloodInstance;
        [NonSerialized]  private SlotPoint          m_SlotPoint;
        [NonSerialized]  private NavMeshAgent       m_NavMeshAgent;
        [NonSerialized]  private float              m_CurrentHealth;
        [NonSerialized]  private float              m_CurrentEnergy;

        public Vector3            Position        => transform.position;
        public Quaternion         Rotation        => transform.rotation;
        public TargetFilter       Type            => m_Type;
        public BodyMaterial       Material        => m_BodyMaterial;
        public BodyType           BodyType        => m_BodyType;
        public Vector3            HitPosition     => m_SlotPoint.GetSlotPosition(TransformSlot.HitPosition);
        public LocomotionProperty Locomotion      => m_Locomotion;
        public SlotPoint          SlotPoint       => m_SlotPoint;
        public NavMeshAgent       NavMeshAgent    => m_NavMeshAgent;
        public AbilityDirector    AbilityDirector { get; private set; }
        public SkillDirector      SkillDirector   { get; private set; }
        public BuffDirector       BuffDirector    { get; private set; }
        public AnimatorDirector   Animator        { get; private set; }
        public ActorTagStack      Tag             { get; private set; }
        public AttributeProperty  Attribute       { get; private set; }

        public event AttributeChangeDelegate OnCurrentHealthChanged;
        public event AttributeChangeDelegate OnCurrentEnergyChanged;
        public event DamageListener          OnDead;
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

        public float CurrentEnergy
        {
            get => m_CurrentEnergy;
            set
            {
                float old = m_CurrentEnergy;
                m_CurrentEnergy = value;
                OnCurrentEnergyChanged?.Invoke(old, value);
            }
        }

        private void Awake()
        {
            m_SlotPoint     = GetComponent<SlotPoint>();
            m_NavMeshAgent  = GetComponent<NavMeshAgent>();
            AbilityDirector = new AbilityDirector(this);
            SkillDirector   = new SkillDirector(this, AbilityDirector);
            BuffDirector    = new BuffDirector(AbilityDirector);
            Animator        = new AnimatorDirector(GetComponent<Animator>());
            Tag             = new ActorTagStack();
            Attribute       = new AttributeProperty();

            for (int i = 0, length = m_Attributes.Length; i < length; i++)
            {
                Attribute.SetBaseValueWithoutNotify(m_Attributes[i].Type, m_Attributes[i].Value);
            }

            m_CurrentHealth = Attribute[AttributeType.MaxHealth];
            Animator.AddStateExitListener(AnimationDefinition.State.Death, ReBorn);
        }

        private void Start()
        {
            m_BloodInstance = HeaderBlood.CreateBlood(m_BloodPrefab, this);
        }

        private void OnDestroy()
        {
            Animator.RemoveStateExitListener(AnimationDefinition.State.Death, ReBorn);
            if (m_BloodInstance != null) DestroyImmediate(m_BloodInstance);
        }

        public void EnableRootMotion()
        {
        }

        public void DisableRootMotion()
        {
        }

        /// <summary>
        /// ??????????????????
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
        /// ??????????????????
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
        /// ??????
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
        /// ????????????????????????
        /// </summary>
        /// <param name="animationSpeed"></param>
        public void SetMoveAnimationSpeed(float animationSpeed)
        {
            Animator.SetFloat(AnimationDefinition.Parameter.MovementSpeed, animationSpeed);
        }

        /// <summary>
        /// ????????????
        /// </summary>
        public void StopMove()
        {
            SetMoveAnimationSpeed(0f);
        }

        /// <summary>
        /// ??????
        /// </summary>
        public void ReBorn()
        {
            if (Type == TargetFilter.Player)
            {
                transform.position = new Vector3(2.4f, 0f, 2.8f);
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            Animator.Play(AnimationDefinition.State.Birth);
            CurrentHealth = Attribute[AttributeType.MaxHealth];
        }
        
        /// <summary>
        /// ??????
        /// </summary>
        public void Die()
        {
            AudioSource.PlayClipAtPoint(m_DeathAudio, Position);
            Animator.Play(AnimationDefinition.State.Death);
        }

        /// <summary>
        /// ?????????????????????
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
            return m_CurrentHealth > 0f;
        }

        void IDamageMaker.PrepareMakeDamage(in DamageRequest request, ref Attacker attacker)
        {
            attacker.Attack                 = Attribute[AttributeType.Attack];
            attacker.AttackMultipleModifier = Attribute[AttributeType.AttackMultiplierModifier];
            attacker.CriticalChance         = Attribute[AttributeType.CriticalChance];
            attacker.CriticalPowerAddition  = Attribute[AttributeType.CriticalPowerAddition];
        }

        void IDamageMaker.MadeDamage(in DamageResponse response)
        {
            OnMadeDamage?.Invoke(response);
            Events.OnMadeDamage?.Invoke(response);
        }

        bool IDamageTaker.CanTakeDamage(in DamageRequest request)
        {
            return m_CurrentHealth                   > 0f     &&                                   // ??????
                   Tag.HasTag(ActorTag.Transparency) == false &&                                   // ?????????
                   Random.Range(0f, 1f)              >= Attribute[AttributeType.DodgeProbability]; // ?????????
        }

        void IDamageTaker.PrepareTakeDamage(in DamageRequest request, ref Defender defender)
        {
        }

        void IDamageTaker.TakeDamage(in DamageResponse response)
        {
            DecreaseCurrentHealth(response.Damage);
            OnTakeDamage?.Invoke(response);
            Events.OnTakeDamage?.Invoke(response);
            TextMeshPro label = Instantiate(m_DamageLabel);
            label.transform.position = transform.position + m_DamageLabelOffset;
            label.transform.forward  = GameManager.MainCamera.transform.forward;
            label.text               = response.IsCritical ? $"{response.Damage}!" : response.Damage.ToString(CultureInfo.InvariantCulture);
            label.color              = response.IsCritical ? Color.red : new Color(0.99f, 0.86f, 0.31f, 1f);

            if (m_CurrentHealth > 0f)
            {
                Animator.Play(AnimationDefinition.State.Hurt);
            }
            else
            {
                Die();
                OnDead?.Invoke(response);
                Events.OnDead?.Invoke(response);
            }
        }
    }
}