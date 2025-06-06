using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SchoolSchedule.Additional
{
	public class TimeSpanToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is TimeSpan time)
				return time.ToString(@"hh\:mm\:ss");

			return "00:00:00";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var input = value as string;
			if (input != null)
			{
				TimeSpan time;
				if (TimeSpan.TryParseExact(
					input,
					new[] { @"hh\:mm\:ss", @"mm\:ss", @"ss" },
					CultureInfo.InvariantCulture,
					out time))
				{
					return time;
				}
			}
			return TimeSpan.Zero;
		}
	}
}
