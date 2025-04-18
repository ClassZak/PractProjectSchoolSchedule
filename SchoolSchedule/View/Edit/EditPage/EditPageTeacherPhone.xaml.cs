using SchoolSchedule.Model;
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
			DataContext = ValueRef ;
		}
		public EditPageTeacherPhone(TeacherPhone teacherPhone, List<TeacherPhone> teacherPhones) : this()
		{
			ValueRef = teacherPhone;

			foreach (var el in teacherPhones)
				TeacherPhonesForCheck.Add(new TeacherPhone { Id=el.Id,IdTeacher=el.IdTeacher,PhoneNumber=el.PhoneNumber});

			DataContext = ValueRef;
		}

		#region События для номера телефона
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
			var textBox = (TextBox)sender;

			// Блокировка удаления первых двух символов
			if ((e.Key == Key.Back && textBox.SelectionStart <= 2) ||
				(e.Key == Key.Delete && textBox.SelectionStart < 2))
				e.Handled = true;

			// Проверка вставляемого текста
			if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
			{
				string clipboardText = Clipboard.GetText();
				if (!clipboardText.StartsWith("+7") || !Regex.IsMatch(clipboardText.Substring(2), @"^[\d \-]*$"))
					e.Handled = true;
			}
		}

		private void PhoneLostFocus(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			string rawDigits = Regex.Replace(textBox.Text.Substring(2), @"[^\d]", "");

			if (rawDigits.Length == 10)
				textBox.Text = $"+7 {rawDigits.Substring(0, 3)} {rawDigits.Substring(3, 3)}-{rawDigits.Substring(6, 2)}-{rawDigits.Substring(8, 2)}";
		}
		#endregion
		private void RussianTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить только русские буквы, пробелы и дефис
			var regex = new Regex(@"^[\p{IsCyrillic}\s\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}
		private void Phome_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex(@"^[\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}

		public KeyValuePair<bool,string> CheckInputRules()
		{
			if (string.IsNullOrWhiteSpace(ValueRef.PhoneNumber) || ValueRef.PhoneNumber==string.Empty)
				return new KeyValuePair<bool, string>(false, "Введите номер телефона!");
			if(!Regex.IsMatch(ValueRef.PhoneNumber,@"^\+7 \d{3} \d{3}-\d{2}-\d{2}$"))
				return new KeyValuePair<bool, string>(false, "Неверный формат номера телефона!\nПравильно: +7 123 456-78-90");

				
			return new KeyValuePair<bool,string>(true,null);
		}
	}
}
