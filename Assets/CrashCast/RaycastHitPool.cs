using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public readonly struct RaycastHitPool
    {
        public const int Tiny   = 4;
        public const int Small  = 8;
        public const int Middle = 16;
        public const int Large  = 32;

        /// <summary>
        /// Length = 4
        /// </summary>
        public static RaycastHit[] TinyRaycastHit() => GetRaycastHit(Tiny);

        /// <summary>
        /// Length = 8
        /// </summary>
        public static RaycastHit[] SmallRaycastHit() => GetRaycastHit(Small);

        /// <summary>
        /// Length = 16
        /// </summary>
        public static RaycastHit[] MiddleRaycastHit() => GetRaycastHit(Middle);

        /// <summary>
        /// Length = 32
        /// </summary>
        public static RaycastHit[] LargeRaycastHit() => GetRaycastHit(Large);

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="array"></param>
        public static void Release(RaycastHit[] array) => ArrayPool<RaycastHit>.Release(array);

        /// <summary>
        /// Length = 4
        /// </summary>
        public static Collider[] TinyOverlay() => GetOverlay(Tiny);

        /// <summary>
        /// Length = 8
        /// </summary>
        public static Collider[] SmallOverlay() => GetOverlay(Small);

        /// <summary>
        /// Length = 16
        /// </summary>
        public static Collider[] MiddleOverlay() => GetOverlay(Middle);

        /// <summary>
        /// Length = 32
        /// </summary>
        public static Collider[] LargeOverlay() => GetOverlay(Large);

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="array"></param>
        public static void Release(Collider[] array) => ArrayPool<Collider>.Release(array);
        
        private static RaycastHit[] GetRaycastHit(int size)
        {
            return ArrayPool<RaycastHit>.Get(size);
        }

        private static Collider[] GetOverlay(int size)
        {
            return ArrayPool<Collider>.Get(size);
        }
    }
}