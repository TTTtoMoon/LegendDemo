namespace RogueGods.Gameplay.LocalPlayer
{
    public abstract class ControllerState : State<Actor, PlayerController>
    {
        public static readonly LocomotionState   LocomotionState   = new LocomotionState();
        public static readonly SkillState        SkillState        = new SkillState();
        public static readonly NormalAttackState NormalAttackState = new NormalAttackState();
    }
}