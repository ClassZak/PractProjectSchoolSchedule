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
	public partial class EditPageLessonSubsitutionSchedule : Page, IEditPage<Model.LessonSubsitutionSchedule>
	{
		protected EditPageLessonSubsitutionSchedule()
		{
			InitializeComponent();
		}


		private EditLessonSubsitutionScheduleViewModel _viewModel;
		public EditPageLessonSubsitutionSchedule
		(
			bool objectIsNew, 
			LessonSubsitutionSchedule model,
			List<LessonSubsitutionSchedule> modelsForUniqueCheck, 
			List<Model.Subject> subjects,
			List<Model.Group> groups,
			List<Model.Teacher> teachers
		) : this()
		{
			DataContext = new ViewModel.Edit.EditLessonSubsitutionScheduleViewModel
			(
				model, 
				modelsForUniqueCheck, 
				subjects, 
				groups, 
				teachers, 
				objectIsNew
			);
		}


		private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить byte
			var textBox = (TextBox)sender;
			string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
			e.Handled = !byte.TryParse(newText, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
		}
		#region Фокус полей ввода
		private void LessonNumberTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));

			if(int.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int data))
				(DataContext as ViewModel.Edit.EditLessonSubsitutionScheduleViewModel).LessonNumber = data;
			else
				(DataContext as ViewModel.Edit.EditLessonSubsitutionScheduleViewModel).LessonNumber = 1;
		}
		private void ClassRoomTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!(sender is TextBox textBox))
				throw new ArgumentException(nameof(sender));

			if (int.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int data))
				(DataContext as ViewModel.Edit.EditLessonSubsitutionScheduleViewModel).ClassRoom = data;
			else
				(DataContext as ViewModel.Edit.EditLessonSubsitutionScheduleViewModel).ClassRoom = null;
		}
		#endregion
		public KeyValuePair<bool,string> CheckInputRules()
		{
			return (DataContext as EditLessonSubsitutionScheduleViewModel).CheckInputRules();
		}
	}
}
