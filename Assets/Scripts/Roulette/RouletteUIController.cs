using System.Collections;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
    public class RouletteUIController : MonoBehaviour
    {
        public GameObject PlayerWalletGo;
        public ParticleSystem WalletWinParticleSystem;
        public Camera Camera;

        public Button SpinButton;

        private void Awake()
        {
            SpinButton.onClick.AddListener(OnSpin);
        }

        private void OnSpin()
        {
            using var evt = RouletteEvent.Get().SendGlobal((int)RouletteEventType.Spin);
        }

        private bool m_MoveToPosition = false;
        
        [Button]
        private void PlayParticles()
        { 
            WalletWinParticleSystem.Play();

            StartCoroutine(MoveToPositionDelay());
        }

        private IEnumerator MoveToPositionDelay()
        {
            yield return new WaitForSeconds(0.5f);

            m_MoveToPosition = true;
            
            yield return new WaitForSeconds(2f);

            m_MoveToPosition = false;
        }
        
        private void Update()
        {
            if (!m_MoveToPosition)
            {
                return;
            }
            
            Vector3 worldPosition = PlayerWalletGo.transform.position;
            Vector3 viewportPoint = Camera.ScreenToViewportPoint(worldPosition);
            
            viewportPoint = new Vector3(viewportPoint.x, viewportPoint.y, 20);

            var screenPosition = Camera.ViewportToWorldPoint(viewportPoint);
            
            Debug.Log(screenPosition);

            screenPosition = new Vector3(screenPosition.x, screenPosition.y, 0f);

            // Get and modify particle data
            int numParticles = WalletWinParticleSystem.main.maxParticles;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
            WalletWinParticleSystem.GetParticles(particles);

            for (int i = 0; i < numParticles; i++)
            {
                particles[i].position = Vector3.MoveTowards(particles[i].position, screenPosition, Time.deltaTime * 20);
            }

            WalletWinParticleSystem.SetParticles(particles);
        }
    }
}