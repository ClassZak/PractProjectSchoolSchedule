using SchoolSchedule.Model.DTO;
using SchoolSchedule.View;
using SchoolSchedule.ViewModel.Comands;
using SchoolSchedule.ViewModel.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SchoolSchedule.ViewModel
{
	public class MainViewModel : ABaseViewModel
	{
		public MainWindow MainWindow { get; set; }
		public ObservableCollection<object> SelectedSubjects { get; set; } = new ObservableCollection<object>();
		public MainViewModel()
		{
			_groups			= new List<Model.Group>			(new List<Model.Group>			());
			_lessons		= new List<Model.Lesson>		(new List<Model.Lesson>			());
			_schedules		= new List<Model.Schedule>		(new List<Model.Schedule>		());
			_students		= new List<Model.Student>		(new List<Model.Student>		());
			_subjects		= new List<Model.Subject>		(new List<Model.Subject>		());
			_teachers		= new List<Model.Teacher>		(new List<Model.Teacher>		());
			_teacherPhones	= new List<Model.TeacherPhone>	(new List<Model.TeacherPhone>	());
			LoadData();
		}

		private void LoadData() 
		{
			try
			{
				using(var dataBase=new SchoolSchedule.Model.SchoolScheduleEntities())
				{

					App.Current.Dispatcher.Invoke(() =>
					{
						_groups.Clear();
						_lessons.Clear();
						_schedules.Clear();
						_students.Clear();
						_subjects.Clear();
						_teachers.Clear();
						_teacherPhones.Clear();


						_studentTable.Clear();
						_groupTable.Clear();
						_subjectTable.Clear();
					});

					foreach (var el in dataBase.Group.ToList())
						App.Current.Dispatcher.Invoke(() => { _groups.Add(el); });
					foreach (var el in dataBase.Lesson.ToList())
						App.Current.Dispatcher.Invoke(() => { _lessons.Add(el); });
					foreach (var el in dataBase.Schedule.ToList())
						App.Current.Dispatcher.Invoke(() => { _schedules.Add(el); });
					foreach (var el in dataBase.Student.ToList())
						App.Current.Dispatcher.Invoke(() => { _students.Add(el); });
					foreach (var el in dataBase.Subject.ToList())
						App.Current.Dispatcher.Invoke(() => { _subjects.Add(el); });
					foreach (var el in dataBase.Teacher.ToList())
						App.Current.Dispatcher.Invoke(() => { _teachers.Add(el); });
					foreach (var el in dataBase.TeacherPhone.ToList())
						App.Current.Dispatcher.Invoke(() => { _teacherPhones.Add(el); });

					foreach (var el in _students)
						App.Current.Dispatcher.Invoke(() => {_studentTable.Entries.Add(new DTOStudent(el));});
					foreach (var el in _groups)
						App.Current.Dispatcher.Invoke(() => {_groupTable.Entries.Add(new DTOGroup(el));});
					foreach (var el in _subjects)
						App.Current.Dispatcher.Invoke(() => {_subjectTable.Entries.Add(new DTOSubject(el));});

					OnPropertyChanged(nameof(DTOGroup));
					OnPropertyChanged(nameof(DTOStudents));
					OnPropertyChanged(nameof(DTOSubject));
				}
			}
			// Для того, чтобы не было ошибки в xaml
			catch (System.InvalidOperationException)
			{ }
			catch (System.Data.EntityException ex)
			{
				Task.Run(() =>
				{ 
					MessageBox.Show
					(
						ex.InnerException != null ? ex.InnerException.Message : ex.Message, 
						"Ошибка базы данных",
						MessageBoxButton.OK,
						MessageBoxImage.Stop
					);
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message,"Ошибка базы данных",MessageBoxButton.OK,MessageBoxImage.Stop);
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
			try
			{
				using (var dataBase = new Model.SchoolScheduleEntities())
				{
					List<T> elements = dataBase.Set<T>().ToList();

					App.Current.Dispatcher.Invoke(() => { collection.Clear(); });
					foreach (var el in elements)
						App.Current.Dispatcher.Invoke(() => { collection.Add(el); });
				}
			}
			catch(Exception ex)
			{	
				MessageBox.Show(ex.Message,"Ошибка базы данных",MessageBoxButton.OK,MessageBoxImage.Stop);
			}
		}

		protected FieldInfo FindTargetField(Type targetType)
		{
			FieldInfo field = null;
			foreach(var type in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
				if(IsObservableCollection(type.FieldType,out Type elementType) && elementType==targetType)
				{
					field = type;
					break;
				}

			return field;
		}

		#region Команды
		#region Команда обновления
		// Тип аргумента: Type
		RelayCommand _updateData;
		public RelayCommand UpdateDataAsync
		{
			get
			{
				return _updateData ??
				(_updateData = new RelayCommand
				(
					param =>
					{
						if (!(param is Type targetType))
						{
							LoadDataAsync();
							return;
						}

						Task.Run(() =>
						{
							try
							{
								using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
								{
									if (targetType.Name == typeof(Model.Group).Name)
									{
										_groups.Clear();
										App.Current.Dispatcher.Invoke(() => {_groupTable.Clear();});
										foreach (var el in dataBase.Group.ToList())
											App.Current.Dispatcher.Invoke(() => { _groups.Add(el); });
										foreach (var el in dataBase.Group.ToList())
											App.Current.Dispatcher.Invoke(() => { _groupTable.Entries.Add(new DTOGroup(el)); });
									}
									if (targetType.Name == typeof(Model.Student).Name)
									{
										_updateData.Execute(typeof(Model.Group));
										_students.Clear();
										foreach(var el in dataBase.Student.ToList())
											App.Current.Dispatcher.Invoke(() => {_students.Add(el); });

										_studentTable.Clear();
										foreach (var el in _students)
											App.Current.Dispatcher.Invoke(() => {
												_studentTable.Entries.Add(new DTOStudent(el));
											});
									}
									if(targetType.Name==typeof(Model.Subject).Name)
									{
										_subjects.Clear();
										App.Current.Dispatcher.Invoke(() => {_subjectTable.Clear();});
										foreach (var el in dataBase.Subject.ToList())
											App.Current.Dispatcher.Invoke(() => { _subjects.Add(el); });
										foreach (var el in dataBase.Subject.ToList())
											App.Current.Dispatcher.Invoke(() => { _subjectTable.Entries.Add(new DTOSubject(el)); });
									}
								}
							}
							catch (System.Data.EntityException ex)
							{
								Task.Run(() =>
								{
									MessageBox.Show
									(
										ex.InnerException != null ? ex.InnerException.Message : ex.Message,
										"Ошибка базы данных",
										MessageBoxButton.OK,
										MessageBoxImage.Stop
									);
								});
							}
							catch (Exception ex)
							{
								MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
							}
						});
					}
				));
			}
		}
		#endregion
		#region Команда удаления
		// Тип аргумента: Type
		private RelayCommand _deleteCommand;
		public RelayCommand DeleteCommand
		{
			get
			{
				return _deleteCommand ??
				(_deleteCommand = new RelayCommand
				(
					param =>
					{
						DataGrid dataGridRef = param as DataGrid;
						if (dataGridRef == null)
							throw new ArgumentException("Неверный аргумент для обновления таблицы с удаляемыми значениями");
						IEnumerable collectionRef = dataGridRef.ItemsSource;

						Task.Run(() =>
						{
							List<object> selectedItems = new List<object>();
							App.Current.Dispatcher.Invoke(() => 
							{
								if(dataGridRef.SelectedItems!=null)
								foreach(var el in dataGridRef.SelectedItems)
									selectedItems.Add(el);
							});
						});
					}
				));
			}
		}
		#endregion
		#region Команда добавления
		// Тип аргумента: Type
		private RelayCommand _insert;
		public RelayCommand Insert
		{
			get
			{
				return _insert ??
				(_insert = new RelayCommand(
					param =>
					{
						Task.Run(() =>
						{
							try
							{
								if (!(param is Type targetType))
									throw new ArgumentException("Выбран неверный тип аргумента");

								if (targetType.Name == typeof(Model.Group).Name)
								{
									GroupAddWindow addingWindow = null;

									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new GroupAddWindow(_groupTable.Entries);
										addingWindow.Owner=MainWindow;
										addingWindow.ShowDialog();
									});


									if (addingWindow.dialogResult)
									{
										using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
										{
											dataBase.Group.Add(addingWindow.NewGroup);
											dataBase.SaveChanges();
										}
										_updateData.Execute(typeof(Model.Group));
									}
								}
								if(targetType.Name== typeof(Model.Subject).Name)
								{
									SubjectAddWindow addingWindow = null;

									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new SubjectAddWindow(_subjectTable.Entries);
										addingWindow.Owner=MainWindow;
										addingWindow.ShowDialog();
									});

									if(addingWindow.dialogResult)
									{
										using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
										{
											dataBase.Subject.Add(addingWindow.NewSubject);
											dataBase.SaveChanges();
										}
										_updateData.Execute(typeof(Model.Subject));
									}
								}
							}
							// Для того, чтобы не было ошибки в xaml
							catch (System.InvalidOperationException)
							{
								_updateData.Execute(null);
							}
							catch (System.Data.EntityException ex)
							{
								MessageBox.Show
								(
									ex.InnerException != null ? ex.InnerException.Message : ex.Message,
									"Ошибка базы данных",
									MessageBoxButton.OK,
									MessageBoxImage.Stop
								);
								_updateData.Execute(null);
							}
							catch (Exception ex)
							{
								MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
								_updateData.Execute(null);
							}
							finally
							{
								
							}
						});
					}
				));
			}
		}
		#endregion
		#endregion
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


		private List<Model.Group> _groups;
		public List<Model.Group> Groups
		{ get { return _groups; } set { SetPropertyChanged(ref _groups, value); } }

		private List<Model.Lesson> _lessons;
		public List<Model.Lesson> Lessons
		{ get { return _lessons; } set { SetPropertyChanged(ref _lessons, value); } }

		private List<Model.Schedule> _schedules;
		public List<Model.Schedule> Schedules
		{ get { return _schedules; } set { SetPropertyChanged(ref _schedules, value); } }

		private List<Model.Student> _students;
		public List<Model.Student> Students
		{ get { return _students; } set { SetPropertyChanged(ref _students, value); } }

		private List<Model.Subject> _subjects;
		public List<Model.Subject> Subjects
		{ get { return _subjects; } set { SetPropertyChanged(ref _subjects, value); } }

		private List<Model.Teacher> _teachers;
		public List<Model.Teacher> Teachers
		{ get { return _teachers; } set { SetPropertyChanged(ref _teachers, value); } }

		private List<Model.TeacherPhone> _teacherPhones;
		public List<Model.TeacherPhone> TeacherPhones
		{ get { return _teacherPhones; } set { SetPropertyChanged(ref _teacherPhones, value); } }




		private StudentTable _studentTable=new StudentTable();
		public ObservableCollection<DTOStudent> DTOStudents
		{get { return _studentTable.Entries; } set { OnPropertyChanged(nameof(DTOStudents)); _studentTable.Entries=value; }}

		private GroupTable _groupTable=new GroupTable();
		public ObservableCollection<DTOGroup> DTOGroup
		{get { return _groupTable.Entries; } set { OnPropertyChanged(nameof(DTOGroup)); _groupTable.Entries=value; }}

		private SubjectTable _subjectTable=new SubjectTable();
		public ObservableCollection<DTOSubject> DTOSubject
		{get { return _subjectTable.Entries; } set { OnPropertyChanged(nameof(DTOSubject)); _subjectTable.Entries=value; }}
 	}
}
