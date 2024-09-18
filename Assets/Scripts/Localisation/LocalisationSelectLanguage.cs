using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KoboldTools
{
    public class LocalisationSelectLanguage : MonoBehaviour
    {
        public TMP_Dropdown languageSelectionUi;
        private ILocalisation localisation;

        public void Awake()
        {
            if (this.languageSelectionUi == null)
            {
                this.languageSelectionUi = GetComponent<TMP_Dropdown>();
            }
        }

        public IEnumerator Start ()
        {
            // Wait for the localisation singleton to appear.
            while (Localisation.instance == null)
            {
                yield return null;
            }
            this.localisation = Localisation.instance;
            this.localisation.eLanguageChanged.AddListener(this.onLanguageChanged);
            this.onLanguageChanged();

            // Begin to listen for changes to the dropdown.
            this.languageSelectionUi.onValueChanged.AddListener(this.onValueChanged);
        }

        private void onLanguageChanged()
        {
            // Initialise the options for the language selection dropdown.
            var langs = this.localisation.languages.Select(e => e.Value.langNameEnglish).ToList();
            this.languageSelectionUi.ClearOptions();
            this.languageSelectionUi.AddOptions(langs);

            var activeLangName = this.localisation.activeLanguage.langNameEnglish;
            var idx = langs.FindIndex(e => e == activeLangName);
            this.languageSelectionUi.value = idx;
        }

        private void onValueChanged(int idx)
        {
            var langName = this.languageSelectionUi.options[idx].text;
            var lang = this.localisation.languages.Values.ToList().Find(e => e.langNameEnglish == langName);
            if (lang != null)
            {
                this.localisation.activeLanguage = lang;
                
            }
            else
            {
                Debug.Log("The selected language was not found!");
            }
        }
    }
}
