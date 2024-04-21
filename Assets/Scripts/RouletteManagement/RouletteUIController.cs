using Events;
using UnityEngine;
using UnityEngine.UI;

namespace RouletteManagement
{
    public class RouletteUIController : MonoBehaviour
    {
        public Button SpinButton;
        public Button AutoButton;

        private bool m_AutoOn;

        private void Awake()
        {
            ToggleSpinButton(false);
            SpinButton.onClick.AddListener(OnSpin);
            AutoButton.onClick.AddListener(OnAuto);

            GEM.AddListener<RouletteEvent>(OnSpinEventEnd, channel: (int)RouletteEventType.SpinSequenceEnd);
            GEM.AddListener<RouletteEvent>(OnRouletteSetupComplete,
                channel: (int)RouletteEventType.SetupComplete);
        }

        private void OnDisable()
        {
            SpinButton.onClick.RemoveListener(OnSpin);
            AutoButton.onClick.RemoveListener(OnAuto);

            GEM.RemoveListener<RouletteEvent>(OnSpinEventEnd, channel: (int)RouletteEventType.SpinSequenceEnd);
            GEM.RemoveListener<RouletteEvent>(OnRouletteSetupComplete,
                channel: (int)RouletteEventType.SetupComplete);
            GEM.RemoveListener<RouletteEvent>(OnAllSelected, channel: (int)RouletteEventType.AllSlotsSelected);
        }

        private void OnRouletteSetupComplete(RouletteEvent evt)
        {
            ToggleSpinButton(true);
        }

        private void OnSpin()
        {
            using var evt = RouletteEvent.Get().SendGlobal((int)RouletteEventType.Spin);
            ToggleSpinButton(false);
        }

        private void OnSpinEventEnd(RouletteEvent evt)
        {
            ToggleSpinButton(true);

            if (m_AutoOn)
                OnSpin();
        }

        private void ToggleSpinButton(bool interactable)
        {
            SpinButton.interactable = interactable;
        }

        private void OnAuto()
        {
            if (m_AutoOn)
            {
                ToggleAuto(false);
                return;
            }

            GEM.AddListener<RouletteEvent>(OnAllSelected, channel: (int)RouletteEventType.AllSlotsSelected);

            ToggleAuto(true);
            OnSpin();
        }

        private void OnAllSelected(RouletteEvent evt)
        {
            ToggleAuto(false);
            GEM.RemoveListener<RouletteEvent>(OnAllSelected, channel: (int)RouletteEventType.AllSlotsSelected);
        }

        private void ToggleAuto(bool on)
        {
            m_AutoOn = on;
            SpinButton.interactable = !on;
        }
    }
}