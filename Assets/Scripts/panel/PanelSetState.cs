using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{

    public class PanelSetState : ViewController<IPanel>
    {
        [EnumFlag]
        public Gamestates addStateWhenOpen;

        public override void onModelChanged()
        {
            model.open.AddListener(opened);
            model.closeComplete.AddListener(closed);
        }

        public override void onModelRemoved()
        {
            model.open.RemoveListener(opened);
            model.closeComplete.RemoveListener(closed);
        }

        private void opened()
        {
            Gamestate.instance.addState((int)addStateWhenOpen);
        }

        private void closed()
        {
            Gamestate.instance.removeState((int)addStateWhenOpen);
        }
    }
}
