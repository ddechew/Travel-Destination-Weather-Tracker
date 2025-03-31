using SQLite;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DestinationsApp.Models
{
    public class Destination : INotifyPropertyChanged
    {
        private string country;
        private string city;
        private DateTime startDate;
        private DateTime endDate;
        private int duration;
        private string purpose;
        private double rating;
        private string status;

        public Destination() { }

        public Destination(string country, string city, DateTime startDate, int duration, string purpose, double rating, string status)
        {
            this.Country = country;
            this.City = city;
            this.StartDate = startDate;
            this.Duration = duration;
            this.EndDate = startDate.AddDays(duration);
            this.Purpose = purpose;
            this.Rating = rating;
            this.Status = status;

            SetActiveCommand = new Command(SetActive);
            SetCompletedCommand = new Command(SetCompleted);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void onPropertyChange([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ICommand SetActiveCommand { get; }
        public ICommand SetCompletedCommand { get; }

        public string FormattedStartDate =>
            $"Start Date: {StartDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}";

        public string FormattedEndDate =>
            $"End Date: {EndDate.ToString("dd MMM yyyy", CultureInfo.InvariantCulture)}";

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Country
        {
            get => country;
            set
            {
                if (country != value)
                {
                    country = value;
                    onPropertyChange();
                }
            }
        }

        public string City
        {
            get => city;
            set
            {
                if (city != value)
                {
                    city = value;
                    onPropertyChange();
                }
            }
        }

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    onPropertyChange();
                    EndDate = startDate.AddDays(Duration);
                }
            }
        }

        [Ignore]
        public DateTime EndDate
        {
            get => endDate;
            private set
            {
                if (endDate != value)
                {
                    endDate = value;
                    onPropertyChange();
                }
            }
        }

        public int Duration
        {
            get => duration;
            set
            {
                if (duration != value)
                {
                    duration = value;
                    onPropertyChange();
                    EndDate = StartDate.AddDays(duration);
                }
            }
        }

        public string Purpose
        {
            get => purpose;
            set
            {
                if (purpose != value)
                {
                    purpose = value;
                    onPropertyChange();
                }
            }
        }

        public double Rating
        {
            get => rating;
            set
            {
                if (rating != value)
                {
                    rating = value;
                    onPropertyChange();
                }
            }
        }

        public string Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    onPropertyChange();
                    onPropertyChange(nameof(ActiveButtonColor));
                    onPropertyChange(nameof(CompletedButtonColor));
                }
            }
        }

        public Color ActiveButtonColor => (Status == "Planned" || Status == "Ongoing") ? Colors.Green : Colors.Gray;

        public Color CompletedButtonColor => (Status == "Completed") ? Colors.Green : Colors.Gray;

        public void SetActive()
        {
            if (Status == "Completed") return;
            Status = "Ongoing";
            _ = SaveStatusToDatabaseAsync();

        }

        public void SetCompleted()
        {
            Status = "Completed";
            _ = SaveStatusToDatabaseAsync();
        }

        private async Task SaveStatusToDatabaseAsync()
        {
            if (App.Database != null)
            {
                await App.Database.UpdateDestinationAsync(this);
            }
        }
    }
}
