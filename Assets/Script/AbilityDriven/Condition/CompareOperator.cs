using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.Condition
{
    public enum CompareOperator
    {
        [InspectorName("少于")]    
        Less,
        [InspectorName("少于或等于")] 
        LessOrEqual,
        [InspectorName("等于")]    
        Equal,
        [InspectorName("等于或大于")] 
        MoreOrEqual,
        [InspectorName("大于")]    
        More,
    }
}