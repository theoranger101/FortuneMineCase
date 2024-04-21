using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Promises;
using Toolkit.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public enum SceneId
    {
        Shared = 0,
        Initial = 1,
        Barbeque = 2,
    }

    public class SceneTransitionManager : SingletonBehaviour<SceneTransitionManager>
    {
        [SerializeField]
        private SceneId m_InitialScene = SceneId.Initial;

        public SceneId CurrentSceneId => m_CurrentSceneId;
        private SceneId m_CurrentSceneId;

        private List<SceneController> m_SceneControllersList;
        private Dictionary<Scene, SceneController> m_SceneControllers;

        private List<Action<SceneId>> m_SceneChangeListeners = new List<Action<SceneId>>();

        private void Awake()
        {
            if (!SetupInstance(false))
                return;

            Initialize();
            GEM.AddListener<SceneEvent>(OnSceneChangeRequest, channel: (int)SceneEventType.LoadRequest);
            GEM.AddListener<SceneEvent>(OnUnloadRequest, channel: (int)SceneEventType.Unload);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<SceneEvent>(OnSceneChangeRequest, channel: (int)SceneEventType.LoadRequest);
            GEM.RemoveListener<SceneEvent>(OnUnloadRequest, channel: (int)SceneEventType.Unload);
        }

        private void Initialize()
        {
            var initialScene = m_InitialScene.ToString();
            var promise = LoadAdditiveScene(initialScene);

            promise.OnResultT += (success, scene) =>
            {
                if (!success)
                {
                    Debug.LogError("Initial scene could not be loaded.");
                }

                m_SceneControllersList = FindObjectsOfType<SceneController>().ToList();
                m_SceneControllers = m_SceneControllersList.ToDictionary(act => act.gameObject.scene);

                ActivateScene(initialScene);
            };
        }

        private void OnSceneChangeRequest(SceneEvent evt)
        {
            bool changed = ChangeScene(evt.SceneId, evt.Additive);

            evt.result = changed ? EventResult.Positive : EventResult.Negative;
        }

        private void OnUnloadRequest(SceneEvent evt)
        {
            UnloadAdditiveScene(evt.SceneId.ToString());
        }

        private bool ChangeScene(SceneId sceneId, bool additive)
        {
            if (sceneId == m_CurrentSceneId)
            {
                Debug.Log($"Tried to activate the same scene {m_CurrentSceneId}.");
                return false;
            }

            var sceneName = sceneId.ToString();
            m_CurrentSceneId = sceneId;

            if (additive)
            {
                var promise = LoadAdditiveScene(sceneName, true);

                promise.OnResultT += (success, scene) =>
                {
                    if (!success)
                    {
                        return;
                    }

                    ActivateScene(scene);
                };
            }
            else
            {
                if (!ActivateScene(sceneName))
                {
                    return false;
                }
            }

            for (var i = 0; i < m_SceneChangeListeners.Count; i++)
            {
                var listener = m_SceneChangeListeners[i];
                listener?.Invoke(m_CurrentSceneId);
            }

            return true;
        }

        private Promise<string> LoadAdditiveScene(string sceneName, bool activate = false)
        {
            var promise = Promise<string>.Create();
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            asyncOperation.completed += operation => { promise.Complete(sceneName); };

            return promise;
        }

        private void UnloadAdditiveScene(string sceneName)
        {
            if (CurrentSceneId.ToString() == sceneName)
            {
                ActivateScene(m_InitialScene.ToString());
            }

            var asyncOperation = SceneManager.UnloadSceneAsync(sceneName);
            asyncOperation.completed += operation => { };
        }

        private bool ActivateScene(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);

            if (!scene.IsValid())
            {
                Debug.LogError($"Scene {sceneName} is not valid");
                return false;
            }

            foreach (var controller in m_SceneControllersList.Where(act =>
                         act.gameObject.scene != scene))
            {
                controller.SetActiveState(false);
            }

            if (m_SceneControllers.TryGetValue(scene, out var activeController))
            {
                activeController.SetActiveState(true);
            }

            SceneManager.SetActiveScene(scene);

            return true;
        }

        public void AddSceneChangeListener(Action<SceneId> action)
        {
            m_SceneChangeListeners.Add(action);
        }

        public void RemoveSceneChangeListener(Action<SceneId> action)
        {
            m_SceneChangeListeners.Remove(action);
        }
    }
}