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
		protected EditPageStudent()
		{
			InitializeComponent();
		}


		public EditPageStudent(List<Student> students, List<Model.Group> groups, bool objectIsNew, Student model) : this()
		{
			DataContext = new EditStudentViewModel(objectIsNew ? new Student{ BirthDay = new DateTime(2005, 1, 1), Gender = "М"} : model, students, groups, objectIsNew);
		}
		#region Фильтрация ввода
		private void RussianTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить только русские буквы, пробелы и дефис
			var regex = new Regex(@"^[\p{IsCyrillic}\s\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}
		private void Email_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled=HasInvalidEmailChars(e.Text);
		}
		private static readonly Regex forbiddenCharsRegex = new Regex
		(
			@"[^a-z.%+-@-]", // Инвертированный набор разрешённых символов
			RegexOptions.IgnoreCase
		);
		public static bool HasInvalidEmailChars(string input)
		{
			return forbiddenCharsRegex.IsMatch(input);
		}
		#endregion
		#region Фокус полей ввода
		private void NameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			(DataContext as ViewModel.Edit.EditStudentViewModel).Name = textBox.Text;
		}
		private void SurnameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			(DataContext as ViewModel.Edit.EditStudentViewModel).Surname= textBox.Text;
		}
		private void PatronymicTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			(DataContext as ViewModel.Edit.EditStudentViewModel).Patronymic = textBox.Text;
		}
		private void EmailTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			(DataContext as ViewModel.Edit.EditStudentViewModel).Email = textBox.Text;
		}
		#endregion

		public KeyValuePair<bool,string> CheckInputRules()
		{
			return (DataContext as EditStudentViewModel).CheckInputRules();
		}
	}
}
