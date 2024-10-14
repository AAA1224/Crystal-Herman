using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
namespace KoboldTools
{
    public class ButtonChangeGamestate : ViewController<Button>
    {
        [EnumFlag]
        public Gamestates state;
        public bool addThis = false;
        public bool removeThis = false;

        public override void onModelChanged()
        {
            model.onClick.AddListener(() =>
            {
                if (addThis)
                {
                    Gamestate.instance.addState((int)state);
                }
                else if (removeThis)
                {
                    Gamestate.instance.removeState((int)state);
                }
                else
                {
                    Gamestate.instance.onChangeState((int)state);
                }
            });
        }
    }
}
