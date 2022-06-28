using System;
using UnityEngine;

namespace RogueGods.Gameplay
{
    public partial class TransformFollower
    {
        /// <summary>
        /// 跟随频率
        /// </summary>
        public enum Rate
        {
            Update,
            FixedUpdate,
            LateUpdate,
        }

        /// <summary>
        /// 跟随方式
        /// </summary>
        [Flags]
        public enum Mode
        {
            None = 0,

            Position = 1,
            Rotation = 2,
            Scale    = 4,

            All = ~0,
        }

        /// <summary>
        /// 跟随参数
        /// </summary>
        public struct Params
        {
            public static readonly Params Default = new Params();

            /// <summary>
            /// 本地坐标 默认为Vector3.Zero
            /// </summary>
            public Vector3? LocalPosition;

            /// <summary>
            /// 本地旋转 Quaternion.identity
            /// </summary>
            public Quaternion? LocalRotation;

            /// <summary>
            /// 本地缩放 默认为Vector3.Zero
            /// </summary>
            public Vector3? LocalScale;

            /// <summary>
            /// 更新频率 默认为Rate.LateUpdate
            /// </summary>
            public Rate? UpdateRate;

            /// <summary>
            /// 跟下模式 默认为Mode.All
            /// </summary>
            public Mode? UpdateMode;

            /// <summary>
            /// 跟随时长
            /// </summary>
            public float? Duration;
        }
    }
}