using SchoolSchedule.Model;
using SchoolSchedule.Model.DTO;
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
using System.Windows.Shapes;

namespace SchoolSchedule.View
{
	/// <summary>
	/// Логика взаимодействия для GroupAddWindow.xaml
	/// </summary>
	public partial class SubjectAddWindow : Window
	{
		private SubjectAddWindow()
		{
			InitializeComponent();
			DataContext = NewSubject;
		}

		public bool dialogResult = false;

		public Model.Subject NewSubject { get; set; } = new Model.Subject();

		private List<DTOSubject> _subjects=new List<DTOSubject>();
		public SubjectAddWindow(ICollection<DTOSubject> list):this()
		{
			foreach(var el in list)
				_subjects.Add(el);
		}

		private void Button_Click_Cancel(object sender, RoutedEventArgs e)
		{
			dialogResult = false;

			this.Close();
		}
		private void Button_Click_Add(object sender, RoutedEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(NewSubject.Name))
			{
				MessageBox.Show("Введите название нвого предмета", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (_subjects.Find(x => x.Name == NewSubject.Name) != null)
			{
				MessageBox.Show($"Предмет {NewSubject.Name} уже существует", "Ошибка добавления элемента", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			dialogResult = true;
			Close();
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
	}
}
