using System;
using UnityEngine;
using UnityEngine.UI;

namespace KoboldTools
{
    public class LocaliseElementUIText : MonoBehaviour, ILocaliseElement
    {
        #region LocalisedElementUIText

        public Text guiTextComponent = null;

        public void Start()
        {
            if (guiTextComponent == null)
            {
                guiTextComponent = gameObject.GetComponent<Text>();
            }

            if (Localisation.instance != null)
            {
                Localisation.instance.eLanguageChanged.AddListener(onLanguageChanged);
            }

            updateText();
        }

        private void onLanguageChanged()
        {
            updateText();
        }

        #endregion

        #region ILocaliseElement

        [SerializeField]
        private string _textID;
        public string textID
        {
            get
            {
                return _textID;
            }
            set
            {
                _textID = value;
                updateText();
            }
        }

        public void updateText()
        {
            if (!String.IsNullOrEmpty(textID) && guiTextComponent != null)
            {
                if (Localisation.instance != null)
                {
                    guiTextComponent.text = Localisation.instance.getLocalisedText(textID);
                }
            }
        }

        #endregion

    }
}
