using RogueGods.Gameplay.AbilityDriven;
using UnityEngine;

namespace RogueGods.Gameplay.LocalPlayer
{
    public class SkillState : ControllerState
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
            if (_Owner.SkillDirector.CurrentStage == SkillPhase.NoSkill)
            {
                _Machine.ChangeState(LocomotionState);
            }
        }
    }
}