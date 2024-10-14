using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{
    public class StateManager : IStateManager
    {
        public StateManager(int defaultState = -1)
        {
            _currentState = defaultState;
        }


        private int _currentState = -1;
        /// <summary>
        /// Current gamestate
        /// </summary>
        /// <value>Current gamestate</value>
        public int currentState
        {
            get
            {
                return _currentState;
            }
        }

        private StateChangeEvent _changeState = new StateChangeEvent();
        /// <summary>
        /// Gets invoked when the gamestate changes
        /// </summary>
        /// <value>Change state event</value>
        public UnityEvent<int, int> changeState
        {
            get
            {
                return _changeState;
            }
        }
        /// <summary>
        /// Changes the current gamestate
        /// </summary>
        /// <param name="newState">New gamestate.</param>
        public void onChangeState(int newState)
        {

            if (newState != _currentState)
            {
                int oldState = _currentState;
                _currentState = newState;
                _changeState.Invoke(oldState, newState);
            }
        }

        public bool hasState(int state)
        {
            return (_currentState & state) != 0;
        }

        public void addState(int state)
        {
            onChangeState(_currentState | state);
        }

        public void removeState(int state)
        {
            onChangeState(_currentState & ~state);
        }

        public void removeAndAddState(int removeState, int addState)
        {
            onChangeState((_currentState & ~removeState) | addState);
        }

        public static bool isAdded(int state, int oldState, int newState)
        {
            return (state & oldState) == 0 && (state & newState) != 0;
        }

        public static bool isRemoved(int state, int oldState, int newState)
        {
            return (state & oldState) != 0 && (state & newState) == 0;
        }
    }
}
