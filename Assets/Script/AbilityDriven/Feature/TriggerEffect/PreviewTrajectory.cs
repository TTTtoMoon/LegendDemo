using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using UnityEngine;

namespace RogueGods.Gameplay.AbilityDriven.TriggerEffect
{
    [Serializable]
    [EffectWithoutTarget]
    [Description("弹道预览")]
    public class PreviewTrajectory : TriggerFeatureEffect
    {
        [Description("预览线")]
        public GameObject Line;
        
        [Description("预览射线宽度")]
        public float PreviewWidth;
        
        [Description("预览时长")]
        public float PreviewDuration;

        protected override void OnEnable()
        {
        }

        protected override void OnDisable()
        {
        }

        protected override void Invoke(TargetCollection targets)
        {
            OrbEmission orbEmission = GetOrbEmission();
            if (orbEmission == null) return;
            GameManager.Instance.StartCoroutine(Preview());

            IEnumerator Preview()
            {
                List<PreviewData>  previewList  = ListPool<PreviewData>.Get();
                List<LineRenderer> previewLines = ListPool<LineRenderer>.Get();
                orbEmission.PreviewTrajectory(previewList);
                for (int i = 0; i < previewList.Count; i++)
                {
                    int          reboundCount  = orbEmission.WhenHitWall is ReboundOrbWhenHitWall reboundOrbWhenHitWall ? reboundOrbWhenHitWall.ReboundTimes.IntValue : 0;
                    bool         crossWall     = orbEmission.WhenHitWall is DoNothingWhenHitWall;
                    Vector3      shotPosition  = previewList[i].ShotPosition;
                    Vector3      shotDirection = previewList[i].ShotDirection;
                    LineRenderer line          = GameManager.LineRenderSystem.CreatRay(Line, shotPosition, shotDirection, reboundCount, crossWall, PreviewWidth);
                    previewLines.Add(line);
                }

                float startTime = Time.time;
                while (startTime + PreviewDuration > Time.time)
                {
                    yield return null;
                    if (Ability == null || Ability.IsActive == false)
                    {
                        break;
                    }

                    previewList.Clear();
                    orbEmission.PreviewTrajectory(previewList);
                    for (int i = 0; i < previewList.Count; i++)
                    {
                        GameManager.LineRenderSystem.UpdataRay(previewLines[i], previewList[i].ShotPosition, previewList[i].ShotDirection);
                    }
                }

                for (int i = 0; i < previewLines.Count; i++)
                {
                    GameManager.LineRenderSystem.ReleaseRay(previewLines[i]);
                }
            }
            
            OrbEmission GetOrbEmission()
            {
                if (Ability.FindFeature(out AbilityTimelineFeature timelineFeature))
                {
                    for (int i = 0; i < timelineFeature.Frames.Length; i++)
                    {
                        for (int j = 0; j < timelineFeature.Frames[i].Effects.Length; j++)
                        {
                            if (timelineFeature.Frames[i].Effects[j] is OrbEmission result)
                            {
                                return result;
                            }
                        }
                    }
                }

                return null;
            }
        }
    }

    public struct PreviewData
    {
        public PreviewData(Vector3 shotPosition, Quaternion shotRotation)
        {
            ShotPosition  = shotPosition;
            ShotDirection = shotRotation * Vector3.forward;
        }

        public Vector3 ShotPosition;
        public Vector3 ShotDirection;
    }
}