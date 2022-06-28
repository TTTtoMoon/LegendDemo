using UnityEngine;

namespace RogueGods
{
    public class SafeAreaAdaptive : MonoBehaviour
    {
        private void Awake()
        {
            var safeArea = Screen.safeArea;
            var offset = new Vector3(0,    Mathf.Abs(Screen.height - safeArea.height));
            if (transform is RectTransform)
            {
                ((RectTransform)this.transform).localPosition -= offset;
            }            
        }
    }
}
