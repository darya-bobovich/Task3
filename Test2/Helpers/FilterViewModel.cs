using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Test2.ViewModels
{
    public class FilterViewModel : INotifyPropertyChanged
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
            set { _date = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); }
        }

        public string MiddleName
        {
            get => _middleName;
            set { _middleName = value; OnPropertyChanged(); }
        }

        public string City
        {
            get => _city;
            set { _city = value; OnPropertyChanged(); }
        }

        public string Country
        {
            get => _country;
            set { _country = value; OnPropertyChanged(); }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}