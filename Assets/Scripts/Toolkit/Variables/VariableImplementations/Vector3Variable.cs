using System;
using System.Globalization;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/Vector3 Variable")]
    public class Vector3Variable : Variable<Vector3>
    {
        public override string Serialize()
        {
            // vector3 value packed with '&'s between axes.
            return
                $"{Value.x.ToString(CultureInfo.InvariantCulture)}" +
                $"&{Value.y.ToString(CultureInfo.InvariantCulture)}" +
                $"&{Value.z.ToString(CultureInfo.InvariantCulture)}";
        }

        public override void Deserialize(string s)
        {
            var split = s.Split('&');
            value = new Vector3(
                float.Parse(split[0], CultureInfo.InvariantCulture),
                float.Parse(split[1], CultureInfo.InvariantCulture),
                float.Parse(split[2], CultureInfo.InvariantCulture)
                );
        }
    }

    [Serializable]
    public class Vector3Reference : Reference<Vector3, Vector3Variable>
    {
        
    }
}