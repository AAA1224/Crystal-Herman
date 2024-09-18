using System.Collections.Generic;

namespace KoboldTools
{
    public interface ILocalisationDataReader
    {
        Dictionary<string, ILocalisedText> fetchLocalisedData();
    }
}
