

namespace KoboldTools
{
    public interface ILogEntry
    {
        string identifier
        { get; }
        System.DateTime timestamp
        { get; }

        System.Object getContent();
    }
}
