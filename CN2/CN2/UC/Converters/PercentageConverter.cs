using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CN2.UC.Converters
{
    public class PercentageConverter : MarkupExtension, IValueConverter
    {
        private static PercentageConverter _instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return System.Convert.ToDouble(value)*System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new PercentageConverter());
        }
    }
}
