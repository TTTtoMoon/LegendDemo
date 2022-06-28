using Abilities;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public interface IDamageMaker : IAbilityTarget
    {
        /// <summary>
        /// 位置
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// 角度
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// 能否造成伤害
        /// </summary>
        /// <param name="request"></param>
        bool CanMakeDamage(in DamageRequest request);

        /// <summary>
        /// 准备造成伤害，处理增伤减伤
        /// </summary>
        void PrepareMakeDamage(in DamageRequest request, ref Attacker attacker);

        /// <summary>
        /// 造成了伤害
        /// </summary>
        void MadeDamage(in DamageResponse response);
    }

    public static class IDamageMakerNullChecker
    {
        public static bool IsNull(this IDamageMaker damageMaker)
        {
            if(damageMaker == null)
            {
                return true;
            }

            if (damageMaker is Object unityObject)
            {
                return unityObject == null;
            }

            return false;
        }
    }
}