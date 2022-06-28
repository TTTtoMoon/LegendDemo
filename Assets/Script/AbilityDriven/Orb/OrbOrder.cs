using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    public struct OrbOrder
    {
        public int                    AbilityID;
        public Vector3                Position;
        public Quaternion             Rotation;
        public Vector3                Scale;
        public float                  Length;
        public bool                   CastImmediately;
        public bool                   DestroyWhenChangeScene;
        public bool                   DelayMove;
        public bool                   FollowDuringDelayTime;
        public float                  DelayMoveTime;
        public OrbMode                Mode;
        public OrbWhenHitWall         WhenHitWall;
        public Filter<IAbilityTarget> TargetFilter;
        public IAbilityTarget         Target;
        public Vector3                TargetPosition;
        public Ability                Giver;
        public IAbilityVariableTable  VariableTable;
    }
}