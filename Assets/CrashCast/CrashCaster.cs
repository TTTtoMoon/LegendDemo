using System;
using System.Collections.Generic;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 碰撞检测
    /// </summary>
    public static class CrashCaster
    {
        private static readonly ColliderCompare s_Compare          = new ColliderCompare();
        private static readonly int             s_CanCastLayerMask = LayerDefine.Gethit.Mask;

        public static CrashResult Cast(Collider collider, Vector3 startPosition, Vector3 deltaPosition)
        {
            CrashResult result;
            switch (collider)
            {
                case BoxCollider temp:
                    result = Cast(new Box(temp)
                    {
                        Position = startPosition,
                    }, deltaPosition);
                    break;
                case SphereCollider temp:
                    result = Cast(new Sphere(temp)
                    {
                        Position = startPosition,
                    }, deltaPosition);
                    break;
                case CapsuleCollider temp:
                    result = Cast(new Sphere(temp)
                    {
                        Position = startPosition
                    }, deltaPosition);
                    break;
                default:
                    result = new CrashResult();
                    Debugger.LogError($"不支持的Collider类型{collider.GetType()} : {collider}");
                    break;
            }

            return result;
        }

        public static CrashResult Cast(Box box, Vector3 deltaPosition)
        {
            if (deltaPosition.sqrMagnitude < float.Epsilon)
            {
                return Overlay(box);
            }

            deltaPosition.y = 0f;
            float        distance  = deltaPosition.magnitude;
            Vector3      direction = deltaPosition / distance;
            RaycastHit[] hits      = RaycastHitPool.MiddleRaycastHit();
            int          castCount = Physics.BoxCastNonAlloc(box.Position, box.HalfSize, direction, hits, box.Rotation, distance, s_CanCastLayerMask);
            return new CrashResult(castCount, hits);
        }

        public static CrashResult Cast(Sphere sphere, Vector3 deltaPosition)
        {
            if (deltaPosition.sqrMagnitude < float.Epsilon)
            {
                return Overlay(sphere);
            }

            deltaPosition.y = 0f;
            float        distance  = deltaPosition.magnitude;
            Vector3      direction = deltaPosition / distance;
            RaycastHit[] hits      = RaycastHitPool.MiddleRaycastHit();
            int          castCount = Physics.SphereCastNonAlloc(sphere.Position, sphere.Radius, direction, hits, distance, s_CanCastLayerMask);
            return new CrashResult(castCount, hits);
        }

        public static CrashResult Overlay(Collider collider)
        {
            CrashResult result;
            switch (collider)
            {
                case BoxCollider temp:
                    result = Overlay(new Box(temp));
                    break;
                case SphereCollider temp:
                    result = Overlay(new Sphere(temp));
                    break;
                case CapsuleCollider temp:
                    result = Overlay(new Sphere(temp));
                    break;
                default:
                    result = new CrashResult();
                    Debugger.LogError($"不支持的Collider类型{collider.GetType()} : {collider}");
                    break;
            }

            return result;
        }

        public static CrashResult Overlay(Box box)
        {
            Collider[] hits      = RaycastHitPool.MiddleOverlay();
            int        castCount = Physics.OverlapBoxNonAlloc(box.Position, box.HalfSize, hits, box.Rotation, s_CanCastLayerMask);
            s_Compare.Center = box.Position;
            Array.Sort(hits, 0, castCount, s_Compare);
#if UNITY_EDITOR
            Debug.DrawLine(box.Position + box.Rotation * new Vector3(-box.HalfSize.x, 0f, -box.HalfSize.z), box.Position + box.Rotation * new Vector3(-box.HalfSize.x, 0f, box.HalfSize.z),  Color.red, 0.2f);
            Debug.DrawLine(box.Position + box.Rotation * new Vector3(-box.HalfSize.x, 0f, box.HalfSize.z),  box.Position + box.Rotation * new Vector3(box.HalfSize.x,  0f, box.HalfSize.z),  Color.red, 0.2f);
            Debug.DrawLine(box.Position + box.Rotation * new Vector3(box.HalfSize.x,  0f, box.HalfSize.z),  box.Position + box.Rotation * new Vector3(box.HalfSize.x,  0f, -box.HalfSize.z), Color.red, 0.2f);
            Debug.DrawLine(box.Position + box.Rotation * new Vector3(box.HalfSize.x,  0f, -box.HalfSize.z), box.Position + box.Rotation * new Vector3(-box.HalfSize.x, 0f, -box.HalfSize.z), Color.red, 0.2f);
#endif
            return new CrashResult(castCount, hits);
        }

        public static CrashResult Overlay(Sphere sphere)
        {
            Collider[] hits      = RaycastHitPool.MiddleOverlay();
            int        castCount = Physics.OverlapSphereNonAlloc(sphere.Position, sphere.Radius, hits, s_CanCastLayerMask);
            s_Compare.Center = sphere.Position;
            Array.Sort(hits, 0, castCount, s_Compare);
            return new CrashResult(castCount, hits);
        }

        private class ColliderCompare : IComparer<Collider>
        {
            public Vector3 Center;
            
            public int Compare(Collider x, Collider y)
            {
                return (x.transform.position - Center).sqrMagnitude.CompareTo((y.transform.position - Center).sqrMagnitude);
            }
        }
    }
}