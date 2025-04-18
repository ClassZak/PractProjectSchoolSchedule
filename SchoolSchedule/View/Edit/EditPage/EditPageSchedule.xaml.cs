using SchoolSchedule.Model;
using SchoolSchedule.ViewModel.Edit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Логика взаимодействия для EditPageSubject.xaml
	/// </summary>
	public partial class EditPageSchedule : Page, IEditPage<Model.Schedule>
	{
		public bool ObjectIsNew{  get; set; }
		
		public List<Model.Schedule> SchedulesForCheck { get; set; } = new List<Model.Schedule>();
		public Model.Schedule ValueRef{ get ; set ; }
		protected EditPageSchedule()
		{
			InitializeComponent();
			DataContext = ValueRef;
		}


		private EditScheduleViewModel _viewModel;
		public EditPageSchedule(List<Model.Schedule> schedule, List<Model.Lesson> lessons,List<Model.Teacher> teachers, bool objectIsNew, Model.Schedule lesson) : this()
		{
			ObjectIsNew= objectIsNew;

			if (ObjectIsNew)
				ValueRef = new Model.Schedule();
			else
				ValueRef = lesson;

			_viewModel = new EditScheduleViewModel(ValueRef,lessons,teachers);
			DataContext = _viewModel;
		}


		private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить byte
			var textBox = (TextBox)sender;
			string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
			e.Handled = !byte.TryParse(newText, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
		}
		private void RussianTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить только русские буквы, пробелы и дефис
			var regex = new Regex(@"^[\p{IsCyrillic}\s\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}
		static Regex timeRegex = new Regex(@"^[0-9:]+$");
		private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !timeRegex.IsMatch(e.Text);
		}


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


		private void TimeBox_LostFocus(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			if(textBox!=null)
				FormatTimeTextBox(textBox);
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

		public KeyValuePair<bool,string> CheckInputRules()
		{
			var obj=(this.DataContext as EditScheduleViewModel).CurrentSchedule;
			if (obj.IdLesson == 0)
				return new KeyValuePair<bool, string>(false, "Выберете урок для расписания");
			if (obj.IdTeacher== 0)
				return new KeyValuePair<bool, string>(false, "Выберете преподавателя для расписания");
			if(obj.StartTime == null ||  obj.EndTime == null || obj.Date==null)
				return new KeyValuePair<bool, string>(false, "Настройте время начала и конеца урока, а также дату проведения занятий");
			if(obj.StartTime>obj.EndTime)
				return new KeyValuePair<bool, string>(false, "Настройте правильно время начала и конеца урока. Занятие не может начаться позже его завершения");
			if(SchedulesForCheck.Any(x=>x.IdLesson==obj.IdLesson && x.IdTeacher==obj.IdTeacher && x.Date==obj.Date && x.StartTime==obj.StartTime && x.EndTime==obj.EndTime))
				return new KeyValuePair<bool,string>(true, "Введите другие данные для составления расписаания. Подобная запись уже есть в базе данных");
				
				
			return new KeyValuePair<bool,string>(true,null);
		}
	}
}
