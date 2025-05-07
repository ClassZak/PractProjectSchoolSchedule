using SchoolSchedule.Model;
using SchoolSchedule.ViewModel.Edit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace SchoolSchedule.View.Edit.EditPage
{
	/// <summary>
	/// Логика взаимодействия для EditPageSubject.xaml
	/// </summary>
	public partial class EditPageGroup : Page, IEditPage<Model.Group>
	{
		public EditPageGroup()
		{
			InitializeComponent();
		}
		public EditPageGroup(Model.Group currentModel, List<Model.Group> modelsForUniqueCheck) : this()
		{
			List<Model.Group> modelsForViewModel = new List<Model.Group>(modelsForUniqueCheck);
			if (currentModel != null)
				modelsForViewModel.Remove(currentModel);

			DataContext = new EditGroupViewModel(currentModel, modelsForViewModel);
		}

		#region Фильтрация ввода
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
		#endregion
		#region Фокус полей ввода
		private void NameTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException();
			
			(DataContext as ViewModel.Edit.EditGroupViewModel).Name=textBox.Text;
        }
		private void YearTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));

			int.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int data);
			(DataContext as ViewModel.Edit.EditGroupViewModel).Year=data;
        }
		#endregion
		public KeyValuePair<bool, string> CheckInputRules()
		{
			return (DataContext as ViewModel.Edit.EditGroupViewModel).CheckInputRules();
		}
	}
}
