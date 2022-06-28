using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RogueGods.Utility
{
    public static class Utilities
    {
        public static async void RunAsync(this Task task)
        {
            await task;
        }

        public static void RunSync(this Task task)
        {
            while (task.Status == TaskStatus.Running)
            {
            }
        }

        public static float2 ThirdPersonToTopCoordinate(float3 float3)
        {
            return new float2(float3.x, float3.z);
        }

        public static float[] WeightToProbability(float[] weight)
        {
            //获取总权值
            float allWeight = 0;
            for (int i = 0; i < weight.Length; i++)
            {
                allWeight += weight[i];
            }

            float[] probability = new float[weight.Length];
            for (int i = 0; i < weight.Length; i++)
            {
                probability[i] = weight[i] / allWeight;
            }

            return probability;
        }

        /// <summary>
        ///     根据权重随机
        /// </summary>
        /// <param name="probabilityList">存有权重的list</param>
        /// <param name="num">抽取的数量</param>
        /// <returns></returns>
        public static int RandomByProbability(float[] probabilityList)
        {
            //获取总权值
            float allProbability                                            = 0;
            for (int i = 0; i < probabilityList.Length; i++) allProbability += probabilityList[i];

            //获得一份权值化为百分比的倍数
            float multiple = 100 / allProbability;

            //化为百分比的概率
            float[] dealProbability = new float[probabilityList.Length];
            allProbability = 0;
            for (int i = 0; i < probabilityList.Length; i++)
            {
                dealProbability[i] =  allProbability + probabilityList[i] * multiple;
                allProbability     += probabilityList[i] * multiple;
            }

            float randomValue = Random.value * 100;

            //如果这个值直接小于第一个了，就说明这个值是第一个
            if (randomValue < dealProbability[0])
            {
                return 0;
            }

            for (int i = 1; i < dealProbability.Length; i++)
                if (randomValue <= dealProbability[i] && randomValue >= dealProbability[i - 1])
                    return i;
            Debugger.LogWarning("竟然真出现了第三种情况");
            return 0;
        }

        public static Vector2 GetUGUIPositionFromWorldPosition(Camera camera, RectTransform canvasRectTransform, Vector3 worldPosition)
        {
            Vector2 screenPoint = camera.WorldToScreenPoint(worldPosition);
            Vector2 screenSize  = new Vector2(Screen.width, Screen.height);
            screenPoint -= screenSize / 2;
            Vector2 anchorPos = screenPoint / screenSize * canvasRectTransform.sizeDelta;
            return anchorPos;
        }

        private static readonly double[] byteUnits =
        {
            1073741824.0, 1048576.0, 1024.0, 1
        };

        private static readonly string[] byteUnitsNames =
        {
            "GB", "MB", "KB", "B"
        };

        public static string FormatBytes(long bytes)
        {
            var size = "0 B";
            if (bytes == 0) return size;

            for (var index = 0; index < byteUnits.Length; index++)
            {
                var unit = byteUnits[index];
                if (bytes >= unit)
                {
                    size = $"{bytes / unit:##.##} {byteUnitsNames[index]}";
                    break;
                }
            }

            return size;
        }

        public static bool Contains<T>(this T[] _this, T item) where T : class
        {
            for (int i = 0; i < _this.Length; i++)
            {
                if (_this[i] == item)
                {
                    return true;
                }
            }

            return false;
        }

        public static T ElementAtIndexSafety<T>(this List<T> list, int index)
        {
            if (list == null || list.Count == 0) return default;
            return list[Mathf.Clamp(index, 0, list.Count - 1)];
        }

        public static T ElementAtIndexSafety<T>(this T[] array, int index)
        {
            if (array == null || array.Length == 0) return default;
            return array[Mathf.Clamp(index, 0, array.Length - 1)];
        }
        
        public static T ElementAtIndexSafety<T>(this List<T> list, int index, T fallbackWhenListEmpty)
        {
            if (list == null || list.Count <= index || index < 0) return fallbackWhenListEmpty;
            return list[index];
        }

        public static T ElementAtIndexSafety<T>(this T[] array, int index, T fallbackWhenArrayEmpty)
        {
            if (array == null || array.Length <= index || index < 0) return fallbackWhenArrayEmpty;
            return array[index];
        }

        public static TValue GetValueSafety<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue fallback = default)
        {
            if (dictionary == null) return fallback;
            return dictionary.TryGetValue(key, out TValue result) ? result : fallback;
        }

        public static bool EqualsIgnoreEmpty(this string _this, string other)
        {
            if (string.IsNullOrEmpty(_this) && string.IsNullOrEmpty(other))
            {
                return true;
            }

            return _this == other;
        }

        public static Vector3 RandomNavPositionInRing(Vector3 origin, float minRadius, float maxRadius)
        {
            Vector2 random = Random.insideUnitCircle;
            origin.x = minRadius + (maxRadius - minRadius) * random.x;
            origin.z = minRadius + (maxRadius - minRadius) * random.y;
            return origin;
        }
    }
}