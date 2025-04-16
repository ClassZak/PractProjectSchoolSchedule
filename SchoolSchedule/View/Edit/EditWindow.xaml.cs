using SchoolSchedule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SchoolSchedule.View.Edit
{
	/// <summary>
	/// Логика взаимодействия для EditWindow.xaml
	/// </summary>
	public partial class EditWindow : Window
	{
		public Type EditType { get; set; }
		public object EditObject { get; set; }
		public new bool DialogResult { get; set; } = false;

		private List<Model.Group> _groups=new List<Model.Group>();
		private List<Model.Lesson> _lessons=new List<Model.Lesson>();
		private List<Model.Schedule> _schedules=new List<Model.Schedule>();
		private List<Model.Student> _students=new List<Model.Student>();
		private List<Model.Subject> _subjects = new List<Model.Subject>();
		private List<Model.Teacher> _teachers = new List<Model.Teacher>();
		private List<Model.TeacherPhone> _teacherPhones =new List<Model.TeacherPhone>();


		private EditWindow()
		{
			InitializeComponent();
		}
		public EditWindow
		(
			Type editType,
			object editObject,
			List<Model.Group> groups,
			List<Model.Lesson> lessons ,
			List<Model.Schedule> schedules,
			List<Model.Student> students,
			List<Model.Subject> subjects,
			List<Model.Teacher> teachers,
			List<Model.TeacherPhone> teacherPhones
		) : this()
		{
			EditType = editType;
			EditObject = editObject;

			_groups = groups;
			_lessons = lessons;
			_schedules = schedules;
			_students = students;
			_subjects = subjects;
			_teachers = teachers;
			_teacherPhones=teacherPhones;

			SelectPage();
		}

		void SelectPage()
		{
			if(EditObject==null)
			{
				Title = "Добавление";
				okButton.Content="Добавить";
			}
			if (EditType.Name == typeof(Model.Subject).Name)
			{
				if(EditObject==null)
					EditObject = new Model.Subject();
				mainFrame.Content = new EditPage.EditPageSubject(EditObject as Subject,_subjects);
			}
		}

		private void Button_Click_Ok(object sender, RoutedEventArgs e)
		{
			if (EditType.Name == typeof(Model.Subject).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageSubject).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value,"Ошибка редактирования",MessageBoxButton.OK,MessageBoxImage.Stop);
					return;
				}
			}

			DialogResult=true;
			Close();
		}

		private void Button_Click_Cancel(object sender, RoutedEventArgs e)
		{
			DialogResult =false;
			Close();
		}
	}
}
