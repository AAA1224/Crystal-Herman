using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace KoboldTools
{
    public class Localisation : MonoBehaviour, ILocalisation
    {
        public const string UndefinedKey = "[undefined]";
        public const string NoLanguageLoaded = "[no language loaded]";
        public bool Enabled = true;

        #region Singleton

        //Here is a private reference only this class can access
        [SerializeField]
        private static Localisation _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Localisation instance
        {
            get
            {
                //If _instance hasn't been set yet, we grab it from the scene!
                //This will only happen the first time this reference is used.
                if (_instance == null) _instance = GameObject.FindObjectOfType<Localisation>();
                return _instance;
            }
        }

        #endregion

        #region Localisation

        private void Awake()
        {
            _activeLanguage = new GameLanguage(_defaultLanguageID);
        }

        #endregion

        #region ILocalisation

        [SerializeField]
        private string _defaultLanguageID = "eng";
        public string defaultLanguageID
        {
            get
            {
                return _defaultLanguageID;
            }
        }

        private ILanguage _activeLanguage;
        public ILanguage activeLanguage
        {
            get
            {
                return _activeLanguage;
            }
            set
            {
                if (!_activeLanguage.Equals(value))
                {
                    _activeLanguage = value;
                    eLanguageChanged.Invoke();
                }
            }
        }

        private Dictionary<string, ILanguage> _languages = new Dictionary<string, ILanguage>();
        public Dictionary<string, ILanguage> languages
        {
            get
            {
                return _languages;
            }
        }

        private Dictionary<string, ILocalisedText> _localisedTexts = new Dictionary<string, ILocalisedText>();
        public Dictionary<string, ILocalisedText> localisedTexts
        {
            get
            {
                return _localisedTexts;
            }
        }

        private UnityEvent _eLanguageChanged = new UnityEvent();
        public UnityEvent eLanguageChanged
        {
            get
            {
                return _eLanguageChanged;
            }
        }

        public void addLanguage(ILanguage inLanguage, bool replaceExisting = false)
        {
            if (!_languages.ContainsKey(inLanguage.ISO6393))
            {
                _languages.Add(inLanguage.ISO6393, inLanguage);
            }
            else
            {
                if (replaceExisting)
                {
                    _languages[inLanguage.ISO6393] = inLanguage;
                    Debug.LogWarning("The language '{0}' already exists. Overwriting the existing language." + inLanguage.ISO6393);
                }
                else
                {
                    Debug.LogWarning("The language '{0}' already exists. Will therefore not overwrite." + inLanguage.ISO6393);
                }
            }
        }

        public void addLocalisedText(string textID, string lang, string text)
        {
            if (_localisedTexts.ContainsKey(textID))
            {
                if (!_localisedTexts[textID].hasLocalisedContent(lang))
                {
                    _localisedTexts[textID].addLocalisedText(lang, text);
                }
            }
            else
            {
                _localisedTexts.Add(textID, new LocalisedText(lang, text));
            }
        }

        public bool hasTextId(string inKey)
        {
            return this.hasTextId(inKey, _activeLanguage.ISO6393);
        }

        public bool hasTextId(string inKey, string languageId)
        {
            if (!String.IsNullOrEmpty(inKey))
            {
                return _localisedTexts.ContainsKey(inKey) && _localisedTexts[inKey].hasLocalisedContent(languageId);
            }
            else
            {
                // RootLogger.Warning(this, "Language key is null or empty: {0}", inKey);
                return false;
            }
        }

        public string getLocalisedText(string inKey)
        {
            return getLocalisedText(inKey, _activeLanguage.ISO6393);
        }

        public string getLocalisedText(string inKey, string languageId)
        {
            if (this.Enabled)
            {
                if (this._languages.Count > 0)
                {
                    if (this.hasTextId(inKey, languageId))
                    {
                        string localisedText = this._localisedTexts[inKey].getLocalisedText(languageId);
                        if (String.IsNullOrEmpty(localisedText))
                        {
                            Debug.Log("The entry for key '{0}' in '{1}' is empty. Consider adding text." + inKey + languageId);
                        }
                        return localisedText;
                    }
                    else
                    {
                        Debug.LogWarning("No entry for '{0}' in '{1}'. Cannot display text." + inKey + languageId);
                        return Localisation.UndefinedKey;
                    }
                }
                else
                {
                    return Localisation.NoLanguageLoaded;
                }
            }
            else
            {
                return inKey;
            }
        }

        public string getLocalisedFormat(string inKey, params object[] args)
        {
            var fmtString = this.getLocalisedText(inKey, this._activeLanguage.ISO6393);
            try
            {
                return String.Format(fmtString, args);
            }
            catch (FormatException e)
            {
                Debug.LogError("Offending format string '{0}' or arguments '{1}'" + fmtString + args.ToList().ToString());
                return Localisation.UndefinedKey;
            }
        }

        #endregion
    }
}
