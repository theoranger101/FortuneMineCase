using UnityEngine;

namespace Toolkit.Singletons
{
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static bool s_didCacheInstance = false;

        protected static T s_Instance;

        public static T Instance
        {
            get
            {
                if (s_didCacheInstance) return s_Instance;

                s_Instance = GetInstance();
                s_didCacheInstance = true;

                return s_Instance;
            }

            protected set => s_Instance = value;
        }

        public static T GetInstance()
        {
            var instance = Object.FindObjectOfType<T>();
            return instance == null ? CreateInstance<T>() : instance;
        }

        private static T CreateInstance<T>() where T : Behaviour
        {
            var instanceObj = new GameObject(typeof(T) + "_Instance");

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(instanceObj);
            }

            return instanceObj.AddComponent<T>();
        }

        protected bool SetupInstance(bool persistOnLoad = true)
        {
            if (s_Instance != null && s_Instance != this) // another instance is already present.
            {
                Debug.Log($"An instance of type {this.GetType()} already exists. Destroying duplicate.");

                Destroy(this.gameObject);
                return false;
            }

            s_Instance = (T)this;

            if (persistOnLoad)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
#endif
            }

            return true;
        }

        public static bool HasInstance() => s_Instance;
    }
}