using System;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/String Variable")]
    public class StringVariable : Variable<string>
    {
        public override string Serialize()
        {
            return Value;
        }

        public override void Deserialize(string s)
        {
            value = s;
        }
    }

    [Serializable]
    public class StringReference : Reference<string, StringVariable>
    {
        
    }
}