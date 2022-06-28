using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    public class AbilityShield : MonoBehaviour, IAbilityTarget, IFilterTarget
    {
        public bool IsValidAndActive()
        {
            return this != null;
        }

        public Vector3      Position => transform.position;
        public Quaternion   Rotation => transform.rotation;
        public TargetFilter Type     { get; set; }
    }
}