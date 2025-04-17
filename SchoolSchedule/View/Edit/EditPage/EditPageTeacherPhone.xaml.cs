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
		private void Phome_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex(@"^[\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}

		public KeyValuePair<bool,string> CheckInputRules()
		{
			
				
			return new KeyValuePair<bool,string>(true,null);
		}
	}
}
