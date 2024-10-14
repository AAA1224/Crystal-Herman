using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KoboldTools
{

    public class QuitGameButton : VCBehaviour<Button>
    {
        public override void onModelChanged()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                gameObject.SetActive(false);
            }
            else
            {
                model.onClick.AddListener(btnClicked);
            }
        }

        public override void onModelRemoved()
        {
            model.onClick.RemoveListener(btnClicked);
        }

        private void btnClicked()
        {
            Application.Quit();
        }
    }
}
