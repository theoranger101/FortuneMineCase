using System;
using System.Collections.Generic;
using Events;
using Toolkit.Variables;
using Toolkit.Variables.EventImplementations;
using UnityEngine;

namespace Toolkit.DataBinding
{
    [DefaultExecutionOrder(-5)]
    public class DataObserver<TRef, TVar, T> : MonoBehaviour where TRef : Reference<T, TVar>, new()
                                                                     where TVar : Variable<T>
    {
        public bool StackTrace = false;

        [SerializeField]
        protected bool OnStart = true;

        [SerializeField]
        protected TRef m_Data;

        public Action<T> OnModified;

        [SerializeField]
        private float delay = -1f;

        public float Delay
        {
            get => delay;
            set => delay = value;
        }

        private bool m_IsInitialized = false;

        private Queue<T> m_DataQueue;

        private Queue<float> m_TimeQueue;

        private void Reset()
        {
            m_Data = new TRef
            {
                Type = Reference.ReferenceType.GlobalVariable
            };
        }

        protected virtual void OnValidate()
        {
            if (m_Data.Type != Reference.ReferenceType.Constant) return;
            
            Debug.LogWarning("Observed data cannot be 'Constant', switching to 'Global'.");
            m_Data.Type = Reference.ReferenceType.GlobalVariable;
        }

        protected virtual void Start()
        {
            m_IsInitialized = true;

            if (m_Data.Type != Reference.ReferenceType.Constant)
            {
                var variable = m_Data.GetVariable();
                variable.AddListener<VariableEvent<T>>(OnDataModified, channel:(int)VariableEventType.OnModified);
            }
            
            if (OnStart)
            {
                OnDataModifiedNoDelay(m_Data.Value);
            }

            if (delay > 0)
            {
                m_DataQueue = new Queue<T>(32);
                m_TimeQueue = new Queue<float>(32);
            }
        }

        protected virtual void OnEnable()
        {
            if (!m_IsInitialized)
            {
                return;
            }

            if (m_Data.Type != Reference.ReferenceType.Constant)
            {
                var variable = m_Data.GetVariable();
                
                if(variable == null)
                    return;
                
                variable.AddListener<VariableEvent<T>>(OnDataModified, channel:(int)VariableEventType.OnModified);
            }
            
            OnDataModifiedNoDelay(m_Data.Value);
        }

        protected virtual void OnDisable()
        {
            if (m_Data.Type != Reference.ReferenceType.Constant)
            {
                var variable = m_Data.GetVariable();
                
                if(variable == null)
                    return;
                
                variable.RemoveListener<VariableEvent<T>>(OnDataModified, channel:(int)VariableEventType.OnModified);
            }
        }

        private void Update()
        {
            if (m_TimeQueue == null)
            {
                return;
            }

            while (m_TimeQueue.Count > 0 && UnityEngine.Time.time >= m_TimeQueue.Peek())
            {
                m_TimeQueue.Dequeue();
                var delayedData = m_DataQueue.Dequeue();
                OnModified.Invoke(delayedData);
            }
        }

        protected virtual void OnDataModifiedNoDelay(T val)
        {
            var oldDelay = delay;
            delay = -1;
            OnDataModified(val);
            delay = oldDelay;
        }

        private void OnDataModified(T val)
        {
            if (delay <= 0)
            {
                OnModified.Invoke(val);
                return;
            }

            if (m_DataQueue == null)
            {
                m_DataQueue = new Queue<T>(32);
                m_TimeQueue = new Queue<float>(32);
            }
            
            m_DataQueue.Enqueue(val);
            m_TimeQueue.Enqueue(UnityEngine.Time.time + this.delay);
        }
        
        private void OnDataModified(VariableEvent<T> evt)
        {
            OnDataModified(evt.Value);
        }
    }
}