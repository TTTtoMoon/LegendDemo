using UnityEngine;
using UnityEngine.UI;

namespace RogueGods.Gameplay.Blood
{
    public sealed class HeaderBlood : MonoBehaviour
    {
        [SerializeField] private Image m_Blood;

        private RectTransform m_RectTransform;
        private Actor         m_Actor;

        public static HeaderBlood CreateBlood(HeaderBlood prefab, Actor actor)
        {
            HeaderBlood instance = Instantiate(prefab, GameManager.UISystem.Canvas.transform);
            instance.m_Actor             =  actor;
            actor.OnCurrentHealthChanged += instance.ActorOnOnCurrentHealthChanged;
            actor.Attribute.AddOnValueChanged(AttributeType.MaxHealth, instance.OnMaxHealthValueChanged);
            return instance;
        }

        private void Awake()
        {
            m_RectTransform = transform as RectTransform;
        }

        private void OnDestroy()
        {
            if (m_Actor == null) return;
            m_Actor.OnCurrentHealthChanged -= ActorOnOnCurrentHealthChanged;
            m_Actor.Attribute.RemoveOnValueChanged(AttributeType.MaxHealth, OnMaxHealthValueChanged);
        }

        private void OnMaxHealthValueChanged(float oldValue, float newValue)
        {
            UpdateBlood();
        }

        private void ActorOnOnCurrentHealthChanged(float oldValue, float newValue)
        {
            UpdateBlood();
        }

        private void UpdateBlood()
        {
            if (m_Actor == null) return;
            m_Blood.fillAmount = m_Actor.CurrentHealth / m_Actor.Attribute[AttributeType.MaxHealth];
        }

        private void LateUpdate()
        {
            m_RectTransform.anchoredPosition = RectTransformUtility.WorldToScreenPoint(GameManager.MainCamera, m_Actor.Position + 2f * Vector3.up);
        }
    }
}