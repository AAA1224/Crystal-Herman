using System;
using System.Collections.Generic;

namespace KoboldTools
{
    public class LocalisedText : ILocalisedText
    {
        private Dictionary<string, string> _textlines = new Dictionary<string, string>();
        public Dictionary<string, string> textlines
        {
            get
            {
                return _textlines;
            }
        }

        public LocalisedText(string languageID, string text)
        {
            if (!_textlines.ContainsKey(languageID))
            {
                _textlines.Add(languageID, text);
            }
        }

        public void addLocalisedText(string languageID, string text)
        {
            if (!_textlines.ContainsKey(languageID))
            {
                _textlines.Add(languageID, text);
            }
        }

        public string getLocalisedText(string languageID)
        {
            if (hasLocalisedContent(languageID))
            {
                return _textlines[languageID];
            }
            else
            {
                return "";
            }
        }

        public bool hasLocalisedContent(string languageID)
        {
            if (!String.IsNullOrEmpty(languageID) && _textlines.ContainsKey(languageID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
