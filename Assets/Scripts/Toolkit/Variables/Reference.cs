using System;
using System.Reflection;
using UnityEngine;

namespace Toolkit.Variables
{
    [Serializable]
    public abstract class Reference
    {
        public enum ReferenceType
        {
            Constant = 0,
            GlobalVariable = 1,
            MemberVariable = 2
        }

        [SerializeField]
        protected ReferenceType m_Type = ReferenceType.Constant;
        
        public ReferenceType Type
        {
            get => m_Type;
            set => m_Type = value;
        }

        public abstract object GetValue();
    }

    [SerializeField]
    public abstract class Reference<T, T1> : Reference where T1 : Variable<T>
    {
        protected static Type GetWrappedType()
        {
            return typeof(T1);
        }

        [SerializeField]
        protected T constantValue; // for constant type

        [SerializeField]
        protected T1 variable; // for global and member types

        [SerializeField]
        protected GameObject gameObject;

        [SerializeField]
        protected MonoBehaviour behaviour;

        [SerializeField]
        protected string fieldName;

        public T Value
        {
            get
            {
                return GetValueByType();
            }

            set
            {
                if (m_Type == ReferenceType.Constant)
                {
                    constantValue = value;
                }
                else
                {
                    variable.Value = value;
                }
            }
        }

        private T GetValueByType()
        {
            switch (m_Type)
            {
                case ReferenceType.GlobalVariable:
                    return GetGlobalValue();
                case ReferenceType.MemberVariable:
                    return GetMemberValue();
                case ReferenceType.Constant:
                    return GetConstantValue();
            }

            return variable.Value;
        }

        private T GetConstantValue()
        {
            return constantValue;
        }

        private T GetGlobalValue()
        {
            if (variable != null)
            {
                return variable.Value;
            }
            else
            {
                Debug.LogWarning($"Value reference of type {typeof(T)} is set to use a variable which is null. Returning constant.");
                return constantValue;
            }
        }

        private T GetMemberValue()
        {
            if (variable == null)
            {
                CollectMemberVariable();
            }

            return variable.Value;
        }

        private void CollectMemberVariable()
        {
            // retrieve a field named fieldName from the object behaviour, regardless of its access modifier (public or private), as long as it's an instance field.
            var field = behaviour.GetType().GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            this.variable = (T1)field.GetValue(behaviour);

            if (this.variable == null)
            {
                throw new MemberAccessException(
                    "You have to make sure that the target 'Member Variable' is initialized before access time. Consider initializing it in the Awake method and accessing it in and after Start method.");
            }
        }

        public override object GetValue()
        {
            return Value;
        }

        public T1 GetVariable()
        {
            if (variable == null && m_Type == ReferenceType.MemberVariable)
            {
                CollectMemberVariable();
            }

            return variable;
        }

        public void SetVariable(T1 variable)
        {
            this.variable = variable;
        }
    }
}