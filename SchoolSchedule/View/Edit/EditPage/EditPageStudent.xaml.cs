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
	public partial class EditPageStudent : Page, IEditPage<Student>
	{
		public bool ObjectIsNew{  get; set; }
		
		public List<Student> StudentsForCheck { get; set; } = new List<Student>();
		public Student ValueRef{ get ; set ; }
		protected EditPageStudent()
		{
			InitializeComponent();
			DataContext = ValueRef;
		}


		private EditStudentViewModel _viewModel;
		public EditPageStudent(List<Student> students, List<Model.Group> groups, bool objectIsNew, Student student) : this()
		{
			ObjectIsNew= objectIsNew;

			if (ObjectIsNew)
				ValueRef = new Student();
			else
				ValueRef = student;

			_viewModel = new EditStudentViewModel(ValueRef, groups);
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
			if (string.IsNullOrWhiteSpace(ValueRef.Surname))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? "фамилию нового" : "новую фамилию") + " ученика");
			if (string.IsNullOrWhiteSpace(ValueRef.Name))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? "имя нового" : "новое имя") + " ученика");
			if (string.IsNullOrWhiteSpace(ValueRef.Patronymic))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? "отчество нового" : "новое отчество") + " ученика");
			if(!ObjectIsNew)
			if(StudentsForCheck.Where(el=>el.Name==ValueRef.Name && el.Surname==ValueRef.Surname && el.Patronymic==ValueRef.Patronymic).Any())
				return new KeyValuePair<bool, string>(false, $"Ученик \"{ValueRef.Surname} {ValueRef.Name} {ValueRef.Patronymic}\" уже существует");
			if(ValueRef.Email != null)
				if(string.IsNullOrWhiteSpace(ValueRef.Email))
					if(HasInvalidEmailChars(ValueRef.Email))
						return new KeyValuePair<bool, string>(false, $"Неверный формат электронной почты!");
						

			return new KeyValuePair<bool,string>(true,null);
		}
	}
}
