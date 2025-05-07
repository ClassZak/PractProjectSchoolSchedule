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
	/// Логика взаимодействия для EditPageSubject.xaml
	/// </summary>
	public partial class EditPageSubject : Page, IEditPage<Subject>
	{
		public EditPageSubject()
		{
			InitializeComponent();
		}
		public EditPageSubject(Subject currentModel, List<Subject> modelsForUniqueCheck) : this()
		{
			List<Subject> modelsForViewModel = new List<Subject> (modelsForUniqueCheck);
			if(currentModel != null)
				modelsForViewModel.Remove(currentModel);

			DataContext = new EditSubjectViewModel(currentModel, modelsForViewModel);
		}

		#region Фильтрация ввода
		private void RussianTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить только русские буквы, пробелы, дефис и цифры
			var regex = new Regex(@"^[\p{IsCyrillic}\s\-\d]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}
		#endregion
		#region Фокус полей ввода
		private void NameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));

			(DataContext as ViewModel.Edit.EditSubjectViewModel).Name = textBox.Text;
		}
		#endregion
		public KeyValuePair<bool,string> CheckInputRules()
		{
			return (DataContext as EditSubjectViewModel).CheckInputRules();
		}
	}
}
