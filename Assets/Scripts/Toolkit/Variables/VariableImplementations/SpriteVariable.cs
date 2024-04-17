using System;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/Sprite Variable")]
    public class SpriteVariable : Variable<Sprite>
    {
        public override bool CanBeBoundToPlayerPrefs()
        {
            return false;
        }
    }

    [Serializable]
    public class SpriteReference : Reference<Sprite, SpriteVariable>
    {
        
    }
}