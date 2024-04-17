using System;
using System.Globalization;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/Color Variable")]
    public class ColorVariable : Variable<Color>
    {
        public override string Serialize()
        {
            // color value packed with '&'s between color channels.
            return
                $"{Value.r.ToString(CultureInfo.InvariantCulture)}" +
                $"&{Value.g.ToString(CultureInfo.InvariantCulture)}" +
                $"&{Value.b.ToString(CultureInfo.InvariantCulture)}" +
                $"&{Value.a.ToString(CultureInfo.InvariantCulture)}";
        }

        public override void Deserialize(string s)
        {
            var split = s.Split('&');

            value = new Color(
                float.Parse(split[0], CultureInfo.InvariantCulture),
                float.Parse(split[1], CultureInfo.InvariantCulture),
                float.Parse(split[2], CultureInfo.InvariantCulture),
                float.Parse(split[3], CultureInfo.InvariantCulture)
            );
        }

        public void Multiply(Color color)
        {
            Value *= color;
        }

        public void Add(Color color)
        {
            Value += color;
        }
    }

    [Serializable]
    public class ColorReference : Reference<Color, ColorVariable>
    {
        
    }
}