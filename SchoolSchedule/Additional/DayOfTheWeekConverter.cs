using SchoolSchedule.Model.Additional;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SchoolSchedule.Additional
{
	public class DayOfTheWeekConverter : IValueConverter
	{
		static readonly Dictionary<DayOfTheWeek, string> DAYS = new Dictionary<DayOfTheWeek, string>
		{
			{ DayOfTheWeek.Monday,"Понедельник" },
			{ DayOfTheWeek.Tuesday,"Вторник" },
			{ DayOfTheWeek.Wednesday,"Среда" },
			{ DayOfTheWeek.Thursday,"Четверг" },
			{ DayOfTheWeek.Friday,"Пятница" },
			{ DayOfTheWeek.Saturday,"Суббота" },
			{ DayOfTheWeek.Sunday,"Воскресенье" },
		};
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is DayOfTheWeek day)
				return DAYS[day];

			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string dayString)
				return DAYS.FirstOrDefault(x => x.Value == dayString).Key;

			return null;
		}
	}
}
