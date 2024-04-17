using Events;

namespace Toolkit.Variables.EventImplementations
{
    public enum VariableEventType
    {
        OnSerialized = 0,
        OnModified = 1,
    }
    
    public class VariableEvent : Event<VariableEvent>
    {
        public string Key;
        public string Value;
        
        public static VariableEvent Get()
        {
            var evt = GetPooledInternal();
            return evt;
        }
        
        public static VariableEvent Get(string key, string value)
        {
            var evt = GetPooledInternal();
            evt.Key = key;
            evt.Value = value;
            return evt;
        }
    }

    public class VariableEvent<T> : Event<VariableEvent<T>>
    {
        public Variable<T> Variable;
        public T Value;
        
        public static VariableEvent<T> Get(Variable<T> variable, T value)
        {
            var evt = GetPooledInternal();
            evt.Variable = variable;
            evt.Value = value;
            
            return evt;
        }
        
        public static VariableEvent<T> Get(T value)
        {
            var evt = GetPooledInternal();
            evt.Value = value;
            
            return evt;
        }
    }
}