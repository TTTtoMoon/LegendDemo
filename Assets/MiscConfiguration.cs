using RogueGods.Gameplay.AbilityDriven.PassivityEffect;
using RogueGods.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RogueGods
{
    //[CreateAssetMenu(menuName = "RogueGods/杂项配置")]
    public class MiscConfiguration : ScriptableObject
    {
        private static MiscConfiguration m_Instance;

        public static void PreLoad()
        {
            m_Instance =Resources.Load<MiscConfiguration>(nameof(MiscConfiguration));
        }

        #region 战斗
        
        [FoldoutGroup("战斗")]
        [LabelText("怪物碰撞伤害系数")]
        public float CrashDamageCoefficientOfMonster;

        [FoldoutGroup("战斗")]
        [LabelText("环绕技能配置")] [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Title")]
        public AroundConfig[] AroundConfigs = new AroundConfig[0];
        
        [FoldoutGroup("战斗")]
        [LabelText("碰撞伤害时间间隔(秒)")]
        public float CrashDamageTimeSpan = 0.5f;

        [FoldoutGroup("战斗")]
        [LabelText("破盾特效")]
        public GameObject ShieldBrokenVFX;

        [FoldoutGroup("战斗")]
        [LabelText("复活特效")]
        public GameObject ReliveVFX;

        [FoldoutGroup("战斗")]
        [LabelText("暴击震屏：抖动曲线")] [SuffixLabel("x:时间(秒) y:强度")]
        public AnimationCurve CriticalCameraShakeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        /// <summary>
        /// 获取怪物碰撞伤害系数
        /// </summary>
        public static float GetCrashDamageCoefficientOfMonster()
        {
            return m_Instance.CrashDamageCoefficientOfMonster;
        }

        /// <summary>
        /// 获取环绕半径
        /// </summary>
        public static AroundConfig GetAroundConfig(int index)
        {
            return m_Instance.AroundConfigs.ElementAtIndexSafety(index);
        }

        /// <summary>
        /// 获取怪物碰撞伤害系数
        /// </summary>
        public static float GetCrashDamageTimeSpan()
        {
            return m_Instance.CrashDamageTimeSpan;
        }

        /// <summary>
        /// 获取破盾特效guid
        /// </summary>
        public static GameObject GetShieldBrokenVFX()
        {
            return m_Instance.ShieldBrokenVFX;
        }

        /// <summary>
        /// 获取复活特效guid
        /// </summary>
        public static GameObject GetReliveVFX()
        {
            return m_Instance.ReliveVFX;
        }

        #endregion

        #region 抛物线

        [FoldoutGroup("抛物线")] [SerializeField] [LabelText("通用跳跃/抛掷高度，越大影子距离本体越远")]
        public float ConstParabolaHeight;
        
        [FoldoutGroup("抛物线")] [SerializeField] [LabelText("抛物线缩放曲线")] [InfoBox("曲线x轴应在0-1之间表示距离百分比，y轴表示缩放大小")]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public AnimationCurve[] ParabolasScaleCurves = new AnimationCurve[0];
        
        [FoldoutGroup("抛物线")] [SerializeField] [LabelText("抛物线高度曲线")] [InfoBox("曲线x轴应在0-1之间表示距离百分比，y轴表示缩放大小")]
        [ListDrawerSettings(ShowIndexLabels = true)]
        public AnimationCurve[] ParabolaHeightCurves = new AnimationCurve[0];

        /// <summary>
        /// 获取通用跳跃/抛掷高度
        /// </summary>
        /// <returns></returns>
        public static float GetConstParabolaHeight()
        {
            return m_Instance.ConstParabolaHeight;
        }

        /// <summary>
        /// 获取对应高度的抛物线缩放曲线
        /// </summary>
        /// <param name="heightLevel">高度等级</param>
        /// <returns></returns>
        public static AnimationCurve GetParabolaScaleCurve(int heightLevel)
        {
            if (m_Instance.ParabolasScaleCurves.Length == 0) return null;
            return m_Instance.ParabolasScaleCurves[Mathf.Clamp(heightLevel, 0, m_Instance.ParabolasScaleCurves.Length)];
        }
        
        /// <summary>
        /// 获取对应高度的抛物线缩放曲线
        /// </summary>
        /// <param name="heightLevel">高度等级</param>
        /// <returns></returns>
        public static AnimationCurve GetParabolaHeightCurve(int heightLevel)
        {
            if (m_Instance.ParabolaHeightCurves.Length == 0) return null;
            return m_Instance.ParabolaHeightCurves[Mathf.Clamp(heightLevel, 0, m_Instance.ParabolaHeightCurves.Length)];
        }

        #endregion
    }
}