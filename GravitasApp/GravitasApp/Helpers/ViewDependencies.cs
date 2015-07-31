using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace GravitasApp.Helpers
{

    public class ListEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            IEnumerable list = value as IEnumerable;
            foreach (var item in list)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool val = (bool)value;
            if (val == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ListContentToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {

            string[] fallbacks = (parameter as string).Split(';');

            if (value == null)
                return fallbacks[0];
            else
            {
                IEnumerable<string> items = value as IEnumerable<string>;
                int itemCount = items.Count();
                if (itemCount == 0)
                    return fallbacks[0];
                else if (itemCount == 1)
                    return items.First();
                else
                    return fallbacks[1];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return CategoryMetadata.GetMetadata(value as string).LabelBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
