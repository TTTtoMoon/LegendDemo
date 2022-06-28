using System;
using Abilities;
using RogueGods.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RogueGods.Gameplay.AbilityDriven.PassivityEffect
{
    [Serializable]
    [Description("环绕盾牌")]
    public class AroundShield : AroundEffect
    {
        [Description("筛选抵消子弹")]
        public Filter<Ability> BlockFilter = new Filter<Ability>();

        [Description("盾牌宽度")] 
        public Vector3 ShieldSize = new Vector3(1f, 5f, 0.1f);

        protected override AroundAgent CreateAgent(IAbilityTarget target)
        {
            return new Shield(this);
        }

        private class Shield : AroundAgent
        {
            public Shield(AroundShield effect) : base(effect)
            {
                GameManager.OrbSystem.OnCreateOrb += OnCreateOrb;
                m_Effect                               =  effect;
                m_Shields                              =  new AbilityShield[VFX.Length];
                for (int i = 0, length = VFX.Length; i < length; i++)
                {
                    m_Shields[i]      = VFX[i].gameObject.AddComponent<AbilityShield>();
                    m_Shields[i].Type = effect.Ability.Owner.GetFilterType();
                    BoxCollider box = new GameObject("Trigger").AddComponent<BoxCollider>();
                    box.gameObject.layer = LayerDefine.Gethit.Index;
                    box.transform.SetParent(VFX[i].transform, false);
                    box.size      = effect.ShieldSize;
                    box.isTrigger = true;
                }
            }

            private AroundShield    m_Effect;
            private AbilityShield[] m_Shields;

            protected override void OnDispose()
            {
                GameManager.OrbSystem.OnCreateOrb -= OnCreateOrb;
                for (int i = 0, length = VFX.Length; i < length; i++)
                {
                    Object.DestroyImmediate(VFX[i].gameObject.GetComponent<AbilityShield>());
                    Object.DestroyImmediate(VFX[i].gameObject.GetComponentInChildren<BoxCollider>());
                }
            }

            public override void OnUpdate(Vector3[] prePositions)
            {
            }

            private void OnCreateOrb(Orb orb)
            {
                orb.OnCrash.AddListener(OnCrashOther, -1);

                bool OnCrashOther(IAbilityTarget target)
                {
                    if (ReferenceEquals(orb.Ability.Source, m_Effect.Ability.Source) == false &&
                        target is AbilityShield shield                                        &&
                        m_Shields.Contains(shield)                                            &&
                        m_Effect.BlockFilter.Verify(orb.Ability))
                    {
                        GameManager.OrbSystem.Destroy(orb, OrbDestroyOption.None);
                        return true;
                    }

                    return false;
                }
            }
        }
    }
}