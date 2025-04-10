using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel
{
	public class MainViewModel : ABaseViewModel
	{
		public MainViewModel()
		{
			using(Model.SchoolScheduleEntities dataBase=new Model.SchoolScheduleEntities())
			{
				dataBase.Configuration.ProxyCreationEnabled = false;
				dataBase.Configuration.LazyLoadingEnabled = false;

				_groups			= new ObservableCollection<Model.Group>			(dataBase.Groups.		ToList());
				_lessons		= new ObservableCollection<Model.Lesson>		(dataBase.Lessons.		ToList());
				_schedules		= new ObservableCollection<Model.Schedule>		(dataBase.Schedules.	ToList());
				_students		= new ObservableCollection<Model.Student>		(dataBase.Students.		ToList());
				_subjects		= new ObservableCollection<Model.Subject>		(dataBase.Subjects.		ToList());
				_teachers		= new ObservableCollection<Model.Teacher>		(dataBase.Teachers.		ToList());
				_teacherPhones	= new ObservableCollection<Model.TeacherPhone>	(dataBase.TeacherPhones.ToList());
			}
		}

		private ObservableCollection<Model.Group> _groups;
		public ObservableCollection<Model.Group> Groups
		{ get {return _groups;}set {SetPropertyChanged(ref _groups, value);} }

		private ObservableCollection<Model.Lesson> _lessons;
		public ObservableCollection<Model.Lesson> Lessons
		{ get { return _lessons; } set { SetPropertyChanged(ref _lessons, value); } }

		private ObservableCollection<Model.Schedule> _schedules;
		public ObservableCollection<Model.Schedule> Schedules
		{ get { return _schedules; } set { SetPropertyChanged(ref _schedules, value); } }

		private ObservableCollection<Model.Student> _students;
		public ObservableCollection<Model.Student> Students
		{ get { return _students; } set { SetPropertyChanged(ref _students, value); } }

		private ObservableCollection<Model.Subject> _subjects;
		public ObservableCollection<Model.Subject> Subjects
		{ get { return _subjects; } set { SetPropertyChanged(ref _subjects, value); } }

		private ObservableCollection<Model.Teacher> _teachers;
		public ObservableCollection<Model.Teacher> Teachers
		{ get { return _teachers; } set { SetPropertyChanged(ref _teachers, value); } }

		private ObservableCollection<Model.TeacherPhone> _teacherPhones;
		public ObservableCollection<Model.TeacherPhone> TeacherPhones
		{ get { return _teacherPhones; } set { SetPropertyChanged(ref _teacherPhones, value); } }
	}
}
