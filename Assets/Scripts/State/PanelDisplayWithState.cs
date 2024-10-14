using UnityEngine;
using System.Collections;
namespace KoboldTools
{
    public class PanelDisplayWithState : VCBehaviour<IPanel>
    {

        private IStateManager stateManager;
        public GameObject stateManagerObject;

        [EnumFlag(true)]
        public int show;
        [EnumFlag(true)]
        public int forceHide;

        public override void onModelChanged()
        {
            stateManager = stateManagerObject.GetComponent<IStateManager>();
            if (stateManager != null)
            {
                stateManager.changeState.AddListener(changedState);
                changedState(-1, (int)stateManager.currentState);
            }
            else
            {
                Debug.LogError("No Statemanager found for " + name);
            }
        }

        public override void onModelRemoved()
        {
            stateManager.changeState.RemoveListener(changedState);
        }

        private void changedState(int oldState, int newState)
        {
            if (!model.isOpen && stateManager.hasState(show) && !stateManager.hasState(forceHide))
            {
                model.onOpen();
            }
            else if (model.isOpen && (!stateManager.hasState(show) || stateManager.hasState(forceHide)))
            {
                model.onClose();
            }
        }
    }

}
