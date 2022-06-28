using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RogueGods.Utility
{
    public static class MathematicUtility
    {
        public static Vector3 CirclePointOfTargetRadius(Vector3 center, float angle, float radius)
        {
            Vector3 pos = Vector3.zero;
            pos.x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            pos.y = center.y;
            pos.z = center.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            return pos;
        }
    }
}