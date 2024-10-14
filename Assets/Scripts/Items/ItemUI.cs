using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoboldTools;
using UnityEngine.UI;

namespace Herman
{
    public class ItemUI : VCBehaviour<IItem>
    {
        public Text titleText;
        public Text descriptionText;

        public override void onModelChanged()
        {
            if (this.titleText != null)
            {
                this.titleText.text = this.model.LocalisedTitle;
            }
            if (this.descriptionText != null)
            {
                this.descriptionText.text = this.model.LocalisedDescription;
            }
        }

        public override void onModelRemoved()
        {

        }


    }
}
