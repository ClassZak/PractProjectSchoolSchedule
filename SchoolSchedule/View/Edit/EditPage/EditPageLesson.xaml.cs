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
	public partial class EditPageLesson : Page, IEditPage<Lesson>
	{
		public bool ObjectIsNew{  get; set; }
		
		public List<Lesson> LessonsForCheck { get; set; } = new List<Lesson>();
		public Lesson ValueRef{ get ; set ; }
		protected EditPageLesson()
		{
			InitializeComponent();
			DataContext = ValueRef;
		}


		private EditLessonViewModel _viewModel;
		public EditPageLesson(List<Lesson> lessons, List<Model.Group> groups,List<Model.Subject> subjects, bool objectIsNew, Lesson lesson) : this()
		{
			ObjectIsNew= objectIsNew;

			if (ObjectIsNew)
				ValueRef = new Lesson();
			else
				ValueRef = lesson;

			_viewModel = new EditLessonViewModel(ValueRef, groups,subjects);
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

		private static readonly Regex forbiddenCharsRegex = new Regex
		(
			@"[^\w.%+-@-]", // Инвертированный набор разрешённых символов
			RegexOptions.IgnoreCase
		);
		public static bool HasInvalidEmailChars(string input)
		{
			return forbiddenCharsRegex.IsMatch(input);
		}

		public KeyValuePair<bool,string> CheckInputRules()
		{
			var lesson = (DataContext as EditLessonViewModel).CurrentLesson;
			if (lesson.IdGroup == 0)
				return new KeyValuePair<bool, string>(false, "Выбрете класс для урока");
			if (lesson.IdSubject== 0)
				return new KeyValuePair<bool, string>(false, "Выбрете предмет для урока");
			if (lesson.Number<1 && lesson.Number>8)
				return new KeyValuePair<bool, string>(false, "Выбрете номер урока урока от 1 до 8");
			if(LessonsForCheck.Any(x=>x.Number==lesson.Number && x.IdGroup==lesson.IdGroup && x.IdSubject==lesson.IdSubject))
				return new KeyValuePair<bool, string>(false, $"Введите другие данные для урока. Подобная запись уже есть в базе данных");
				
			
			return new KeyValuePair<bool,string>(true,null);
		}
	}
}
