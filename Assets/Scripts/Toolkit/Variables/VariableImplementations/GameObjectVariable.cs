using System;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/Game Object Variable")]
    public class GameObjectVariable : Variable<GameObject>
    {
        public override bool CanBeBoundToPlayerPrefs()
        {
            return false;
        }
    }

    [Serializable]
    public class GameObjectReference : Reference<GameObject, GameObjectVariable>
    {
        
    }
}