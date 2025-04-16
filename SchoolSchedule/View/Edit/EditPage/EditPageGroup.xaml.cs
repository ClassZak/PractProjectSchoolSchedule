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
		public List<Model.Group> GroupsForCheck { get; set; } = new List<Model.Group>();
		public Model.Group ValueRef { get; set; } = new Model.Group();
		public EditPageGroup()
		{
			InitializeComponent();
			DataContext = ValueRef;
		}
		public EditPageGroup(Model.Group group, List<Model.Group> groups) : this()
		{
			ValueRef = group;

			foreach (var el in groups)
				GroupsForCheck.Add(new Model.Group { Id=el.Id, Year=el.Year, Name=el.Name });

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

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (string.IsNullOrWhiteSpace(ValueRef.Name))
				return new KeyValuePair<bool, string>(false, "Введите непустое значение для группы класса");
			string upperStr = ValueRef.Name.ToUpper();
			if (upperStr.Length != 1 || (upperStr[0] < 'А' || upperStr[0] > 'Е'))
				return new KeyValuePair<bool, string>(false, "Введите одну русскую букву от А до Е для определения группы");
			if (ValueRef.Year<1 || ValueRef.Year>11)
				return new KeyValuePair<bool, string>(false, "Введите номер года обучения от 1 до 11 включительно");
			if (GroupsForCheck.Where(el => el.Name == ValueRef.Name && el.Year==ValueRef.Year).Any())
				return new KeyValuePair<bool, string>(false, $"Класс \"{ValueRef.Year}{ValueRef.Name}\" уже существует");

			return new KeyValuePair<bool, string>(true, null);
		
		}
	}
}
