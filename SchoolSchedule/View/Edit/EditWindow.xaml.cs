using SchoolSchedule.ViewModel.Edit;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SchoolSchedule.View.Edit
{
	/// <summary>
	/// Логика взаимодействия для EditWindow.xaml
	/// </summary>
	public partial class EditWindow : Window
	{
		private bool isNewObject=false;
		
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
			if (EditObject == null)
				isNewObject = true;
			if (isNewObject)
			{
				Title = "Добавление";
				okButton.Content = "Добавить";
			}


			if (EditType.Name == typeof(Model.Subject).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый предмет";
					EditObject = new Model.Subject();
				}
				else
					newObjectLabel.Text = $"Изменение предмета \"{(EditObject as Model.Subject).Name}\"";

				mainFrame.Content = new EditPage.EditPageSubject(EditObject as Model.Subject, _subjects);
			}
			if (EditType.Name == typeof(Model.Group).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый класс";
					EditObject = new Model.Group { Year = 1, Name="А" }; 
				}
				else
					newObjectLabel.Text = $"Изменение класса \"{(EditObject as Model.Group).Year}{(EditObject as Model.Group).Name}\"";

				mainFrame.Content = new EditPage.EditPageGroup(EditObject as Model.Group, _groups);
			}
			if(EditType.Name==typeof(Model.Student).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый ученик";
					EditObject = new Model.Student();
				}
				else
					newObjectLabel.Text = $"Изменение данных ученика \"{(EditObject as Model.Student).Surname} {(EditObject as Model.Student).Name} {(EditObject as Model.Student).Patronymic}\"";

				mainFrame.Content = new EditPage.EditPageStudent(_students, _groups, isNewObject, EditObject as Model.Student);
			}
			if(EditType.Name==typeof(Model.Teacher).Name)
			{
				if(isNewObject)				
				{
					newObjectLabel.Text = "Новый учитель";
					EditObject = new Model.Teacher();
				}
				else
					newObjectLabel.Text = $"Изменение данных учителя \"{EditObject as Model.Teacher}\"";

				Width = 590;
				Height = 500;
				mainFrame.Content = new EditPage.EditPageTeacher(_teachers,_groups,_subjects,_teacherPhones,isNewObject,EditObject as Model.Teacher);
			}
			if (EditType.Name == typeof(Model.TeacherPhone).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый телефонный номер";
					EditObject = new Model.TeacherPhone();
				}
				else
					newObjectLabel.Text = $"Редактирование номера";

				mainFrame.Content = new EditPage.EditPageTeacherPhone(EditObject as Model.TeacherPhone,_teacherPhones);
			}
			if(EditType.Name==typeof(Model.Lesson).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый урок";
					EditObject = new Model.Lesson();
				}
				else
					newObjectLabel.Text = $"Редактирование урока";

				mainFrame.Content=new EditPage.EditPageLesson(_lessons,_groups,_subjects,isNewObject,EditObject as Model.Lesson);
			}
		}

		private void Button_Click_Ok(object sender, RoutedEventArgs e)
		{
			if (EditType.Name == typeof(Model.Subject).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageSubject).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта",MessageBoxButton.OK,MessageBoxImage.Stop);
					return;
				}
			}
			if (EditType.Name == typeof(Model.Group).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageGroup).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта",MessageBoxButton.OK,MessageBoxImage.Stop);
					return;
				}
			}
			if(EditType.Name==typeof(Model.Student).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageStudent).CheckInputRules();
				if (!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
				EditObject = ((mainFrame.Content as EditPage.EditPageStudent).DataContext as EditStudentViewModel).CurrentStudent;
			}
			if(EditType.Name==typeof (Model.TeacherPhone).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageTeacherPhone).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;					
				}
			}
			if(EditType.Name==typeof (Model.Lesson).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageLesson).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;					
				}
				EditObject = ((mainFrame.Content as EditPage.EditPageLesson).DataContext as EditLessonViewModel).CurrentLesson;
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
