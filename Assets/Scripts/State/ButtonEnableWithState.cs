using System;
using UnityEngine;
using UnityEngine.UI;
using KoboldTools.Logging;

namespace KoboldTools
{
    public class ButtonEnableWithState : VCBehaviour<Button>
    {
        public GameObject stateManagerObject;
        private IStateManager stateManager;

        [EnumFlag(true)]
        public int show;
        [EnumFlag(true)]
        public int forceHide;

        public override void onModelChanged()
        {
            this.stateManager = this.stateManagerObject.GetComponent<IStateManager>();
            if (this.stateManager != null)
            {
                this.stateManager.changeState.AddListener(this.onStateChanged);
                this.onStateChanged(-1, (int)this.stateManager.currentState);
            }
            else
            {
                RootLogger.Exception(this, "No StateManager component found for {0}.", this.name);
            }
        }

        public override void onModelRemoved()
        {
            if (this.stateManager != null)
            {
                this.stateManager.changeState.RemoveListener(this.onStateChanged);
            }
        }

        private void onStateChanged(int oldState, int newState)
        {
            if (this.stateManager.hasState(this.show) && !this.stateManager.hasState(this.forceHide))
            {
                if (!this.model.interactable)
                {
                    this.model.interactable = true;
                }
            }
            else
            {
                if (this.model.interactable)
                {
                    this.model.OnPointerExit(null);
                    this.model.interactable = false;
                }
            }
        }
    }
}
