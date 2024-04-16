using Events;

namespace SceneManagement
{
    public enum SceneEventType
    {
        LoadRequest = 0,
        LoadSuccess = 1,
        LoadFail = 2,
        Unload = 4,
    }

    public class SceneEvent : Event<SceneEvent>
    {
        public SceneId SceneId;
        public bool Additive;

        public static SceneEvent Get(SceneId sceneId, bool additive = false)
        {
            var evt = GetPooledInternal();
            evt.SceneId = sceneId;
            evt.Additive = additive;

            return evt;
        }
    }
}