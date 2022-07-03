using RogueGods.Gameplay.AbilityDriven;
using RogueGods.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityInput = UnityEngine.Input;

namespace RogueGods.Gameplay.LocalPlayer
{
    public class PlayerController : StateMachine<Actor, PlayerController>
    {
        public readonly PlayerInput Input = new PlayerInput();

        [SerializeField] private float m_InputCacheTime = 0.2f;
        [SerializeField] private int[] m_NormalAttacks  = new int[0];
        [SerializeField] private int   m_SpecialAttack;

        public SkillDescriptor[] NormalAttacks { get; private set; }
        public SkillDescriptor   SpecialAttack { get; private set; }

        protected override State<Actor, PlayerController> GetDefaultState()
        {
            return ControllerState.LocomotionState;
        }

        protected override void Start()
        {
            base.Start();
            Input.InputCacheTime = m_InputCacheTime;
            SetOwner(GetComponent<Actor>());
            NormalAttacks = new SkillDescriptor[m_NormalAttacks.Length];
            for (int i = 0; i < m_NormalAttacks.Length; i++)
            {
                NormalAttacks[i] = new SkillDescriptor(m_NormalAttacks[i]);
            }

            SpecialAttack = new SkillDescriptor(m_SpecialAttack);
        }

        protected override void Update()
        {
            if (Owner.CurrentHealth <= 0f)
            {
                Input.ClearInput();
                return;
            }

            Input.UpdateInputState();
            Vector2 direction = Vector2.zero;
            if (UnityInput.GetKey(KeyCode.W))
            {
                direction.y += 1f;
            }

            if (UnityInput.GetKey(KeyCode.A))
            {
                direction.x -= 1f;
            }

            if (UnityInput.GetKey(KeyCode.S))
            {
                direction.y -= 1f;
            }

            if (UnityInput.GetKey(KeyCode.D))
            {
                direction.x += 1f;
            }

            direction.Rotation(GameManager.MainCamera.transform.eulerAngles.y);
            Input.SetDirection(direction);

            if (UnityInput.GetKeyDown(KeyCode.Space))
            {
                Input.EnqueueInput(InputType.Dash, true);
            }
            else if (UnityInput.GetKeyUp(KeyCode.Space))
            {
                Input.EnqueueInput(InputType.Dash, false);
            }

            if (UnityInput.GetKeyDown(KeyCode.J))
            {
                Input.EnqueueInput(InputType.NormalAttack, true);
            }
            else if (UnityInput.GetKeyUp(KeyCode.J))
            {
                Input.EnqueueInput(InputType.NormalAttack, false);
            }

            if (UnityInput.GetKeyDown(KeyCode.K))
            {
                Input.EnqueueInput(InputType.EnergySkill, true);
            }
            else if (UnityInput.GetKeyUp(KeyCode.K))
            {
                Input.EnqueueInput(InputType.EnergySkill, false);
            }

            if (UnityInput.GetKeyDown(KeyCode.R))
            {
                Owner.CurrentHealth = 0f;
                Owner.Animator.Play(AnimationDefinition.State.Death);
            }

            base.Update();
        }

        public bool VerifyInput(InputType inputType, InputState inputState)
        {
            return Input.VerifyInput(inputType, inputState);
        }

        public bool AnyInput()
        {
            return Input.AnyInput();
        }

        public bool TryNormalAttack()
        {
            if (ControllerState.NormalAttackState.TryNormalAttack(Owner, NormalAttacks) == false)
            {
                return false;
            }

            return true;
        }

        public bool TrySpecialAttack()
        {
            if (Owner.SkillDirector.Begin(SpecialAttack) == false)
            {
                return false;
            }

            return true;
        }
    }
}