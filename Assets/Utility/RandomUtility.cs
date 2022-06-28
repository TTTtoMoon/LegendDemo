using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RogueGods.Utility
{
    public static class RandomUtility
    {
        public static RandomWeightObject WeightRandom(List<RandomWeightObject> pool, RandomDataType randomDataType = RandomDataType.None)
        {
            if (pool.Count == 1)
            {
                if (randomDataType == RandomDataType.Room)
                {
                    Debugger.Log(Color.cyan, $"本次加入池子的房间类型分布只有【{pool[0].ID}】");
                }

                return pool[0];
            }

            var totalWeight = 0f;

            var debugInfo = new StringBuilder();

            switch (randomDataType)
            {
                case RandomDataType.GodWishQuality:
                    debugInfo.Append("本次加入池中的品质有:");
                    break;
                case RandomDataType.GodWishType:
                    debugInfo.Append("本次加入池中的祝福能力类型有:");
                    break;
                case RandomDataType.Room:
                    debugInfo.Append("本次加入池中的待选房间分布区域为:");
                    break;
            }

            foreach (var randomObject in pool)
            {
                var randomWeightObject = randomObject;
                randomWeightObject.SectionMin =  totalWeight;
                totalWeight                   += randomWeightObject.Weight;
                randomWeightObject.SectionMax =  totalWeight;

                if (randomDataType == RandomDataType.GodWishQuality || randomDataType == RandomDataType.GodWishType)
                {
                    switch (randomWeightObject.ID)
                    {
                        case 1:
                            if (randomDataType == RandomDataType.GodWishQuality)
                            {
                                debugInfo.Append($" 【白色品质,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]】 ");
                            }
                            else if (randomDataType == RandomDataType.GodWishType)
                            {
                                debugInfo.Append($" 【常规能力,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]】 ");
                            }

                            break;

                        case 2:
                            if (randomDataType == RandomDataType.GodWishQuality)
                            {
                                debugInfo.Append($" 【蓝色品质,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]】 ");
                            }
                            else if (randomDataType == RandomDataType.GodWishType)
                            {
                                debugInfo.Append($" 【双重能力,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]】 ");
                            }

                            break;

                        case 3:
                            if (randomDataType == RandomDataType.GodWishQuality)
                            {
                                debugInfo.Append($" 【紫色品质,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]】 ");
                            }
                            else if (randomDataType == RandomDataType.GodWishType)
                            {
                                debugInfo.Append($" 【传奇能力,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]】 ");
                            }

                            break;

                        case 4:
                            debugInfo.Append($" 【橙色品质,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]】 ");
                            break;
                    }
                }
                else if (randomDataType == RandomDataType.Room)
                {
                    debugInfo.Append($"【房间分布区域【{randomWeightObject.ID}】,随机区间为[{randomWeightObject.SectionMin},{randomWeightObject.SectionMax}]");
                }
            }

            var randomFactor = (int)Random.Range(0, totalWeight * 100000) / 100000;
            var target       = pool.FirstOrDefault(section => randomFactor >= section.SectionMin && randomFactor < section.SectionMax);
            // if (randomDataType != RandomDataType.None)
            // {
            //     debugInfo.Append($"。本次随机的结果为【{randomFactor}】。");
            //     Debugger.Log(Color.cyan,debugInfo.ToString());
            // }

            return target;
        }

        /// <summary>
        /// 概率命中
        /// </summary>
        /// <param name="probability">概率0.0~1.0</param>
        /// <returns>返回是否命中</returns>
        public static bool Probability(float probability)
        {
            return probability >= Random.value;
        }

        /// <summary>
        /// 权重随机
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public static T WeightRandom<T>(List<WeightObject<T>> pool)
        {
            if (pool.Count == 1) return pool[0].Object;
            int totalWeight = 0;
            for (int i = 0; i < pool.Count; i++)
            {
                totalWeight += pool[i].Weight;
            }

            int random = Random.Range(0, totalWeight);
            totalWeight = 0;
            for (int i = 0; i < pool.Count; i++)
            {
                totalWeight += pool[i].Weight;
                if (random < totalWeight)
                {
                    return pool[i].Object;
                }
            }

            throw new Exception();
        }
    }

    public class RandomWeightObject
    {
        public int   ID;
        public float Weight;
        public float SectionMin;
        public float SectionMax;
    }

    public enum RandomDataType
    {
        None,
        GodWishQuality,
        GodWishType,
        Room
    }

    public struct WeightObject<T>
    {
        public WeightObject(T obj, int weight)
        {
            Object = obj;
            Weight = weight;
        }

        public T   Object;
        public int Weight;
    }
}