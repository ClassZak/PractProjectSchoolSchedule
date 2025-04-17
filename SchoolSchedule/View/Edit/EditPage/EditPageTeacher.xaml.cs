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
	/// Логика взаимодействия для EditPageTeacher.xaml
	/// </summary>
	public partial class EditPageTeacher : Page
	{
		public bool ObjectIsNew { get; set; }
		public List<Model.Teacher> TeachersForCheck { get; set; } = new List<Model.Teacher>();
		public Model.Teacher ValueRef { get; set; }
		private EditTeacherViewModel _editTeacherViewModel { get; set; }

		protected EditPageTeacher()
		{
			InitializeComponent();
			DataContext = ValueRef;
		}
		public EditPageTeacher(List<Model.Teacher> teachers,List<Model.Group> groups, List<Model.Subject> subjects, List<Model.TeacherPhone> teacherPhones, bool objectIsNew,Model.Teacher teacher) : this()
		{
			ObjectIsNew = objectIsNew;

			if (ObjectIsNew)
				ValueRef = new Model.Teacher();
			else
				ValueRef = teacher;

			_editTeacherViewModel = new EditTeacherViewModel(ValueRef,groups,subjects,teacherPhones);
			DataContext = _editTeacherViewModel;
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


		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (string.IsNullOrWhiteSpace(ValueRef.Name))
				return new KeyValuePair<bool, string>(false, "Введите не пустое значение для названия предмета");

			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
