using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{
    public class Gamestate : MonoBehaviour, IStateManager
    {
        [EnumFlag]
        public Gamestates defaultState;
        private IStateManager _stateManager = new StateManager(-1);

        public void Awake()
        {
            _stateManager = new StateManager((int)defaultState);
        }

        #region Singleton

        //Here is a private reference only this class can access
        [SerializeField]
        private static Gamestate _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Gamestate instance
        {
            get
            {
                //If _instance hasn't been set yet, we grab it from the scene!
                //This will only happen the first time this reference is used.
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Gamestate>();
                }
                return _instance;
            }
        }

        public int currentState
        {
            get
            {
                return _stateManager.currentState;
            }
        }

        public UnityEvent<int, int> changeState
        {
            get
            {
                return _stateManager.changeState;
            }
        }

        #endregion

        public void onChangeState(int newState)
        {
            Debug.Log("[GAME] change state to '" + (Gamestates)newState + "'");
            _stateManager.onChangeState(newState);
            Debug.Log("[GAME] changed state to '"+ (Gamestates)_stateManager.currentState + "'");
        }

        public bool hasState(int state)
        {
            return _stateManager.hasState(state);
        }

        public void addState(int state)
        {
            Debug.Log("[GAME] add state '" + (Gamestates)state + "'");
            _stateManager.addState(state);
            Debug.Log("[GAME] changed state to '" + (Gamestates)_stateManager.currentState + "'");
        }

        public void removeState(int state)
        {
            Debug.Log("[GAME] remove state '" + (Gamestates)state + "'");
            _stateManager.removeState(state);
            Debug.Log("[GAME] changed state to '" + (Gamestates)_stateManager.currentState + "'");
        }

        public void removeAndAddState(int removeState, int addState)
        {
            Debug.Log("[GAME] remove state '" + (Gamestates)removeState + "' and add state'"+(Gamestates)addState+"'");
            _stateManager.removeAndAddState(removeState, addState);
            Debug.Log("[GAME] changed state to '" + (Gamestates)_stateManager.currentState + "'");
        }

    }
}
