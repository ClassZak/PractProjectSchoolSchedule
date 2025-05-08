using SchoolSchedule.Model;
using SchoolSchedule.ViewModel.Edit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchoolSchedule.View.Edit.EditPage
{
	/// <summary>
	/// Логика взаимодействия для EditPageBellSchedule.xaml
	/// </summary>
	public partial class EditPageBellSchedule : Page, IEditPage<BellSchedule>
	{
		public EditPageBellSchedule()
		{
			InitializeComponent();
		}
		public EditPageBellSchedule(bool objectIsNew, BellSchedule model, List<BellSchedule> modelsForUniqueCheck, List<BellScheduleType> bellScheduleTypes) : this()
		{
			DataContext =  new EditBellScheduleViewModel(model, modelsForUniqueCheck, bellScheduleTypes, objectIsNew);
		}
		private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить byte
			var textBox = (TextBox)sender;
			string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
			e.Handled = !byte.TryParse(newText, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
		}
		static readonly Regex timeRegex = new Regex(@"^[0-9:]+$");
		private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !timeRegex.IsMatch(e.Text);
		}

		#region TimeBox
		private void FormatTimeTextBox(TextBox textBox)
		{
			var input = textBox.Text.Trim();

			// Очистка от недопустимых символов
			var cleaned = new string(input.Where(c => char.IsDigit(c) || c == ':').ToArray());

			// Разделение на компоненты времени
			var parts = cleaned.Split(new[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);

			int hours = 0, minutes = 0, seconds = 0;

			try
			{
				if (parts.Length >= 1)
					seconds = ParseAndClamp(parts[parts.Length - 1], 0, 59);
				if (parts.Length >= 2)
					minutes = ParseAndClamp(parts[parts.Length - 2], 0, 59);
				if (parts.Length >= 3)
					hours = ParseAndClamp(parts[parts.Length - 3], 0, 23);

				// Форматирование результата
				string formattedTime;
				if (parts.Length == 1)
				{
					formattedTime = string.Format("{0:D2}", seconds);
				}
				else if (parts.Length == 2)
				{
					formattedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);
				}
				else
				{
					formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
				}

				textBox.Text = formattedTime;
			}
			catch
			{
				textBox.Text = "00:00:00";
			}
		}
		private int ParseAndClamp(string part, int min, int max)
		{
			if (!int.TryParse(part, out int value))
				return min;

			if (value < min) return min;
			if (value > max) return max;
			return value;
		}
		private void TimeBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				var textBox = (TextBox)sender;
				FormatTimeTextBox(textBox);
				// Принудительно обновляем привязку
				textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

				e.Handled = true;
			}
		}
		#endregion
		#region Фокус полей ввода
		private void TimeBox_LostFocus(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			if (textBox != null)
				FormatTimeTextBox(textBox);
		}
		private void LessonNumberTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			int.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int data);
			(DataContext as ViewModel.Edit.EditBellScheduleViewModel).LessonNumber = data;
		}
		#endregion

		public KeyValuePair<bool, string> CheckInputRules()
		{
			return (DataContext as EditBellScheduleViewModel).CheckInputRules();
		}
	}
}
