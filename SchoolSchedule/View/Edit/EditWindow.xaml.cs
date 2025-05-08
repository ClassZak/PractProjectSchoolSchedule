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
		private List<Model.Schedule> _schedules=new List<Model.Schedule>();
		private List<Model.Student> _students=new List<Model.Student>();
		private List<Model.Subject> _subjects = new List<Model.Subject>();
		private List<Model.Teacher> _teachers = new List<Model.Teacher>();
		private List<Model.TeacherPhone> _teacherPhones =new List<Model.TeacherPhone>();
		private List<Model.BellScheduleType> _bellScheduleTypes = new List<Model.BellScheduleType>();
		private List<Model.BellSchedule> _bellSchedules = new List<Model.BellSchedule>();
		private List<Model.LessonSubsitutionSchedule> _lessonSubsitutionSchedules = new List<Model.LessonSubsitutionSchedule>();


		private EditWindow()
		{
			InitializeComponent();
		}
		public EditWindow
		(
			Type editType,
			object editObject,
			List<Model.Group> groups,
			List<Model.Schedule> schedules,
			List<Model.Student> students,
			List<Model.Subject> subjects,
			List<Model.Teacher> teachers,
			List<Model.TeacherPhone> teacherPhones,
			List<Model.BellScheduleType> bellScheduleTypes,
			List<Model.BellSchedule> bellSchedules,
			List<Model.LessonSubsitutionSchedule> lessonSubsitutionSchedules,
			Window ownerWindow
		) : this()
		{
			Owner=ownerWindow;	

			EditType = editType;
			EditObject = editObject;

			_groups = groups;
			_schedules = schedules;
			_students = students;
			_subjects = subjects;
			_teachers = teachers;
			_teacherPhones=teacherPhones;
			_bellScheduleTypes=bellScheduleTypes;
			_bellSchedules = bellSchedules;
			_lessonSubsitutionSchedules= lessonSubsitutionSchedules;

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
			else if (EditType.Name == typeof(Model.Group).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый класс";
					EditObject = new Model.Group { Year = 1, Name = "А" };
				}
				else
					newObjectLabel.Text = $"Изменение класса \"{(EditObject as Model.Group).Year}{(EditObject as Model.Group).Name}\"";

				mainFrame.Content = new EditPage.EditPageGroup(EditObject as Model.Group, _groups);
			}
			else if (EditType.Name == typeof(Model.Student).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый ученик";
					EditObject = new Model.Student();
				}
				else
					newObjectLabel.Text = $"Изменение данных ученика \"{(EditObject as Model.Student).Surname} {(EditObject as Model.Student).Name} {(EditObject as Model.Student).Patronymic}\"";

				Width = 590;
				Height = 500;
				mainFrame.Content = new EditPage.EditPageStudent(_students, _groups, isNewObject, EditObject as Model.Student);
			}
			else if (EditType.Name == typeof(Model.Teacher).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый учитель";
					EditObject = new Model.Teacher();
				}
				else
					newObjectLabel.Text = $"Изменение данных учителя \"{EditObject as Model.Teacher}\"";

				Width = 700;
				Height = 600;
				mainFrame.Content = new EditPage.EditPageTeacher(_teachers, _groups, _subjects, _teacherPhones, isNewObject, EditObject as Model.Teacher, this);
			}
			else if (EditType.Name == typeof(Model.TeacherPhone).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новый телефонный номер";
					EditObject = new Model.TeacherPhone();
				}
				else
					newObjectLabel.Text = $"Редактирование номера";

				mainFrame.Content = new EditPage.EditPageTeacherPhone(EditObject as Model.TeacherPhone, _teacherPhones);
			}
			else if (EditType.Name == typeof(Model.Schedule).Name)
			{
				Width = 590;
				Height = 500;

				if (isNewObject)
				{
					newObjectLabel.Text = "Новый урок";
					EditObject = new Model.Schedule();
				}
				else
					newObjectLabel.Text = $"Редактирование расписания";

				mainFrame.Content = new EditPage.EditPageSchedule(isNewObject, EditObject as Model.Schedule, _schedules, _subjects,_groups, _teachers,_bellSchedules);
			}
			else if (EditType.Name == typeof(Model.BellScheduleType).Name)
			{
				if (isNewObject)
				{
					newObjectLabel.Text = "Новое расписание звонков";
					EditObject = new Model.BellScheduleType();
				}
				else
					newObjectLabel.Text = $"Редактирование названия расписания звонков";

				mainFrame.Content = new EditPage.EditPageBellScheduleType(EditObject as Model.BellScheduleType, _bellScheduleTypes);
			}
			else if (EditType.Name == typeof(Model.BellSchedule).Name)
			{
				Width = 590;
				Height = 500;

				if (isNewObject)
				{
					newObjectLabel.Text = "Новые звонки";
					EditObject = new Model.BellSchedule();
				}
				else
					newObjectLabel.Text = $"Редактирование расписания звонков";

				mainFrame.Content = new EditPage.EditPageBellSchedule(isNewObject, EditObject as Model.BellSchedule, _bellSchedules,_bellScheduleTypes);
			}
			else if (EditType.Name == typeof(Model.LessonSubsitutionSchedule).Name)
			{
				Width = 590;
				Height = 500;

				if (isNewObject)
				{
					newObjectLabel.Text = "Новая заменая уроков";
					EditObject = new Model.LessonSubsitutionSchedule();
				}
				else
					newObjectLabel.Text = "Редактирование замены уроков";

				mainFrame.Content = new EditPage.EditPageLessonSubsitutionSchedule(isNewObject, EditObject as Model.LessonSubsitutionSchedule, _lessonSubsitutionSchedules, _subjects,_groups,_teachers);
			}
		}
		#region Кнопки
		private void Button_Click_Ok(object sender, RoutedEventArgs e)
		{
			if (EditType.Name == typeof(Model.Subject).Name)
			{
				KeyValuePair<bool, string> checkResult = ((mainFrame.Content as EditPage.EditPageSubject).DataContext as ViewModel.Edit.EditSubjectViewModel).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта",MessageBoxButton.OK,MessageBoxImage.Stop);
					return;
				}
			}
			else if (EditType.Name == typeof(Model.Group).Name)
			{
				KeyValuePair<bool, string> checkResult = ((mainFrame.Content as EditPage.EditPageGroup).DataContext as ViewModel.Edit.EditGroupViewModel).CheckInputRules();
				if (!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
			}
			else if(EditType.Name==typeof(Model.Student).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageStudent).CheckInputRules();
				if (!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
				EditObject = ((mainFrame.Content as EditPage.EditPageStudent).DataContext as EditStudentViewModel).CurrentModel;
			}
			else if (EditType.Name == typeof(Model.Teacher).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageTeacher).CheckInputRules();
				if (!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
				var newTeacher = ((mainFrame.Content as EditPage.EditPageTeacher).DataContext as EditTeacherViewModel).CurrentModel;
				var viewModel = ((mainFrame.Content as EditPage.EditPageTeacher).DataContext as EditTeacherViewModel);
				newTeacher.Subject.Clear();
				newTeacher.Group.Clear();
				newTeacher.TeacherPhone.Clear();
				foreach (var el in viewModel.ChoosenSubjects)
					newTeacher.Subject.Add(el);
				foreach (var el in viewModel.ChoosenGroups)
					newTeacher.Group.Add(el);
				foreach (var el in viewModel.TeacherPhones)
					newTeacher.TeacherPhone.Add(el.ModelRef);

				EditObject = newTeacher;
			}
			else if(EditType.Name==typeof (Model.TeacherPhone).Name)
			{
				KeyValuePair<bool, string> checkResult = ((mainFrame.Content as EditPage.EditPageTeacherPhone).DataContext as ViewModel.Edit.EditTeacherPhoneViewModel).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;					
				}
			}
			else if(EditType.Name==typeof (Model.Schedule).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageSchedule).CheckInputRules();
				if(!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;					
				}
				EditObject = ((mainFrame.Content as EditPage.EditPageSchedule).DataContext as EditScheduleViewModel).CurrentSchedule;
			}
			else if(EditType.Name==typeof(Model.BellScheduleType).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageBellScheduleType).CheckInputRules();
				if(!checkResult.Key) 
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
			}
			else if (EditType.Name == typeof(Model.BellSchedule).Name)
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageBellSchedule).CheckInputRules();
				if (!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
				EditObject = ((mainFrame.Content as EditPage.EditPageBellSchedule).DataContext as EditBellScheduleViewModel).CurrentModel;
			}
			else if(EditType.Name==typeof(Model.LessonSubsitutionSchedule).Name) 
			{
				KeyValuePair<bool, string> checkResult = (mainFrame.Content as EditPage.EditPageLessonSubsitutionSchedule).CheckInputRules();
				if (!checkResult.Key)
				{
					MessageBox.Show(checkResult.Value, "Ошибка редактирования атрибутов объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
					return;
				}
				EditObject=((mainFrame.Content as EditPage.EditPageLessonSubsitutionSchedule).DataContext as EditLessonSubsitutionScheduleViewModel).CurrentLessonSubsitutionSchedule;
			}

			DialogResult =true;
			Close();
		}

		private void Button_Click_Cancel(object sender, RoutedEventArgs e)
		{
			DialogResult =false;
			Close();
		}
		#endregion
	}
}
