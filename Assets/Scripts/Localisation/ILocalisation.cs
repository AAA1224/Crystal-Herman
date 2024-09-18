using UnityEngine.Events;
using System.Collections.Generic;

namespace KoboldTools
{
    public interface ILocalisation
    {
        string defaultLanguageID { get; }
        ILanguage activeLanguage { get; set; }
        Dictionary<string, ILanguage> languages { get; }
        Dictionary<string, ILocalisedText> localisedTexts { get; }
        UnityEvent eLanguageChanged { get; }

        bool hasTextId(string inKey);
        string getLocalisedText(string inKey);
        string getLocalisedText(string inKey, string languageID);
        string getLocalisedFormat(string inKey, params object[] args);
        void addLanguage(ILanguage inLanguage, bool replaceExisting = false);
        void addLocalisedText(string textID, string lang, string text);
    }
}
