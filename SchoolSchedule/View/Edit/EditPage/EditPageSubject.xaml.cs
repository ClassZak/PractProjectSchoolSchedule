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
	/// Логика взаимодействия для EditPageSubject.xaml
	/// </summary>
	public partial class EditPageSubject : Page, IEditPage<Subject>
	{
		public List<Subject> SubjectsForCheck { get; set; } = new List<Subject>();
		public Subject ValueRef{ get ; set ; }
		public EditPageSubject()
		{
			InitializeComponent();
			DataContext = ValueRef ;
		}
		public EditPageSubject(Subject subject, List<Subject> subjects) : this()
		{
			ValueRef = subject;

			foreach (var el in subjects)
				SubjectsForCheck.Add(new Subject { Id=el.Id,Name=el.Name});

			DataContext = ValueRef;
		}
		private void RussianTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить только русские буквы, пробелы и дефис
			var regex = new Regex(@"^[\p{IsCyrillic}\s\-]+$");
			e.Handled = !regex.IsMatch(e.Text);
		}

		public KeyValuePair<bool,string> CheckInputRules()
		{
			if (string.IsNullOrWhiteSpace(ValueRef.Name))
				return new KeyValuePair<bool, string>(false, "Введите не пустое значение для названия предмета");
			if(SubjectsForCheck.Where(el=>el.Name==ValueRef.Name).Any())
				return new KeyValuePair<bool, string>(false, $"Предмет \"{ValueRef.Name}\" уже присутствует в базе данных");
				
			return new KeyValuePair<bool,string>(true,null);
		}
	}
}
