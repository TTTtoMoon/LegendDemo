using UnityEngine;

namespace Abilities
{
    /// <summary>
    /// 功能目标源点
    /// </summary>
    public enum FeatureTargetOrigin
    {
        [InspectorName("拥有者")]
        Owner,

        [InspectorName("来源者")]
        Source,

        [InspectorName("触发目标")]
        TriggerTarget,
        
        [InspectorName("索敌目标")]
        SearchedTarget,
    }
}