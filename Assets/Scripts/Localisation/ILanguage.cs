using System;
using System.Globalization;
using UnityEngine;

namespace KoboldTools
{
    public interface ILanguage : IEquatable<ILanguage>
    {
        string ISO6393 { get; }
        string langNameEnglish { get; }
        string langNameOriginal { get; }
        CultureInfo culture { get; }
        Texture2D icon { get; }
        Font font { get; }
    }
}
