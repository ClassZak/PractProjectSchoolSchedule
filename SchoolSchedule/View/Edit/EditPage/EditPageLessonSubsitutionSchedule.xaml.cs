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
		public bool ObjectIsNew{  get; set; }
		
		public List<Model.LessonSubsitutionSchedule> LessonSubsitutionSchedulesForCheck { get; set; } = new List<Model.LessonSubsitutionSchedule>();
		public Model.LessonSubsitutionSchedule ValueRef { get ; set ; }
		protected EditPageLessonSubsitutionSchedule()
		{
			InitializeComponent();
			DataContext = ValueRef;
		}


		private EditLessonSubsitutionScheduleViewModel _viewModel;
		public EditPageLessonSubsitutionSchedule
		(
			bool objectIsNew, 
			LessonSubsitutionSchedule lessonSubsitutionSchedule,
			List<LessonSubsitutionSchedule> lessonSubsitutionSchedules, 
			List<Model.Subject> subjects,
			List<Model.Group> groups,
			List<Model.Teacher> teachers
		) : this()
		{
			ObjectIsNew= objectIsNew;
			LessonSubsitutionSchedulesForCheck= lessonSubsitutionSchedules;

			if (ObjectIsNew)
				ValueRef = new Model.LessonSubsitutionSchedule();
			else
				ValueRef = lessonSubsitutionSchedule;

			_viewModel = new EditLessonSubsitutionScheduleViewModel(ObjectIsNew,ValueRef,subjects,groups,teachers);
			DataContext = _viewModel;
		}


		private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Разрешить byte
			var textBox = (TextBox)sender;
			string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
			e.Handled = !byte.TryParse(newText, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
		}
		public KeyValuePair<bool,string> CheckInputRules()
		{
			var obj=(this.DataContext as EditLessonSubsitutionScheduleViewModel).CurrentLessonSubsitutionSchedule;
			return new KeyValuePair<bool,string>(true,null);
		}
	}
}
