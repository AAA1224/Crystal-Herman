using UnityEngine;
using System.Collections;
namespace KoboldTools {
    public class PanelDisplayWithGamestate : ViewController<IPanel>
    {
        [EnumFlag]
        public Gamestates state;
        [EnumFlag]
        public Gamestates forceHide;

        public override void onModelChanged()
        {
            Gamestate.instance.changeState.AddListener(changedState);
            changedState(-1, (int)Gamestate.instance.currentState);
        }

        public override void onModelRemoved()
        {
            Gamestate.instance.changeState.RemoveListener(changedState);
        }

        private void changedState(int oldState, int newState)
        {

            if (Gamestate.instance.hasState((int)state) && !Gamestate.instance.hasState((int)forceHide))
            {
                if (!model.isOpen)
                    model.onOpen();
            }
            else
            {
                if (model.isOpen)
                    model.onClose();
            }
            
        }
    }

}
