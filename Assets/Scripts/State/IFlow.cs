using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoboldTools
{
    /// <summary>
    /// Game flow handles the flow of specific gamestates within a game. This should usually be implemented as a singleton.
    /// </summary>
    public interface IFlow : IStateManager
    {
        /// <summary>
        /// Adds an exit condituon to the specified state.
        /// </summary>
        void addExitCondition(int exitState, IFlowCondition condition);
        /// <summary>
        /// Adds an exit condition
        /// </summary>
        void addExitCondition(IFlowCondition condition);
        void addEnterCondition(int enterState, IFlowCondition condition);
        void removeExitCondition(int exitState, IFlowCondition condition);
        void removeExitCondition(IFlowCondition condition);
        void removeEnterCondition(int enterState, IFlowCondition condition);
        void forceState(int newState);
        bool running { get; set; }
    }
}
