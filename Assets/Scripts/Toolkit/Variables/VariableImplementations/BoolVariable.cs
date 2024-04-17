using System;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/Bool Variable")]
    public class BoolVariable : Variable<bool>
    {
        public override string Serialize()
        {
            return value ? "1" : "0";
        }

        public override void Deserialize(string s)
        {
            value = (s == "1");
        }

        public void Negate()
        {
            Value = !Value;
        }
    }

    [Serializable]
    public class BoolReference : Reference<bool, BoolVariable>
    {
        
    }
}