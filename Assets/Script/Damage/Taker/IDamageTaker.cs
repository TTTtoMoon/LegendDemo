using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public interface IDamageTaker : IAbilityTarget
    {
        /// <summary>
        /// 受击材质类型
        /// </summary>
        BodyMaterial Material { get; }

        /// <summary>
        /// 身材
        /// </summary>
        BodyType BodyType { get; }

        /// <summary>
        /// 受击点位置
        /// </summary>
        Vector3 HitPosition { get; }

        /// <summary>
        /// 能否受到伤害
        /// </summary>
        bool CanTakeDamage(in DamageRequest request);

        /// <summary>
        /// 准备承受伤害，处理增伤减伤
        /// </summary>
        void PrepareTakeDamage(in DamageRequest request, ref Defender defender);

        /// <summary>
        /// 受击后接口
        /// </summary>
        void TakeDamage(in DamageResponse response);
    }
}