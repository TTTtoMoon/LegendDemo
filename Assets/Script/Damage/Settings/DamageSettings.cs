using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using Debugger = RogueGods.Utility.Debugger;

namespace RogueGods.Gameplay
{
    //[CreateAssetMenu(menuName = "RogueGods/DamageSettings")]
    public class DamageSettings : ScriptableObject
    {
        #region 基础参数配置

        [TabGroup("基础参数配置")]
        [SerializeField] [LabelText("暴击倍率")] [Min(1f)]
        private float m_CriticalPower = 2f;

        public float CriticalPower => m_CriticalPower;
        
        [TabGroup("基础参数配置")]
        [SerializeField] [LabelText("最大减伤率")] [Range(0f, 1f)]
        private float m_MaxDamageDeductions = 0.95f;

        public float MaxDamageDeductions => m_MaxDamageDeductions;

        #endregion
    
        #region 击中效果配置

        [TabGroup("击中效果配置")]
        [SerializeField] [LabelText("通用击中音效")]
        private AudioClip m_DamageImpactDefaultAudio;

        [TabGroup("击中效果配置")]
        [SerializeField] [LabelText("通用击中特效")]
        private GameObject m_DamageImpactDefaultVFX;

        [TabGroup("击中效果配置")]
        [SerializeField] [LabelText("特效缩放")] [TableList(AlwaysExpanded = true)] 
        private DamageImpactConfig.VFXScale[] m_DamageImpactVFXScales;
        
        [TabGroup("击中效果配置")]
        [SerializeField] [LabelText("击中效果")] [ListDrawerSettings(Expanded = true, ListElementLabelName = "Name")] 
        private DamageImpactConfig[] m_DamageImpactSettings;

        public DamageImpactSetting GetDamageImpactSetting(DamageMaterial damageMaterial, BodyMaterial takerMaterial, BodyType takerBodyType)
        {
            AudioClip  audio     = null;
            GameObject vfx       = null;
            bool       vfxFollow = false;
            float      vfxScale  = 1f;

            for (int i = 0, length = m_DamageImpactSettings.Length; i < length; i++)
            {
                ref DamageImpactConfig setting = ref m_DamageImpactSettings[i];
                if (setting.DamageMaterial == damageMaterial &&
                    setting.TakerMaterial  == takerMaterial)
                {
                    audio     = setting.Audio;
                    vfx       = setting.VFX;
                    vfxFollow = setting.VFXFollow;
                    break;
                }
            }
            
            audio = audio == null ? m_DamageImpactDefaultAudio : audio;
            vfx   = vfx   == null ? m_DamageImpactDefaultVFX : vfx;
            LogDamageImpactWarning(damageMaterial, takerMaterial, audio, vfx);

            for (int i = 0, length = m_DamageImpactVFXScales.Length; i < length; i++)
            {
                ref DamageImpactConfig.VFXScale setting = ref m_DamageImpactVFXScales[i];
                if (takerBodyType== setting.Body)
                {
                    vfxScale = setting.Scale;
                }
            }

            return new DamageImpactSetting(audio, vfx, vfxFollow, vfxScale);
        }
        
        [Conditional("DEBUGGER")]
        private static void LogDamageImpactWarning(DamageMaterial damageMaterial, BodyMaterial takerMaterial, AudioClip audio, GameObject vfx)
        {
            string info    = $"无法找到{damageMaterial}针对对{takerMaterial}的打击";
            bool   needLog = false;
            if (audio == null)
            {
                info    += "[音效]";
                needLog =  true;
            }

            if (vfx == null)
            {
                info    += "[特效]";
                needLog =  true;
            }

            if (needLog) Debugger.LogWarning(info);
        }

        #endregion

        #region 击退效果配置

        [TabGroup("击退效果配置")]
        [SerializeField] [LabelText("击退效果")] [TableList(AlwaysExpanded = true)]
        private DamageRetreatSetting[] m_DamageRetreatSettings;

        public DamageRetreatSetting GetRetreatSetting(BodyType bodyType)
        {
            for (int i = 0, length = m_DamageRetreatSettings.Length; i < length; i++)
            {
                if (m_DamageRetreatSettings[i].BodyType == bodyType)
                {
                    return m_DamageRetreatSettings[i];
                }
            }

            Debugger.LogError($"无法找到{bodyType}体型对应的击退曲线。");
            return DamageRetreatSetting.Default;
        }
        
        #endregion
    }
}