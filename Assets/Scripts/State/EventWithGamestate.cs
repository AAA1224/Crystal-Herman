using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{
    public class EventWithGamestate : MonoBehaviour
    {
        [EnumFlag]
        public Gamestates state;
        public UnityEvent enterState;
        public UnityEvent exitState;

        public void Start()
        {
            Gamestate.instance.changeState.AddListener(changedState);
            //Debug.Log("add event listener, current state is " + (Gamestates)Gamestate.instance.currentState);
            changedState(-1, Gamestate.instance.currentState);
        }

        private void changedState(int oldState, int newState)
        {
            bool exit = false;
            bool enter = false;

            if ((oldState & (int)state) != 0)
            {
                exit = true;
            }

            if (Gamestate.instance.hasState((int)state))
            {
                enter = true;
            }

            if (exit && !enter)
                exitState.Invoke();

            if (enter && (!exit || oldState == -1))
                enterState.Invoke();

        }
    }
}
