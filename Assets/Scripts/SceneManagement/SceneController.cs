using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    /// <summary>
    /// Used in additive scenes to toggle scene objects
    /// </summary>
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_DestroyWhenAdditive = new List<GameObject>();

        [SerializeField]
        private List<CanvasGroup> m_CanvasGroups = new List<CanvasGroup>();

        [SerializeField]
        [HideInInspector]
        private List<Canvas> m_Canvases;

        [SerializeField]
        private List<GameObject> m_GameObjects = new List<GameObject>();

        [SerializeField]
        private List<MonoBehaviour> m_Components = new List<MonoBehaviour>();


#if UNITY_EDITOR
        private void OnValidate()
        {
            m_Canvases = m_CanvasGroups.Select(cg => cg.GetComponent<Canvas>()).ToList();
        }
#endif

        [SerializeField]
        private bool dontDisable = false;

        private void Start()
        {
            if (!dontDisable && SceneManager.sceneCount > 1)
            {
                OnLoadedAdditively();
            }
        }

        public void OnLoadedAdditively()
        {
            for (var i = 0; i < m_DestroyWhenAdditive.Count; i++)
            {
                Destroy(m_DestroyWhenAdditive[i]);
            }

            SetActiveState(false, false);
        }

        public void SetActiveState(bool isActive, bool animate = true)
        {
            float alpha = isActive ? 1f : 0f;

            for (var i = 0; i < m_CanvasGroups.Count; i++)
            {
                CanvasGroup canvasGroup = m_CanvasGroups[i];
                Canvas canvas = m_Canvases[i];

                if (isActive)
                {
                    canvas.enabled = true;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }

                canvasGroup.DOKill();
                StartCoroutine(TurnOffCanvas(canvas, canvasGroup, isActive, alpha));
            }

            for (int i = 0; i < m_GameObjects.Count; i++)
            {
                var go = m_GameObjects[i];
                go.SetActive(isActive);
            }

            for (var i = 0; i < m_Components.Count; i++)
            {
                m_Components[i].enabled = isActive;
            }
        }

        private IEnumerator TurnOffCanvas(Canvas canvas, CanvasGroup canvasGroup, bool isActive, float alpha)
        {
            yield return new WaitForSeconds(1f);

            canvasGroup.alpha = alpha;
            if (isActive) yield break;
            
            canvas.enabled = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void AddToCanvasGroups(CanvasGroup cg)
        {
            m_CanvasGroups.Add(cg);
            m_Canvases.Add(cg.GetComponent<Canvas>());
        }
    }
}