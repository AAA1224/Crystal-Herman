using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace KoboldTools {
    public class ButtonEnableWithGamestate : ViewController<Button>
    {
        [EnumFlag]
        public Gamestates state;
        [EnumFlag]
        public Gamestates forceHide;

        public override void onModelChanged()
        {
            Gamestate.instance.changeState.AddListener(changedState);
            changedState(-1, Gamestate.instance.currentState);
        }

        public override void onModelRemoved()
        {
            Gamestate.instance.changeState.RemoveListener(changedState);
        }

        private void changedState(int oldState, int newState)
        {

            if (Gamestate.instance.hasState((int)state) && !Gamestate.instance.hasState((int)forceHide))
            {
                if (!model.interactable)
                    model.interactable = true;
            }
            else
            {
                if (model.interactable)
                {
                    model.OnPointerExit(null);
                    model.interactable = false;
                }
            }

        }
    }

}
