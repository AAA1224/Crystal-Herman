using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KoboldTools.Logging;

namespace KoboldTools
{
    /// <summary>
    /// abstract class to access gameflow. Needs to be inherited from a specific gameflow class for each game.
    /// </summary>
    public abstract class FlowBehaviour : MonoBehaviour, IFlow
    {
        //state manager implementation
        private IStateManager statemanager = new StateManager();
        public int currentState
        {
            get
            {
                return statemanager.currentState;
            }
        }

        public UnityEvent<int, int> changeState
        {
            get
            {
                return statemanager.changeState;
            }
        }
        [SerializeField]
        private bool _running = true;
        public bool running {
            get
            {
                return _running;
            }
            set
            {
                _running = value;
            }
        }

        public void addState(int state)
        {
            throw new NotImplementedException("Changing States on a Flow Statemanager is not allowed! Add conditions to the Flow Statemanager and let the manager handle changes by itself.");
        }

        public bool hasState(int state)
        {
            return statemanager.hasState(state);
        }

        public void onChangeState(int newState)
        {
            throw new NotImplementedException("Changing States on a Flow Statemanager is not allowed! Add conditions to the Flow Statemanager and let the manager handle changes by itself.");
        }

        public void removeAndAddState(int removeState, int addState)
        {
            throw new NotImplementedException("Changing States on a Flow Statemanager is not allowed! Add conditions to the Flow Statemanager and let the manager handle changes by itself.");
        }

        public void removeState(int state)
        {
            throw new NotImplementedException("Changing States on a Flow Statemanager is not allowed! Add conditions to the Flow Statemanager and let the manager handle changes by itself.");
        }

        //IGameFlow implementation
        private IFlowPhase flow;
        private IFlowPhase rootFlow;

        //startup game flow in awake
        public void Awake()
        {
            flow = rootFlow = createFlow();
            statemanager.onChangeState(flow.flowState);
        }

        public void Update()
        {
            if (flow == null || !running)
                return;

            //check current flow state
            if (flow.canExit())
            {
                //exit conditions are met, transition to next phase
                flow = flow.getNextPhase();
                if (flow != null)
                {
                    RootLogger.Info(this, "Exit flow state to: {0}", this.flow);
                    statemanager.onChangeState(flow.flowState);
                }
                else
                {
                    RootLogger.Info(this, "End of flow reached at: {0}", this.currentState);
                }
            }
            else
            {
                //wait for exit conditions
            }
        }

        //abstract factory method to create the game flow
        protected abstract IFlowPhase createFlow();

        /// <summary>
        /// add a condition that has to be met to enter the flow phase
        /// </summary>
        /// <param name="state of the flow phase"></param>
        /// <param name="condition"></param>
        public void addEnterCondition(int enterState, IFlowCondition condition)
        {
            flow.traverseFlow(rootFlow, delegate(IFlowPhase phase)
            {
                if(phase.flowState == enterState)
                {
                    phase.addEnterCondition(condition);
                }
            });
        }
        /// <summary>
        /// remove a condition that has to be met to enter the flow phase
        /// </summary>
        /// <param name="state of the flow phase"></param>
        /// <param name="condition"></param>
        public void removeEnterCondition(int enterState, IFlowCondition condition)
        {
            flow.traverseFlow(rootFlow, delegate (IFlowPhase phase)
            {
                if (phase.flowState == enterState)
                {
                    phase.removeEnterCondition(condition);
                }
            });
        }
        /// <summary>
        /// add a condition that has to be met to exit the flow phase
        /// </summary>
        /// <param name="state of the flow phase"></param>
        /// <param name="condition"></param>
        public void addExitCondition(int exitState, IFlowCondition condition)
        {
            flow.traverseFlow(rootFlow, delegate (IFlowPhase phase)
            {
                if (phase.flowState == exitState)
                {
                    phase.addExitCondition(condition);
                }
            });
        }
        /// <summary>
        /// remove a condition that has to be met to exit the flow phase
        /// </summary>
        /// <param name="state of the flow phase"></param>
        /// <param name="condition"></param>
        public void removeExitCondition(int exitState, IFlowCondition condition)
        {
            flow.traverseFlow(rootFlow, delegate (IFlowPhase phase)
            {
                if (phase.flowState == exitState)
                {
                    phase.removeExitCondition(condition);
                }
            });
        }
        /// <summary>
        /// add exit condition to all states
        /// </summary>
        /// <param name="condition"></param>
        public void addExitCondition(IFlowCondition condition)
        {
            flow.traverseFlow(rootFlow, delegate (IFlowPhase phase)
            {
                phase.addExitCondition(condition);
            });
        }
        /// <summary>
        /// remove exit condition from all states
        /// </summary>
        /// <param name="condition"></param>
        public void removeExitCondition(IFlowCondition condition)
        {
            flow.traverseFlow(rootFlow, delegate (IFlowPhase phase)
            {
                phase.removeExitCondition(condition);
            });
        }

        /// <summary>
        /// forces a state change without checking conditions
        /// </summary>
        /// <param name="newState">the forced state</param>
        public void forceState(int newState)
        {
            //traverse flow to search for the matching flowphase
            flow.traverseFlow(rootFlow, delegate (IFlowPhase phase)
            {
                if (phase.flowState == newState)
                {
                    //set new flowphase
                    flow = phase;
                    return;
                }
            });
            //change state to new flowphase
            statemanager.onChangeState(flow.flowState);
        }
    }
}
