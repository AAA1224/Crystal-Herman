using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoboldTools
{
    public interface ILocalisedText
    {
        Dictionary<string, string> textlines { get; }

        bool hasLocalisedContent(string languageID);
        string getLocalisedText(string languageID);
        void addLocalisedText(string languageID, string text);
    }
}
