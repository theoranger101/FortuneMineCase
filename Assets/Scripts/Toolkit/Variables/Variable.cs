using System;
using System.Collections.Generic;
using Events;
using Toolkit.Variables.EventImplementations;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Variables
{
    public abstract class Variable : ScriptableObject
    {
        private static VariablesModule Module => VariablesModule.Instance;

        [HideInInspector]
        public int Hash;

        public bool LogStackTrace = false;

        public bool BindToPlayerPrefs = false;

        [HideInInspector]
        public string PrefsKey;

        public bool IsDirty { get; protected set; } = false;

        public static int NameToHash(string name)
        {
            unchecked // arithmetic operations are not checked for overflow. 
            {
                int hash = 23;
                int len = name.Length;

                for (var i = 0; i < len; i++)
                {
                    hash = hash * 31 + name[i];
                }

                return hash;
            }
        }

        #region Abstract/Virtual Methods

        public abstract object GetValueAsObject();

        public abstract T GetValue<T>(T fallbackVal = default);

        public abstract bool GetValue<T>(out T val);

        public abstract void SetValueAsObject(object obj);

        public abstract void InvokeModified();

        public abstract void ResetToEditorValue();

        public virtual string Serialize()
        {
            return GetValueAsObject().ToString();
        }

        public virtual void Deserialize(string s)
        {
            var val = Convert.ChangeType(s, GetType());
            SetValueAsObject(s);
        }

        public virtual bool CanBeBoundToPlayerPrefs() => true;

        public virtual void OnInspectorChanged()
        {
        }

        public abstract void RaiseModifiedEvent();

        #endregion

        #region Public Static Methods

        public static void Initialize()
        {
            var module = Module;

            var variables = module.Variables = new Dictionary<int, Variable>(32);
            var prefsVariables = module.PrefsVariables = new List<Variable>(8);

            var loadedVariables = Resources.LoadAll<Variable>("Variables");

            for (var i = 0; i < loadedVariables.Length; i++)
            {
                InitializeVariable(ref loadedVariables[i], ref variables, ref prefsVariables);
            }
        }

        public static void SavePlayerPrefs()
        {
            var prefsVariables = Module.PrefsVariables;

            for (var i = 0; i < prefsVariables.Count; i++)
            {
                var v = prefsVariables[i];

                if (!v.IsDirty)
                    continue;

                v.IsDirty = false;

                var key = v.PrefsKey;
                var value = v.Serialize();

                using var serializedEvt = VariableEvent.Get(key, value).SendGlobal((int)VariableEventType.OnSerialized);
                PlayerPrefs.SetString(key, value);
            }

            PlayerPrefs.Save();
        }

        public static T Get<T>(int hash, bool allowNull = false) where T : Variable
        {
            CheckVariablesInitialization();

            var variables = Module.Variables;
            if (variables.TryGetValue(hash, out var variable))
            {
                return (T)variable;
            }

            if (!allowNull)
            {
                Debug.LogError("Variable is not loaded.");
            }

            Debug.LogWarning("Variable is not loaded, returning null.");
            return null;
        }

        public static T Get<T>(string name, bool allowNull = false) where T : Variable =>
            Get<T>(NameToHash(name), allowNull);

        public static bool GetValue<T>(int hash, out T val)
        {
            var variables = Module.Variables;
            if (variables.TryGetValue(hash, out var variable))
            {
                if (variable.GetValue(out val))
                {
                    return true;
                }
            }

            val = default;
            return false;
        }

        public static bool GetValue<T>(string name, out T val) => GetValue(NameToHash(name), out val);

        public static T GetValue<T>(int hash, T fallbackVal = default)
        {
            CheckVariablesInitialization();

            var variables = Module.Variables;
            if (variables.TryGetValue(hash, out var variable))
            {
                return variable.GetValue(fallbackVal);
            }

            return fallbackVal;
        }

        public static T GetValue<T>(string name, T fallbackVal = default) => GetValue(NameToHash(name), fallbackVal);

        public static T Create<T>() where T : Variable
        {
            return ScriptableObject.CreateInstance<T>();
        }

        #endregion

        #region Private Static Methods(mid-operation)

        protected static void InitializeVariable(ref Variable variable, ref Dictionary<int, Variable> variables,
            ref List<Variable> prefsVariables)
        {
            AddToDictionary(ref variable, ref variables);

            variable.hideFlags = HideFlags.DontUnloadUnusedAsset;
            DontDestroyOnLoad(variable);

            if (variable.BindToPlayerPrefs)
            {
                SetupPlayerPrefs(ref variable, ref prefsVariables);
            }
            else
            {
                variable.ResetToEditorValue();
            }

            variable.RaiseModifiedEvent();
        }

        protected static void AddToDictionary(ref Variable variable, ref Dictionary<int, Variable> variables)
        {
            var hash = variable.Hash;

            if (variables.ContainsKey(hash))
            {
                if (variables[hash] == null)
                {
                    variables[hash] = variable;
                }
                else
                {
                    Debug.Log($"Variable name {variable.name} is taken.");
                }
            }
            else
            {
                variables.Add(hash, variable);
            }
        }

        protected static void SetupPlayerPrefs(ref Variable variable, ref List<Variable> prefsVariables)
        {
            try
            {
                prefsVariables.Add(variable);

                var key = variable.PrefsKey;

                if (PlayerPrefs.HasKey(key))
                {
                    var strVal = PlayerPrefs.GetString(key);
                    variable.Deserialize(strVal);
                }
                else
                {
                    variable.ResetToEditorValue();
                }

                CheckLogStackTrace(variable);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected static void CheckLogStackTrace(Variable variable)
        {
#if UNITY_EDITOR
            if (variable.LogStackTrace)
                Debug.Log(variable.name + ": " + variable.GetValueAsObject(), variable);
#endif
        }

        protected static void CheckVariablesInitialization()
        {
            if (!VariablesModule.HasInstance())
            {
                Initialize();
            }
        }

        #endregion
    }

    public abstract class Variable<T> : Variable
    {
        [SerializeField]
        private T editorValue;

        [SerializeField]
        protected T value;

        public T Value
        {
            get => value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    return;
                }

                IsDirty = true;
                this.value = value;

#if UNITY_EDITOR
                CheckLogStackTrace(this);

                if (!Application.isPlaying)
                {
                    this.editorValue = value;
                    EditorUtility.SetDirty(this);
                }
#endif

                RaiseModifiedEvent();
            }
        }

        private void OnEnable()
        {
            if (!BindToPlayerPrefs)
            {
                value = editorValue;
            }

            Hash = NameToHash(name);
            PrefsKey = $"Variable_{Hash}";
        }

        public override TReq GetValue<TReq>(TReq fallbackVal = default)
        {
            var val = Value;

            if (val is TReq valTReq)
            {
                return valTReq;
            }

            Debug.LogWarning(
                $"Variable {name} is of the type {val.GetType()} while the requested type is {typeof(T)}. Returning fallback value.");
            return fallbackVal;
        }

        public override bool GetValue<TReq>(out TReq valOut)
        {
            var val = Value;

            if (val is TReq valTReq)
            {
                valOut = valTReq;
                return true;
            }

            valOut = default;
            return false;
        }

        public override void SetValueAsObject(object obj)
        {
            value = (T)obj;
        }

        public void SetValueWithoutNotify(T val)
        {
            this.value = val;
        }

        public override object GetValueAsObject()
        {
            return value;
        }

        public override void RaiseModifiedEvent()
        {
            using var changedEvt = VariableEvent<T>.Get(value);
            this.SendEvent(changedEvt, (int)VariableEventType.OnModified);
        }

        public override void InvokeModified()
        {
            editorValue = value;
            RaiseModifiedEvent();
        }

        public override void ResetToEditorValue()
        {
            Value = editorValue;
        }

        // automatically convert an instance of one data type to another without explicitly casting
        public static implicit operator T(Variable<T> v)
        {
            return v.Value;
        }
    }
}