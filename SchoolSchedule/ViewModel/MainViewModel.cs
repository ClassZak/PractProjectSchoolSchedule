using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel
{
	public class MainViewModel : ABaseViewModel
	{
		public MainViewModel()
		{
			_groups			= new ObservableCollection<Model.Group>			(new List<Model.Group>			());
			_lessons		= new ObservableCollection<Model.Lesson>		(new List<Model.Lesson>			());
			_schedules		= new ObservableCollection<Model.Schedule>		(new List<Model.Schedule>		());
			_students		= new ObservableCollection<Model.Student>		(new List<Model.Student>		());
			_subjects		= new ObservableCollection<Model.Subject>		(new List<Model.Subject>		());
			_teachers		= new ObservableCollection<Model.Teacher>		(new List<Model.Teacher>		());
			_teacherPhones	= new ObservableCollection<Model.TeacherPhone>	(new List<Model.TeacherPhone>	());
			LoadDataAsync();
		}

		private void LoadData()
		{
			using(Model.SchoolScheduleEntities dataBase=new Model.SchoolScheduleEntities())
			{
				dataBase.Configuration.ProxyCreationEnabled = false;
				dataBase.Configuration.LazyLoadingEnabled = false;

				App.Current.Dispatcher.Invoke(() =>
				{
					_groups.Clear();
					_lessons.Clear();
					_schedules.Clear();
					_students.Clear();
					_subjects.Clear();
					_teachers.Clear();
					_teacherPhones.Clear();
				});

				foreach (var el in dataBase.Groups.ToList())
					App.Current.Dispatcher.Invoke(() => { _groups.Add(el); });
				foreach (var el in dataBase.Lessons.ToList())
					App.Current.Dispatcher.Invoke(() => { _lessons.Add(el); });
				foreach (var el in dataBase.Schedules.ToList())
					App.Current.Dispatcher.Invoke(() => { _schedules.Add(el); });
				foreach(var el in dataBase.Students.ToList())
					App.Current.Dispatcher.Invoke(() => { _students.Add(el); });
				foreach (var el in dataBase.Subjects.ToList())
					App.Current.Dispatcher.Invoke(() => { _subjects.Add(el); });
				foreach(var el in dataBase.Teachers.ToList())
					App.Current.Dispatcher.Invoke(() => { _teachers.Add(el); });
				foreach (var el in dataBase.TeacherPhones.ToList())
					App.Current.Dispatcher.Invoke(() => { _teacherPhones.Add(el); });
			}
		}
		public async void LoadDataAsync()
		{
			await Task.Run(() =>
			{
				LoadData();
				App.Current.Dispatcher.Invoke(() =>
				{
					Groups=_groups;
					Lessons = _lessons;
					Schedules = _schedules;
					Students = _students;
					Teachers = _teachers;
					TeacherPhones = _teacherPhones;
				});
			});
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
