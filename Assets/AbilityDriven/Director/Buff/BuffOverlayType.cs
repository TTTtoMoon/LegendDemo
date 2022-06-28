using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven
{
    /// <summary>
    /// buff叠加方式
    /// </summary>
    public enum BuffOverlayType
    {
        [InspectorName("无法覆盖(即后来者无效)")] DontOverlay,

        [InspectorName("重新开始")] Restart,

        [InspectorName("叠加时长")] OverlayDuration,

        [InspectorName("独立计算")] Independent,
    }
}