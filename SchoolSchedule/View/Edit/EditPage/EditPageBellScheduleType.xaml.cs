using SchoolSchedule.Model;
using System;
using System.Collections.Generic;
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
	/// Логика взаимодействия для EditPageBellScheduleType.xaml
	/// </summary>
	public partial class EditPageBellScheduleType : Page, IEditPage<BellScheduleType>
	{
		public List<BellScheduleType> BellScheduleTypesForCheck { get; set; } = new List<BellScheduleType>();
		public EditPageBellScheduleType()
		{
			InitializeComponent();
		}
		public EditPageBellScheduleType(BellScheduleType currentModel, List<BellScheduleType> modelsForUniqueCheck) : this()
		{
			List<BellScheduleType> modelsForViewModel=new List<BellScheduleType>(modelsForUniqueCheck);
			if (currentModel != null)
				modelsForViewModel.Remove(currentModel);

			DataContext = new ViewModel.Edit.EditBellScheduleTypeViewModel(currentModel, modelsForViewModel);
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

			(DataContext as ViewModel.Edit.EditBellScheduleTypeViewModel).Name = textBox.Text;
		}
		#endregion
		public KeyValuePair<bool, string> CheckInputRules()
		{
			return (DataContext as ViewModel.Edit.EditBellScheduleTypeViewModel).CheckInputRules();
		}
	}
}
