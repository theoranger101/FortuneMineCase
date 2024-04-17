using System.Collections.Generic;

namespace Toolkit.Variables
{
    public class VariablesModule : Module<VariablesModule>
    {
        public Dictionary<int, Variable> Variables;
        public List<Variable> PrefsVariables;

        public override void OnEnable()
        {
        }

        public override void OnDisable()
        {
        }

        public override void Update()
        {
        }

        public override void LateUpdate()
        {
        }
    }
}