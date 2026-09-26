using Test2.Model;
using Test2.Resources;

namespace Test2.ViewModels
{
    public sealed partial class MainViewModel
    {
        //Принцип SRP
        private IEnumerable<TaskModel> GetFilteredTasks()
        {
            var filtered = _allTasks.AsEnumerable();

            if (Filter.Date.HasValue)
                filtered = filtered.Where(t => t.Date.Date == Filter.Date.Value.Date);

            if (!string.IsNullOrWhiteSpace(Filter.Name))
                filtered = filtered.Where(t => t.Name != null &&
                    t.Name.Contains(Filter.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.LastName))
                filtered = filtered.Where(t => t.LastName != null &&
                    t.LastName.Contains(Filter.LastName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.MiddleName))
                filtered = filtered.Where(t => t.MiddleName != null &&
                    t.MiddleName.Contains(Filter.MiddleName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.City))
                filtered = filtered.Where(t => t.City != null &&
                    t.City.Contains(Filter.City, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(Filter.Country))
                filtered = filtered.Where(t => t.Country != null &&
                    t.Country.Contains(Filter.Country, StringComparison.OrdinalIgnoreCase));

            return filtered;
        }

        //Принцип SRP
        private void UpdateDisplayedTasks(IEnumerable<TaskModel> tasks)
        {
            DisplayedTasks.Clear();
            foreach (var item in tasks)
            {
                DisplayedTasks.Add(item);
            }
        }

        //Принцип SRP
        private void UpdateStatusAfterFilter(int displayedCount, int totalCount)
        {
            if (Filter.HasFilters)
            {
                StatusText = string.Format(Resource1.FilterApplied, displayedCount, totalCount);
            }
            else
            {
                StatusText = string.Format(Resource1.RecordsTotal, displayedCount);
            }
        }

        //Принцип SRP
        private void ApplyFilter()
        {
            StatusText = Resource1.Filtering;
            var filtered = GetFilteredTasks();
            UpdateDisplayedTasks(filtered);
            UpdateStatusAfterFilter(DisplayedTasks.Count, _allTasks.Count);
        }

        private void ResetFilter()
        {
            Filter.Reset();
            UpdateDisplayedTasks(_allTasks);
            UpdateStatusAfterFilter(_allTasks.Count, _allTasks.Count);
        }
    }
}