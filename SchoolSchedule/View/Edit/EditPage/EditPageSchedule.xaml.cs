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
	/// Логика взаимодействия для EditPageSubject.xaml
	/// </summary>
	public partial class EditPageSchedule : Page, IEditPage<Model.Schedule>
	{
		protected EditPageSchedule()
		{
			InitializeComponent();
		}


		private EditScheduleViewModel _viewModel;
		public EditPageSchedule
		(
			bool objectIsNew,
			Model.Schedule model,
			List<Model.Schedule> modelsForUniqueCheck,
			List<Model.Subject> subjects,
			List<Model.Group> groups,
			List<Model.Teacher> teachers,
			List<Model.BellSchedule> bellSchedules
		) : this()
		{
			DataContext = new ViewModel.Edit.EditScheduleViewModel
			(
				objectIsNew,
				model,
				modelsForUniqueCheck, 
				subjects, 
				groups, 
				teachers, 
				bellSchedules
			);
		}


		private void Int16NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить short
			var textBox = (TextBox)sender;
			string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
			e.Handled = !short.TryParse(newText, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
		}
		private void ClassRoomTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));

			if (int.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int data))
				(DataContext as ViewModel.Edit.EditScheduleViewModel).ClassRoom = data;
		}
		public KeyValuePair<bool, string> CheckInputRules()
		{
			return (DataContext as EditScheduleViewModel).CheckInputRules();
		}
	}
}
