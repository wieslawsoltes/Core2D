#nullable disable
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels;

namespace Core2D.Converters
{
    public class ViewModelToTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ViewModelBase viewModel)
            {
                if (string.IsNullOrEmpty(viewModel.Name))
                {
                    return viewModel.GetType().Name.Replace("ViewModel", "");
                }
                return viewModel.Name;
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
