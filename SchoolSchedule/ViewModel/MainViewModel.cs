using SchoolSchedule.ViewModel.Comands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
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
			LoadData();
		}

		private void LoadData()
		{
			using(var dataBase=new Model.SchoolScheduleEntities())
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
		private async void LoadDataAsync()
		{
			await Task.Run(() =>
			{
				LoadData();
			});
		}

		protected void UpdateList<T>(ObservableCollection<T> collection) where T : class
		{
			using (var dataBase = new Model.SchoolScheduleEntities())
			{
				List<T> elements = dataBase.Set<T>().ToList();

				App.Current.Dispatcher.Invoke(() => { collection.Clear(); });
				foreach (var el in elements)
					App.Current.Dispatcher.Invoke(() => { collection.Add(el); });
			}
		}

		// Тип аргумента: Type
		RelayCommand _updateData;
		public RelayCommand UpdateDataAsync
		{
			get
			{
				return _updateData ?? (_updateData = new RelayCommand(
					param =>
					{
						if (!(param is Type targetType))
						{
							LoadDataAsync();
							return;
						}

						Task.Run(() =>
						{
							FieldInfo targetField = null;
							var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

							foreach (var field in fields)
							{
								if
								(
									IsObservableCollection(field.FieldType, out Type elementType)&& 
									elementType == targetType
								)
								{
									targetField = field;
									break;
								}
							}

							if (targetField == null)
								throw new ArgumentException("Коллекция для указанного типа не найдена.");

							var collection = (IList)targetField.GetValue(this);
							var updateMethod = GetType().GetMethod(nameof(UpdateList), BindingFlags.NonPublic | BindingFlags.Instance);
							var genericMethod = updateMethod.MakeGenericMethod(targetType);
							genericMethod.Invoke(this, new object[] { collection });
						});
					}));
			}
		}
		/// <summary>
		/// Проверка на принадлежность шаблону ObservableCollection<T>, и возвращает T тип из шаблона
		/// </summary>
		/// <param name="type"></param>
		/// <param name="elementType"></param>
		/// <returns></returns>
		protected bool IsObservableCollection(Type type, out Type elementType)
		{
			elementType = null;

			if (type.IsGenericType &&
				type.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
			{
				elementType = type.GetGenericArguments()[0];
				return true;
			}

			return false;
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
