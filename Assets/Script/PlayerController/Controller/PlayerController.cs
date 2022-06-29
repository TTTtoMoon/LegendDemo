using RogueGods.Utility;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RogueGods.Gameplay.LocalPlayer
{
    public class PlayerController : StateMachine<Actor, PlayerController>
    {
        public readonly PlayerInput Input = new PlayerInput();
        
        private Transform m_MainCamera;

        protected override State<Actor, PlayerController> GetDefaultState()
        {
            return ControllerState.LocomotionState;
        }

        protected override void Awake()
        {
            base.Awake();
            if (Camera.main != null) m_MainCamera = Camera.main.transform;
        }

        protected override void Start()
        {
            base.Start();
            SetOwner(GetComponent<Actor>());
        }

        protected override void Update()
        {
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

            direction.Rotation(m_MainCamera.eulerAngles.y);
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
            if (Owner.SkillDirector.Begin(Owner.NormalAttack) == false)
            {
                return false;
            }

            return true;
        }

        public bool TrySkill()
        {
            if (Owner.SkillDirector.Begin(Owner.EnergySkill) == false)
            {
                return false;
            }

            return true;
        }
    }
}