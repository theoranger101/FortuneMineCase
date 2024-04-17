using System;
using System.Globalization;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/Float Variable")]
    public class FloatVariable : Variable<float>
    {
        public override string Serialize()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override void Deserialize(string s)
        {
            value = float.Parse(s, CultureInfo.InvariantCulture);
        }
        
        #region Operation Overloads

        public static float operator +(FloatVariable v1, FloatVariable v2)
        {
            return v1.Value + v2.Value;
        }
        
        public static float operator +(FloatVariable v1, int v2)
        {
            return v1.Value + v2;
        }
        
        public static float operator +(float v1, FloatVariable v2)
        {
            return v1 + v2.Value;
        }
        
        public static float operator -(FloatVariable v1, FloatVariable v2)
        {
            return v1.Value - v2.Value;
        }
        
        public static float operator -(FloatVariable v1, float v2)
        {
            return v1.Value - v2;
        }
        
        public static float operator -(int v1, FloatVariable v2)
        {
            return v1 - v2.Value;
        }
        
        public static float operator *(FloatVariable v1, FloatVariable v2)
        {
            return v1.Value * v2.Value;
        }
        
        public static float operator *(FloatVariable v1, int v2)
        {
            return v1.Value * v2;
        }
        
        public static float operator *(int v1, FloatVariable v2)
        {
            return v1 * v2.Value;
        }
        
        public static float operator /(FloatVariable v1, FloatVariable v2)
        {
            return v1.Value / v2.Value;
        }
        
        public static float operator /(FloatVariable v1, int v2)
        {
            return v1.Value / v2;
        }
        
        public static float operator /(int v1, FloatVariable v2)
        {
            return v1 / v2.Value;
        }

        #endregion
    }
    
    [Serializable]
    public class FloatReference : Reference<float, FloatVariable>
    {
        
    }
}