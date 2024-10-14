using KoboldTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Herman
{
    public class DisplayMonth: MonoBehaviour
    {
        public Text monthText;
        public Text progressText;
        public Slider progressSlider;
        public string progressTextId = "progressText";
        private MainGameManager model;

        public IEnumerator Start()
        {
            while (MainGameManager.Instance == null)
            {
                yield return null;
            }
            model = MainGameManager.Instance;
            onModelChanged();
        }

        public void onModelChanged()
        {
            this.model.onLevelStateChanged.AddListener(this.levelStateChanged);
            this.levelStateChanged();
        }

        public void onModelRemoved()
        {
            this.model.onLevelStateChanged.RemoveListener(this.levelStateChanged);
        }

        private void levelStateChanged()
        {
            if (this.monthText != null)
            {
                this.monthText.text = Mathf.Round(this.model.months).ToString();
            }

            if (this.progressText != null)
            {
                this.progressText.text = Localisation.instance.getLocalisedFormat(this.progressTextId, Mathf.Round(this.model.months), this.model.maximumMonths);
            }

            if (this.progressSlider != null)
            {
                this.progressSlider.value = (this.model.months / this.model.maximumMonths);
            }
        }
    }

}