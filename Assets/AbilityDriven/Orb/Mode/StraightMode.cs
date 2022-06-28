using System;
using System.Collections.Generic;
using Abilities;
using RogueGods.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace RogueGods.Gameplay.AbilityDriven
{
    [Serializable]
    [Description("直射模式")]
    public sealed class StraightMode : OrbMode
    {
        [Description("移动速度(米/秒)")]
        public AbilityVariable MoveSpeed = new AbilityVariable(10f);

        [Description("可穿透数量")]
        public AbilityVariable StrikeCount = new AbilityVariable(0);
        
        [Description("可弹射数量")]
        public AbilityVariable PingPongCount = new AbilityVariable(0);

        [Description("弹射半径")]
        public AbilityVariable PingPongRadius = new AbilityVariable(10f);

        [Description("弹射攻击系数修改")]
        public AbilityVariable PingPongAttackMultipleModifier = new AbilityVariable(0f);

        private int                     m_StrikeCounter = 0;
        private HashSet<IAbilityTarget> m_CrashedTargets;
        private DamageModifier          m_DamageModifier;

        protected override void OnEnable()
        {
            GameManager.Instance.RegisterUpdate(UpdateTransform, order: GameManager.UpdateMonoOrder.UpdateTransform);
            m_CrashedTargets =  HashSetPool<IAbilityTarget>.Get();
            Orb.OnCrash.AddListener(OnCrash);
            m_DamageModifier                        =  OnPrepareMakeDamage;
            GameManager.DamageSystem.DamageModifier += m_DamageModifier;
            
            void OnPrepareMakeDamage(in DamageRequest damageRequest, ref Attacker attacker, ref Defender defender)
            {
                if (damageRequest.Ability != Orb.Ability) return;
                attacker.DamagePower += m_CrashedTargets.Count * PingPongAttackMultipleModifier.Value;
                DamageSystem.PushModify("弹射减伤", attacker);
            }
        }

        protected override void OnDisable()
        {
            m_StrikeCounter = 0;
            HashSetPool<IAbilityTarget>.Release(m_CrashedTargets);
            m_CrashedTargets                        =  null;
            GameManager.DamageSystem.DamageModifier -= m_DamageModifier;
            Orb.OnCrash.RemoveListener(OnCrash);
            GameManager.Instance.UnRegisterUpdate(UpdateTransform);
        }

        private void UpdateTransform()
        {
            if (ApplyRotate())
            {
                return;
            }

            float moveDistance = Time.deltaTime * Orb.MoveSpeedMultiplier * MoveSpeed;
            Orb.TranslatePosition(moveDistance  * Orb.transform.forward);
        }

        private bool OnCrash(IAbilityTarget target)
        {
            if (Orb.TargetFilter.Verify(target) == false)
            {
                return false;
            }

            if (m_StrikeCounter >= StrikeCount && m_CrashedTargets.Count >= PingPongCount)
            {
                GameManager.OrbSystem.Destroy(Orb, OrbDestroyOption.DestroyWhenHitActor);
                return false;
            }

            if (m_CrashedTargets.Add(target) == false)
            {
                return false;
            }

            m_StrikeCounter++;
            if (m_CrashedTargets.Count <= PingPongCount)
            {
                PingPong();
            }

            return true;

            void PingPong()
            {
                using CrashResult result = CrashCaster.Overlay(new Sphere(target.Position, PingPongRadius));
                for (int i = 0; i < result.Count; i++)
                {
                    if (Equals(result[i], target) || RaycastAnyWall(target.Position, result[i].transform.position))
                    {
                        continue;
                    }

                    IAbilityTarget temp = result[i];
                    if (Equals(temp, Orb.Ability.Source)       ||
                        temp.IsValidAndActive()       == false ||
                        Orb.TargetFilter.Verify(temp) == false ||
                        m_CrashedTargets.Contains(temp))
                    {
                        continue;
                    }

                    Rotate(target.Position, (temp.Position - target.Position).IgnoreY().normalized);
                    return;
                }

                GameManager.OrbSystem.Destroy(Orb, OrbDestroyOption.DestroyWhenHitActor);
            }

            bool RaycastAnyWall(Vector3 start, Vector3 end)
            {
                return Physics.Linecast(start, end, LayerDefine.Obstacle.Mask | LayerDefine.Wall.Mask);
            }
        }
    }
}