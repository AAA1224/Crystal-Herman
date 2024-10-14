using System;
using System.Collections.Generic;

namespace KoboldTools
{
    /// <summary>
    /// A flow phase handles conditions for enter and exit the phase as 
    /// well as implementing logic to get the correct next phase.
    /// </summary>
    public class FlowPhase : IFlowPhase
    {
        private string _phaseName;
        private int _flowState = 0;
        private List<IFlowPhase> _nextPhases = new List<IFlowPhase>();
        /// <summary>
        /// Holds the conditions which must be met in order to enter this phase.
        /// </summary>
        private List<IFlowCondition> enterConditions = new List<IFlowCondition>();
        /// <summary>
        /// Holds the conditions which must be met in order to exit this phase.
        /// </summary>
        private List<IFlowCondition> exitConditions = new List<IFlowCondition>();

        /// <summary>
        /// Initializes a new instance of the <see cref="KoboldTools.FlowPhase"/> class.
        /// </summary>
        /// <param name="flowState">Flow state.</param>
        /// <param name="phaseName">Phase name.</param>
        public FlowPhase(int flowState, string phaseName)
        {
            _flowState = flowState;
            _phaseName = phaseName;
        }
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return String.Format("FlowPhase(0x{0:X}, {1})", this._flowState, this._phaseName);
        }
        public int flowState
        {
            get
            {
                return _flowState;
            }
        }
        public List<IFlowPhase> nextPhases
        {
            get
            {
                return _nextPhases;
            }
        }
        public IFlowPhase add(IFlowPhase next)
        {
            _nextPhases.Add(next);
            return next;
        }
        public void add(IFlowPhase[] next)
        {
            foreach(IFlowPhase n in next)
            {
                add(n);
            }
        }
        public void addEnterCondition(IFlowCondition flowCondition)
        {
            enterConditions.Add(flowCondition);
        }
        public void removeEnterCondition(IFlowCondition flowCondition)
        {
            enterConditions.Remove(flowCondition);
        }
        public void addExitCondition(IFlowCondition flowCondition)
        {
            exitConditions.Add(flowCondition);
        }
        public void removeExitCondition(IFlowCondition flowCondition)
        {
            exitConditions.Remove(flowCondition);
        }
        public bool canEnter()
        {
            foreach(IFlowCondition c in enterConditions)
            {
                if (!c.conditionMet)
                {
                    return false;
                }
            }
            return true;
        }
        public bool canExit()
        {
            foreach (IFlowCondition c in exitConditions)
            {
                if (!c.conditionMet)
                {
                    return false;
                }
            }
            return true;
        }
        public IFlowPhase getNextPhase()
        {
            foreach(IFlowPhase phase in _nextPhases)
            {
                if (phase.canEnter())
                {
                    return phase;
                }
            }
            return null;
        }
        public void traverseFlow(IFlowPhase root, Action<IFlowPhase> action)
        {
            traverseFlow(root, action, new List<IFlowPhase>());
        }
        public void traverseFlow(IFlowPhase root, Action<IFlowPhase> action, List<IFlowPhase> traversed)
        {
            //exit if we have a recursive loop
            if (traversed.Contains(root))
                return;

            traversed.Add(root);

            action(root);

            foreach(IFlowPhase phase in root.nextPhases)
            {
                traverseFlow(phase, action, traversed);
            }
        }
    }
}
