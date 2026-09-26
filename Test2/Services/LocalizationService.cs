using System.Globalization;

namespace Test2.Services
{
    public sealed class LocalizationService : ILocalizationService
    {
        private string _currentCulture = "ru-RU";

        public string CurrentCulture => _currentCulture;

        public event Action CultureChanged;

        public void SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            _currentCulture = cultureName;

            //Установка культуры для текущего потока
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CultureChanged?.Invoke();
        }
    }
}
