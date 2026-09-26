namespace Test2.ViewModels
{
    public sealed class FilterViewModel : BaseViewModel
    {
        private DateTime? _date;
        private string _name = "";
        private string _lastName = "";
        private string _middleName = "";
        private string _city = "";
        private string _country = "";

        public DateTime? Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string MiddleName
        {
            get => _middleName;
            set => SetProperty(ref _middleName, value);
        }

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }

        public bool HasFilters =>
            Date.HasValue ||
            !string.IsNullOrWhiteSpace(Name) ||
            !string.IsNullOrWhiteSpace(LastName) ||
            !string.IsNullOrWhiteSpace(MiddleName) ||
            !string.IsNullOrWhiteSpace(City) ||
            !string.IsNullOrWhiteSpace(Country);

        public void Reset()
        {
            Date = null;
            Name = "";
            LastName = "";
            MiddleName = "";
            City = "";
            Country = "";
        }
    }
}