using UnityEngine;
using UnityEngine.EventSystems;

namespace RogueGods.Gameplay.UI
{
    public sealed class UISystem : GameSystem
    {
        private const string UI_ROOT = "UIRoot";

        public Canvas      Canvas      { get; private set; }
        public EventSystem EventSystem { get; private set; }

        public override void Awake()
        {
            base.Awake();
            GameObject uiRoot = Resources.Load<GameObject>(UI_ROOT);
            uiRoot      = Object.Instantiate(uiRoot);
            uiRoot.name = UI_ROOT;
            Object.DontDestroyOnLoad(uiRoot);

            Canvas      = uiRoot.GetComponentInChildren<Canvas>();
            EventSystem = uiRoot.GetComponentInChildren<EventSystem>();
        }
    }
}