using System.Collections.Generic;
using System;

namespace KoboldTools
{
    /// <summary>
    /// A flow phase handles conditions for enter and exit the phase as 
    /// well as implementing logic to get the correct next phase.
    /// </summary>
    public interface IFlowPhase
    {
        /// <summary>
        /// Holds the integer identifier for the associated state.
        /// </summary>
        int flowState { get; }
        /// <summary>
        /// Contains the list of next phases.
        /// </summary>
        List<IFlowPhase> nextPhases { get; }
        /// <summary>
        /// Determines whether the phase may be entered.
        /// </summary>
        /// <returns><c>true</c>, if the phase may be entered, <c>false</c> otherwise.</returns>
        bool canEnter();
        /// <summary>
        /// Determines whether the phase may be exited.
        /// </summary>
        /// <returns><c>true</c>, if the phase may be exited, <c>false</c> otherwise.</returns>
        bool canExit();
        /// <summary>
        /// Adds the specified phase to the list of phases that follow the current one.
        /// </summary>
        /// <param name="next">Next phase.</param>
        IFlowPhase add(IFlowPhase next);
        /// <summary>
        /// Adds a set of phases to the list of subsequent phases.
        /// </summary>
        /// <param name="next">Next phases.</param>
        void add(IFlowPhase[] next);
        IFlowPhase getNextPhase();
        /// <summary>
        /// Adds an entry condition to the current phase.
        /// </summary>
        /// <param name="flowCondition">Flow condition.</param>
        void addEnterCondition(IFlowCondition flowCondition);
        /// <summary>
        /// Adds an exit condition to the current phase.
        /// </summary>
        /// <param name="flowCondition">Flow condition.</param>
        void addExitCondition(IFlowCondition flowCondition);
        /// <summary>
        /// Removes an entry condition.
        /// </summary>
        /// <param name="flowCondition">Flow condition.</param>
        void removeEnterCondition(IFlowCondition flowCondition);
        /// <summary>
        /// Removes an exit condition.
        /// </summary>
        /// <param name="flowCondition">Flow condition.</param>
        void removeExitCondition(IFlowCondition flowCondition);
        /// <summary>
        /// Given an action, recursively traverses the flow graph and applies
        /// the action to every phase.
        /// </summary>
        /// <param name="root">Root.</param>
        /// <param name="action">Action.</param>
        void traverseFlow(IFlowPhase root, Action<IFlowPhase> action);
    }
}
