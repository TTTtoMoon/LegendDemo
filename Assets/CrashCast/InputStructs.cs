using UnityEngine;

namespace RogueGods.Gameplay
{
    /// <summary>
    /// 方形
    /// </summary>
    public struct Box
    {
        private const float MinHeight = 10f;
        
        public Box(BoxCollider collider)
            : this(collider.transform.position + collider.center, collider.transform.rotation, collider.size / 2f)
        {
        }

        public Box(Vector3 position, Quaternion rotation, Vector3 halfSize)
        {
            Position   = position;
            Rotation   = rotation;
            HalfSize   = halfSize;
            HalfSize.y = Mathf.Min(MinHeight);
            Position.y = 0f;
        }

        public Vector3    Position;
        public Quaternion Rotation;
        public Vector3    HalfSize;
    }

    /// <summary>
    /// 圆形
    /// </summary>
    public struct Sphere
    {
        public Sphere(SphereCollider collider)
            : this(collider.transform.position + collider.center, collider.radius)
        {
        }
        
        public Sphere(CapsuleCollider collider)
            : this(collider.transform.position + collider.center, collider.radius)
        {
        }

        public Sphere(Vector3 position, float radius)
        {
            Position   = position;
            Radius     = radius;
            Position.y = 0f;
        }

        public Vector3 Position;
        public float   Radius;
    }
}