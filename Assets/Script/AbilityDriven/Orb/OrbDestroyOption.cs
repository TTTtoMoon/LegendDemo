using System;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Flags]
    public enum OrbDestroyOption
    {
        None                = 0,
        PlayDestroyVFX      = 1 << 0,
        TriggerDestroyEvent = 1 << 1,
        All                 = ~None,

        /// <summary>
        /// 当碰到墙销毁时
        /// </summary>
        DestroyWhenHitWall  = PlayDestroyVFX & TriggerDestroyEvent,
        
        /// <summary>
        /// 当碰到角色销毁时
        /// </summary>
        DestroyWhenHitActor = TriggerDestroyEvent,
    }
}