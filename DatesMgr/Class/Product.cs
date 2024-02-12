using System;
using System.ComponentModel;

namespace DM.Class
{
    public class Product : INotifyPropertyChanged
    {
        private bool _isSelected; // Flag indicating if the product is selected

        // Property for tracking whether the product is selected
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    // Invoking PropertyChanged event when the property changes
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        private int _remainingDays; // Number of remaining days until expiration
        public int RemainingDays
        {
            get => _remainingDays;
            set
            {
                if (_remainingDays != value)
                {
                    _remainingDays = value;
                    OnPropertyChanged(nameof(RemainingDays));
                }
            }
        }

        // Properties for product details
        public string? ProductName { get; set; }
        public DateTime ExpireDate { get; set; }
        public string? Unit { get; set; }
        public int? Qty { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? Notes { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Method for raising PropertyChanged event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
