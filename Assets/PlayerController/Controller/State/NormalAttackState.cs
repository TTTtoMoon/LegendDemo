using RogueGods.Gameplay.AbilityDriven;
using UnityEngine;

namespace RogueGods.Gameplay.LocalPlayer
{
    public class NormalAttackState : ControllerState
    {
        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_Machine.VerifyInput(InputType.EnergySkill, InputState.Down) && _Machine.TrySkill())
            {
                _Machine.ChangeState(SkillState);
                return;
            }

            Vector3 inputDirection = _Machine.Input.Direction;
            if (inputDirection != Vector3.zero)
            {
                _Owner.SkillDirector.Interrupt();
                _Machine.ChangeState(LocomotionState);
                return;
            }
            
            switch (_Owner.SkillDirector.CurrentStage)
            {
                case SkillPhase.Finishing: 
                    if (_Machine.Input.VerifyInput(InputType.NormalAttack, InputState.Up))
                    {
                        _Machine.TryNormalAttack();
                    }

                    break;
                case SkillPhase.NoSkill:
                    _Machine.ChangeState(LocomotionState);
                    break;
            }
        }
    }
}