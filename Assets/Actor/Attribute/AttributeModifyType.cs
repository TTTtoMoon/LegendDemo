using UnityEngine;

namespace RogueGods.Gameplay
{
    public enum AttributeModifyType
    {
        [InspectorName("直接相加减(正加负减)")]
        DirectAddition,
            
        [InspectorName("直接赋值")]
        DirectSet,
            
        [InspectorName("若低于目标值，则赋值为目标值")]
        SetIfLess,
    }
}