
namespace KoboldTools
{
    public interface ILocaliseElement
    {
        string textID { get; set; }

        void updateText();
    }
}
