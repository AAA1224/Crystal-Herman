using System.Globalization;
using UnityEngine;

namespace KoboldTools
{
    public class GameLanguage : ILanguage
    {
        #region GameLanguage
        public GameLanguage(string inISO6393)
        {
            _ISO6393 = inISO6393;
        }

        public GameLanguage(string inISO6393, string inNameEnglish)
        {
            _ISO6393 = inISO6393;
            _langNameEnglish = inNameEnglish;
        }

        public GameLanguage(string inISO6393, string inNameEnglish, string inNameOriginal)
        {
            _ISO6393 = inISO6393;
            _langNameEnglish = inNameEnglish;
            _langNameOriginal = inNameOriginal;
        }

        public GameLanguage(string inISO6393, string inNameEnglish, string inNameOriginal, CultureInfo inCulture)
        {
            _ISO6393 = inISO6393;
            _langNameEnglish = inNameEnglish;
            _langNameOriginal = inNameOriginal;
            _culture = inCulture;
        }
        #endregion

        #region ILanguage
        private string _ISO6393 = "";
        public string ISO6393
        {
            get
            {
                return _ISO6393;
            }
        }

        private string _langNameEnglish = "";
        public string langNameEnglish
        {
            get
            {
                return _langNameEnglish;
            }
        }

        private string _langNameOriginal = "";
        public string langNameOriginal
        {
            get
            {
                return _langNameOriginal;
            }
        }

        private CultureInfo _culture;
        public CultureInfo culture
        {
            get
            {
                return _culture;
            }
        }


        private Texture2D _icon;
        public Texture2D icon
        {
            get
            {
                return _icon;
            }
        }


        private Font _font;
        public Font font
        {
            get
            {
                return _font;
            }
        }

        public bool Equals(ILanguage other)
        {
            return this.ISO6393 == other.ISO6393;
        }
        #endregion
    }
}
