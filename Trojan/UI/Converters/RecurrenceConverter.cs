using System;
using System.Globalization;
using System.Windows.Data;
using Trojan.Core.Models;

namespace Trojan.UI.Converters
{
    public class RecurrenceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecurrenceType recurrence)
            {
                return recurrence switch
                {
                    RecurrenceType.None => "Enkratni",
                    RecurrenceType.Daily => "Dnevno",
                    RecurrenceType.Weekly => "Tedensko",
                    RecurrenceType.Monthly => "Mesečno",
                    _ => "Neznano"
                };
            }
            return "Neznano";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}