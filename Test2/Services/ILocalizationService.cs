using System.Globalization;

namespace Test2.Services
{
    public interface ILocalizationService
    {
        void SetCulture(string cultureName);
        string CurrentCulture { get; }
        event Action CultureChanged;
    }
}