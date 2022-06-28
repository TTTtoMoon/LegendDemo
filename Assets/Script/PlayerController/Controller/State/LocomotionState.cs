using UnityEngine;

namespace RogueGods.Gameplay.LocalPlayer
{
    public class LocomotionState : ControllerState
    {
        private Quaternion? m_InputDirection;
        private float       m_AnimationSpeed = 0f;

        protected override void OnEnter()
        {
            base.OnEnter();
            m_InputDirection = null;
            _Owner.Animator.Play(AnimationDefinition.State.Locomotion);
            OnUpdate(); // 进入时进行一次update检测输入，避免造成的一帧延迟问题
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (HandleSkill())
            {
                return;
            }

            if (HandleAttack())
            {
                return;
            }

            HandleMove();
        }

        protected override void OnExit()
        {
            base.OnExit();
            m_AnimationSpeed = 0f;
            _Owner.StopMove();
        }

        private bool HandleMove()
        {
            Vector3 inputDirection = _Machine.Input.Direction;
            // 输入有效 移动
            if (inputDirection != Vector3.zero)
            {
                m_InputDirection = Quaternion.LookRotation(inputDirection);
                m_AnimationSpeed = Mathf.MoveTowards(m_AnimationSpeed, 1f, _Owner.Locomotion.MovementAcceleration / _Owner.Locomotion.MovementSpeed);
                _Owner.Move(_Owner.Locomotion.MovementSpeed * Time.deltaTime * inputDirection, m_AnimationSpeed);
                return true;
            }

            if (m_InputDirection != null && m_InputDirection.Value != _Owner.Rotation)
            {
                _Owner.transform.rotation = Quaternion.RotateTowards(_Owner.Rotation, m_InputDirection.Value, Time.deltaTime * _Owner.Locomotion.RotationSpeed);
            }

            return false;
        }

        private bool HandleSkill()
        {
            if (_Machine.VerifyInput(InputType.EnergySkill, InputState.Down) && _Machine.TrySkill())
            {
                _Machine.ChangeState(SkillState);
                return true;
            }

            return false;
        }

        private bool HandleAttack()
        {
            if (_Machine.VerifyInput(InputType.NormalAttack, InputState.Down | InputState.Hold) && _Machine.TryNormalAttack())
            {
                _Machine.ChangeState(NormalAttackState);
                return true;
            }

            return false;
        }
    }
}