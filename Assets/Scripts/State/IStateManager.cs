using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace KoboldTools
{
    public interface IStateManager
    {
        int currentState { get; }
        UnityEvent<int, int> changeState { get; }
        void onChangeState(int newState);

        bool hasState(int state);
        void addState(int state);
        void removeState(int state);
        void removeAndAddState(int removeState, int addState);
    }
}
