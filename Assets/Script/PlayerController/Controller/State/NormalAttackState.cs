using RogueGods.Gameplay.AbilityDriven;
using UnityEngine;

namespace RogueGods.Gameplay.LocalPlayer
{
    public class NormalAttackState : ControllerState
    {
        private float m_PreviousAttackTime;
        private int   m_NormalAttackIndex = 0;

        public bool TryNormalAttack(Actor actor, SkillDescriptor[] normalAttacks)
        {
            if (Time.time - m_PreviousAttackTime > 0.2f || m_NormalAttackIndex + 1 >= normalAttacks.Length)
            {
                m_NormalAttackIndex = 0;
            }
            else
            {
                m_NormalAttackIndex++;
            }

            SkillDescriptor normalAttack = normalAttacks[m_NormalAttackIndex];
            return actor.SkillDirector.Begin(normalAttack, skillSpeed: 1f + actor.Attribute[AttributeType.AttackSpeedModifier]);
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_Machine.VerifyInput(InputType.EnergySkill, InputState.Down) && _Machine.TrySpecialAttack())
            {
                _Machine.ChangeState(SkillState);
                return;
            }

            switch (_Owner.SkillDirector.CurrentStage)
            {
                case SkillPhase.Preparing:
                    m_PreviousAttackTime = Time.time;
                    TryLocomotion();
                    break;

                case SkillPhase.Acting:
                    m_PreviousAttackTime = Time.time;
                    break;

                case SkillPhase.Finishing:
                    if (TryLocomotion() == false && _Machine.Input.VerifyInput(InputType.NormalAttack, InputState.Up))
                    {
                        _Machine.TryNormalAttack();
                    }

                    break;

                case SkillPhase.NoSkill:
                    _Machine.ChangeState(LocomotionState);
                    break;
            }

            bool TryLocomotion()
            {
                Vector3 inputDirection = _Machine.Input.Direction;
                if (inputDirection != Vector3.zero)
                {
                    _Owner.SkillDirector.Interrupt();
                    _Machine.ChangeState(LocomotionState);
                    return true;
                }

                return false;
            }
        }
    }
}