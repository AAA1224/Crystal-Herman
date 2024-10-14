using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KoboldTools
{
    public class ButtonShortcut : ViewController<Button>
    {
        public KeyCode key = KeyCode.None;
        private void Update()
        {
            if (Input.GetKeyUp(key))
            {
                model.onClick.Invoke();
            }
        }
    }
}
