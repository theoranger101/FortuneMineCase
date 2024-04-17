using System;
using System.Globalization;
using UnityEngine;

namespace Toolkit.Variables.VariableImplementations
{
    [CreateAssetMenu(menuName = "Variables/Int Variable")]
    public class IntVariable : Variable<int>
    {
        public override string Serialize()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override void Deserialize(string s)
        {
            value = int.Parse(s, CultureInfo.InvariantCulture);
        }

        #region Operation Overloads

        public static int operator +(IntVariable v1, IntVariable v2)
        {
            return v1.Value + v2.Value;
        }
        
        public static int operator +(IntVariable v1, int v2)
        {
            return v1.Value + v2;
        }
        
        public static int operator +(int v1, IntVariable v2)
        {
            return v1 + v2.Value;
        }
        
        public static int operator -(IntVariable v1, IntVariable v2)
        {
            return v1.Value - v2.Value;
        }
        
        public static int operator -(IntVariable v1, int v2)
        {
            return v1.Value - v2;
        }
        
        public static int operator -(int v1, IntVariable v2)
        {
            return v1 - v2.Value;
        }
        
        public static int operator *(IntVariable v1, IntVariable v2)
        {
            return v1.Value * v2.Value;
        }
        
        public static int operator *(IntVariable v1, int v2)
        {
            return v1.Value * v2;
        }
        
        public static int operator *(int v1, IntVariable v2)
        {
            return v1 * v2.Value;
        }
        
        public static int operator /(IntVariable v1, IntVariable v2)
        {
            return v1.Value / v2.Value;
        }
        
        public static int operator /(IntVariable v1, int v2)
        {
            return v1.Value / v2;
        }
        
        public static int operator /(int v1, IntVariable v2)
        {
            return v1 / v2.Value;
        }

        #endregion
    }

    [Serializable]
    public class IntReference : Reference<int, IntVariable>
    {
        
    }
}