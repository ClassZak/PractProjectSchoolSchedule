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
	/// Логика взаимодействия для EditPageTeacher.xaml
	/// </summary>
	public partial class EditPageTeacher : Page
	{
		public Window OwnerWindow { get; set; }
		protected EditPageTeacher()
		{
			InitializeComponent();
		}
		public EditPageTeacher(List<Model.Teacher> teachers,List<Model.Group> groups, List<Model.Subject> subjects, List<Model.TeacherPhone> teacherPhones, bool objectIsNew,Model.Teacher model, Window window) : this()
		{
			OwnerWindow = window;
			DataContext = new EditTeacherViewModel(objectIsNew ? new Teacher { BirthDay = new DateTime(1970, 1, 1), Gender = "М" } : model, teachers, groups, subjects, teacherPhones, objectIsNew, window);
		}
		private void RussianTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить только русские буквы, пробелы и дефис
			var regex = new Regex(@"^[\p{IsCyrillic}\s\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}

		#region Фокус полей ввода
		private void NameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			(DataContext as ViewModel.Edit.EditTeacherViewModel).Name = textBox.Text;
		}
		private void SurnameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			(DataContext as ViewModel.Edit.EditTeacherViewModel).Surname = textBox.Text;
		}
		private void PatronymicTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));
			(DataContext as ViewModel.Edit.EditTeacherViewModel).Patronymic = textBox.Text;
		}
		#endregion
		public KeyValuePair<bool, string> CheckInputRules()
		{
			return (DataContext as EditTeacherViewModel).CheckInputRules();
		}
	}
}
