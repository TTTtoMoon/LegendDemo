using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 请勿在中间添加，或删除，只能在末尾增加新的类型
    /// </summary>
    public enum TransformSlot
    {
        [InspectorName("根节点")]
        Root,
        [InspectorName("头部")]
        Head,

        //血条相关
        [InspectorName("血条相关-伤害值")]
        Damage,
        [InspectorName("血条相关-暴击值")]
        Critical,
        [InspectorName("血条相关-提示")]
        Tip,
        
        [InspectorName("受击点")]
        HitPosition,
        
        [InspectorName("召唤点")]
        Summon,
        
        [InspectorName("左脚")]
        LeftFoot,
        [InspectorName("右脚")]
        RightFoot,
        
        [InspectorName("左手")]
        LeftHand,
        [InspectorName("右手")]
        RightHand,
        [InspectorName("胸口")]
        Chest,
        [InspectorName("左武器")]
        LeftWeapon,
        [InspectorName("右武器")]
        RightWeapon,
        [InspectorName("背部武器")]
        BackWeapon
    }
}