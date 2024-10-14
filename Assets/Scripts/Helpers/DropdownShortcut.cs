using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KoboldTools
{
    public class DropdownShortcut : ViewController<Dropdown>
    {
        public KeyCode[] keys;
        private void Update()
        {
            for (int i=0; i<keys.Length; i++)
            {
                if (model.options.Count > i && Input.GetKeyUp(keys[i]))
                {
                    model.value = i;
                }
            }
        }
    }
}
