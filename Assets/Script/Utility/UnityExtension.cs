using Unity.Mathematics;
using UnityEngine;

namespace RogueGods.Utility
{
    public static class UnityExtension
    {
        private static Vector3[] s_CornersCache = new Vector3[4];
        
        public static TComponent GetOrAddComponent<TComponent>(this Component _this) where TComponent : Component
        {
            return _this.gameObject.GetOrAddComponent<TComponent>();
        }
        
        public static TComponent GetOrAddComponent<TComponent>(this GameObject _this) where TComponent : Component
        {
            if (_this.TryGetComponent(out TComponent component))
            {
                return component;
            }

            return _this.AddComponent<TComponent>();
        }

        public static GameObject Find(this GameObject _this, string path)
        {
            return _this.transform.Find(path).gameObject;
        }

        public static T GetComponentAtChild<T>(this GameObject _this, string path)
        {
            return Find(_this, path).GetComponent<T>();
        }

        public static T GetComponentAtChild<T>(this Transform _this, string path)
        {
            return Find(_this.gameObject, path).GetComponent<T>();
        }

        public static float GetNormalizedTime(this AnimatorStateInfo _this)
        {
            return _this.loop ? Mathf.Repeat(_this.normalizedTime, 1f) : _this.normalizedTime;
        }

        public static float2 ToFloat2(this Vector3 _this)
        {
            return new float2(_this.x, _this.z);
        }

        /// <summary>
        /// 计算位置
        /// </summary>
        public static float2 OffsetPosition(this Transform _this, float2 offset)
        {
            float2  position2D  = _this.position.ToFloat2();
            float2  offset2D    = offset;
            Vector3 offset3D    = new Vector3(offset2D.x, 0f, offset2D.y);
            Vector3 eulerAngles = _this.eulerAngles;
            offset3D   =  Quaternion.Euler(0f, eulerAngles.y, 0f) * offset3D;
            offset2D.x =  offset3D.x;
            offset2D.y =  offset3D.z;
            position2D += offset2D;
            return position2D;
        }

        public static Vector2 ToVector2(this Vector3 _this)
        {
            return new Vector2(_this.x, _this.z);
        }

        public static Vector2 Rotation(this Vector2 _this, float angle)
        {
            float x    = _this.x;
            float y    = _this.y;
            float sin  = Mathf.Sin(Mathf.PI * angle / 180);
            float cos  = Mathf.Cos(Mathf.PI * angle / 180);
            float newX = x * cos  + y * sin;
            float newY = x * -sin + y * cos;
            _this.x = newX;
            _this.y = newY;
            return _this;
        }

        /// <summary>
        /// 进行一次2D偏移
        /// </summary>
        /// <param name="_this">原点</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static Vector3 Offset(this Transform _this, Vector2 offset)
        {
            Vector3 position = _this.position;
            offset     =  offset.Rotation(_this.eulerAngles.y);
            position.x += offset.x;
            position.z += offset.y;
            return position;
        }

        /// <summary>
        /// 获取旋转方向
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Quaternion ToRotation(this Vector3 _this)
        {
            return Quaternion.LookRotation(_this.normalized);
        }

        /// <summary>
        /// 忽略y轴
        /// </summary>
        /// <param name="_this"></param>
        /// <returns></returns>
        public static Vector3 IgnoreY(this Vector3 _this)
        {
            _this.y = 0f;
            return _this;
        }
        
        /// <summary>
        /// 将向量各数值相乘
        /// </summary>
        public static Vector3 Multiple(this Vector3 _this, Vector3 other)
        {
            _this.x *= other.x;
            _this.y *= other.y;
            _this.z *= other.z;
            return _this;
        }
        
        public static bool Overlaps(this RectTransform _this, RectTransform other)
        {
            return _this.GetWorldRect().Overlaps(other.GetWorldRect());
        }

        public static Rect GetWorldRect(this RectTransform _this)
        {
            _this.GetWorldCorners(s_CornersCache);
            float x      = s_CornersCache[0].x;
            float y      = s_CornersCache[0].y;
            float width  = s_CornersCache[2].x - x;
            float height = s_CornersCache[2].y - y;
            return new Rect(x, y, width, height);
        }

        public static Rect GetWorldRect(this RectTransform _this, Rect rect)
        {
            float x    = rect.x;
            float y    = rect.y;
            float xMax = rect.xMax;
            float yMax = rect.yMax;
            s_CornersCache[0] = new Vector3(x,    y,    0.0f);
            s_CornersCache[1] = new Vector3(x,    yMax, 0.0f);
            s_CornersCache[2] = new Vector3(xMax, yMax, 0.0f);
            s_CornersCache[3] = new Vector3(xMax, y,    0.0f);
            Matrix4x4 localToWorldMatrix = _this.localToWorldMatrix;
            for (int index = 0; index < 4; ++index)
            {
                s_CornersCache[index] = localToWorldMatrix.MultiplyPoint(s_CornersCache[index]);
            }

            x = s_CornersCache[0].x;
            y = s_CornersCache[0].y;
            float width  = s_CornersCache[2].x - x;
            float height = s_CornersCache[2].y - y;
            return new Rect(x, y, width, height);
        }

        public static Vector3 GetWorldCenter(this RectTransform _this)
        {
            _this.GetWorldCorners(s_CornersCache);
            return (s_CornersCache[0] + s_CornersCache[2]) / 2;
        }
    }
}