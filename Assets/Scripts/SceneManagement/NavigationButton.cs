using Events;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    [RequireComponent(typeof(Button))]
    public class NavigationButton : MonoBehaviour
    {
        public SceneId SceneId;
        public bool Additive;

        private Button m_Button;

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_Button = GetComponent<Button>();
        }
#endif

        private void Awake()
        {
            m_Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            using var evt = SceneEvent.Get(SceneId, Additive).SendGlobal((int)SceneEventType.LoadRequest);
        }
    }
}