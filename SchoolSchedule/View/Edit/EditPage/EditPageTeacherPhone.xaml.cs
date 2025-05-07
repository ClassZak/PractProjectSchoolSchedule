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
	/// Логика взаимодействия для EditPageTeacherPhone.xaml
	/// </summary>
	public partial class EditPageTeacherPhone : Page, IEditPage<TeacherPhone>
	{
		public List<TeacherPhone> TeacherPhonesForCheck { get; set; } = new List<TeacherPhone>();
		public TeacherPhone ValueRef{ get ; set ; }
		public EditPageTeacherPhone()
		{
			InitializeComponent();
		}
		public EditPageTeacherPhone(TeacherPhone teacherPhone, List<TeacherPhone> teacherPhones) : this()
		{
			ValueRef = teacherPhone;

			foreach(var el in teacherPhones)
				TeacherPhonesForCheck.Add(el);
			if(teacherPhone != null) 
				TeacherPhonesForCheck.Remove(teacherPhone);

			DataContext = new EditTeacherPhoneViewModel(ValueRef,TeacherPhonesForCheck);
		}

		#region Фильтрация ввода
		private void PhonePreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var textBox = (TextBox)sender;
			string currentText = textBox.Text;
			int caretIndex = textBox.CaretIndex;
			string newText = currentText.Insert(caretIndex, e.Text);

			// Блокировка, если первый символ не '+'
			if (currentText.Length == 0 && e.Text != "+")
			{
				e.Handled = true;
				return;
			}

			// Блокировка, если второй символ не '7'
			if (currentText.Length == 1 && (currentText + e.Text) != "+7")
			{
				e.Handled = true;
				return;
			}

			// Разрешаем только цифры, пробелы и дефисы после "+7"
			if (currentText.Length >= 2 && !Regex.IsMatch(e.Text, @"^[\d \-]*$"))
				e.Handled = true;
		}

		private void PhonePreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));

			// Блокировка удаления первых двух символов
			if (((e.Key == Key.Back && textBox.SelectionStart <= 2) ||
				(e.Key == Key.Delete && textBox.SelectionStart < 2)) && textBox.Text.StartsWith("+7"))
				e.Handled = true;

			// Проверка вставляемого текста
			if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				string clipboardText = Clipboard.GetText();
				if (!clipboardText.StartsWith("+7") || !Regex.IsMatch(clipboardText.Substring(2), @"^[\d \-]*$"))
					e.Handled = true;
			}
		}
		#endregion
		#region Фокус полей ввода
		private void PhoneLostFocus(object sender, RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));

			(DataContext as ViewModel.Edit.EditTeacherPhoneViewModel).PhoneNumber = textBox.Text;
		}
		#endregion
		public KeyValuePair<bool,string> CheckInputRules()
		{
			return (DataContext as EditTeacherPhoneViewModel).CheckInputRules();
		}
	}
}
