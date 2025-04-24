using SchoolSchedule.Model;
using SchoolSchedule.Model.DTO;
using SchoolSchedule.View;
using SchoolSchedule.View.Edit;
using SchoolSchedule.ViewModel.Attributes;
using SchoolSchedule.ViewModel.Comands;
using SchoolSchedule.ViewModel.Table;
using SchoolSchedule.ViewModel.TaskModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SchoolSchedule.ViewModel
{
	public class MainViewModel : ABaseViewModel
	{
		public MainWindow MainWindow { get; set; }
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOSubject> SelectedSubjects { get; set; } = new ObservableCollection<Model.DTO.DTOSubject>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOGroup> SelectedGroups { get; set; } = new ObservableCollection<Model.DTO.DTOGroup>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOStudent> SelectedStudents { get; set; } = new ObservableCollection<Model.DTO.DTOStudent>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOTeacher> SelectedTeachers { get; set; } = new ObservableCollection<Model.DTO.DTOTeacher>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Object> SelectedTheacherPhones { get; set; } = new ObservableCollection<Object>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOLesson> SelectedLessons { get; set; } = new ObservableCollection<Model.DTO.DTOLesson>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOSchedule> SelectedSchedules { get; set; } = new ObservableCollection<Model.DTO.DTOSchedule>();
		public MainViewModel()
		{
			_groups = new List<Model.Group>(new List<Model.Group>());
			_lessons = new List<Model.Lesson>(new List<Model.Lesson>());
			_schedules = new List<Model.Schedule>(new List<Model.Schedule>());
			_students = new List<Model.Student>(new List<Model.Student>());
			_subjects = new List<Model.Subject>(new List<Model.Subject>());
			_teachers = new List<Model.Teacher>(new List<Model.Teacher>());
			_teacherPhones = new List<Model.TeacherPhone>(new List<Model.TeacherPhone>());

#if DEBUG
			LoadData();
#else
			LoadDataAsync();
#endif
		}

		#region Управление задачами
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			private set
			{
				_isBusy = value;
				OnPropertyChanged();
				CommandManager.InvalidateRequerySuggested();
			}
		}

		TaskViewModel _taskViewModel = new TaskViewModel();
		public string TaskName
		{
			get => _taskViewModel.TaskName;
			set
			{
				if (_taskViewModel.TaskName != value)
				{
					_taskViewModel.TaskName = value;
					OnPropertyChanged();
				}
			}
		}

		public string TaskStatus
		{
			get => _taskViewModel.TaskStatus;
			 set
			{
				if (_taskViewModel.TaskStatus != value)
				{
					_taskViewModel.TaskStatus = value;
					OnPropertyChanged();
				}
			}
		}
		public ETaskStatus ETaskStatus
		{
			get => _taskViewModel.ETaskStatus;
			 set
			{
				if (_taskViewModel.ETaskStatus != value)
				{
					_taskViewModel.ETaskStatus = value;
					OnPropertyChanged();
				}
			}
		}
		public string ErrorMessage
		{
			get => _taskViewModel.ErrorMessage;
			set
			{
				if (_taskViewModel.ErrorMessage != value)
				{
					_taskViewModel.ErrorMessage = value;
					OnPropertyChanged();
				}
			}
		}
#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
		private async Task EnsureConnectionOpenAsync(DbContext db)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
		{
			if (db.Database.Connection.State != ConnectionState.Open)
			{
				db.Database.Connection.Open();
			}
			// При неудаче здесь бросится SqlException, которую уже поймает ExecuteCommandAsync
		}
		#region Запуск команд
		private async Task ExecuteCommandAsync(string commandName, Func<Task> commandLogic)
		{
			if (!await _semaphore.WaitAsync(0))
			{
				ShowBusyMessage();
				return;
			}

			try
			{
				TaskName = commandName;
				ETaskStatus = ETaskStatus.InProgress;
				ErrorMessage = null;

				await commandLogic();

				ETaskStatus = ETaskStatus.Completed;
			}
			catch (Exception ex)
			{
				ETaskStatus = ETaskStatus.Failed;
				ErrorMessage = ex.Message;    // отобразим в UI
				HandleException(ex);          // ваше всплывающее MessageBox
			}
			finally
			{
				_semaphore.Release();
			}
		}
		#region CRUD функции
		#region Создание
		private async Task CreateEntityAsync(object entity, Type targetType)
		{
			using(var db=new SchoolScheduleEntities())
			{
				await EnsureConnectionOpenAsync(db);
				switch (entity)
				{
					case Model.Group group:
						db.Group.Add(group);
						break;
					case Model.Subject subject:
						db.Subject.Add(subject);
						break;
					case Model.Student student:
						db.Student.Add(student);
						break;
					case Model.Teacher teacher:
						List<Model.TeacherPhone> phones = new List<Model.TeacherPhone>(teacher.TeacherPhone);
						List<Model.Group> groups = new List<Model.Group>(teacher.Group);
						List<Model.Subject> subjects = new List<Model.Subject>(teacher.Subject);

						teacher.Subject.Clear();
						teacher.Group.Clear();
						teacher.TeacherPhone.Clear();


						var forEdit = db.Teacher.Add(teacher);
						await db.SaveChangesAsync();

						if (forEdit == null)
							throw new Exception("Не удалось добавить предметы и классы для учителей");
						foreach (var el in subjects)
							forEdit.Subject.Add(await db.Subject.FindAsync(el.Id));
						foreach (var el in groups)
							forEdit.Group.Add(await db.Group.FindAsync(el.Id));
						foreach (var el in phones)
							db.TeacherPhone.Add(new Model.TeacherPhone { IdTeacher = el.IdTeacher, PhoneNumber = el.PhoneNumber });

						break;
					case Model.Lesson lesson:
						db.Lesson.Add(lesson);
						break;
					case Model.Schedule schedule:
						db.Schedule.Add(schedule);
						break;
				}
				await db.SaveChangesAsync();
			}
		}
		#endregion
		#region Изменение
		public async Task UpdateEntityAsync(object entity, object selectedObject, Type targetType)
		{
			using (var db = new SchoolSchedule.Model.SchoolScheduleEntities())
			{
				await EnsureConnectionOpenAsync(db);
				switch (entity)
				{
					case Model.Group group:
						{
							var selectedDTO = selectedObject as Model.DTO.DTOGroup;


							var forUpdate = await db.Group.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id);

							if (forUpdate == null)
								throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
							forUpdate.Name = group.Name;
							forUpdate.Year = group.Year;
							break;
						}
					case Model.Subject subject:
						{
							var selectedDTO = selectedObject as Model.DTO.DTOSubject;


							var forUpdate = await db.Subject.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id);
							if (forUpdate == null)
								throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
							forUpdate.Name = subject.Name;
							break;
						}
					case Model.Student student:
						{
							var selectedDTO = selectedObject as Model.DTO.DTOStudent;


							var forUpdate = await db.Student.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id);
							if (forUpdate == null)
								throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
							forUpdate.Surname = student.Surname;
							forUpdate.Name = student.Name;
							forUpdate.Patronymic = student.Patronymic;
							forUpdate.IdGroup = student.IdGroup;
							forUpdate.Email = student.Email;
							break;
						}
					case Model.Teacher teacher:
						{
							var selectedDTO = selectedObject as Model.DTO.DTOTeacher;


							List<Model.TeacherPhone> phones = new List<Model.TeacherPhone>(teacher.TeacherPhone);
							List<Model.Group> groups = new List<Model.Group>(teacher.Group);
							List<Model.Subject> subjects = new List<Model.Subject>(teacher.Subject);

							teacher.Subject.Clear();
							teacher.Group.Clear();
							teacher.TeacherPhone.Clear();

							var forUpdate = await db.Teacher
								.Include(t => t.TeacherPhone) // Добавляем Include для загрузки связанных телефонов
								.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id);

							if (forUpdate == null)
								throw new Exception("Объект не найден");

							forUpdate.Surname = teacher.Surname;
							forUpdate.Name = teacher.Name;
							forUpdate.Patronymic = teacher.Patronymic;

							foreach (var phone in forUpdate.TeacherPhone.ToList())
							{
								if (phone.IdTeacher == forUpdate.Id)
									db.TeacherPhone.Remove(phone);
							}

							foreach (var el in phones)
							{
								db.TeacherPhone.Add(new Model.TeacherPhone
								{
									IdTeacher = forUpdate.Id,
									PhoneNumber = el.PhoneNumber
								});
							}

							// Обновляем Subject и Group (many-to-many)
							forUpdate.Subject.Clear();
							foreach (var el in subjects)
							{
								var subject = await db.Subject.FindAsync(el.Id);
								if (subject != null)
									forUpdate.Subject.Add(subject);
							}

							forUpdate.Group.Clear();
							foreach (var el in groups)
							{
								var group = await db.Group.FindAsync(el.Id);
								if (group != null)
									forUpdate.Group.Add(group);
							}
							break;
						}
					case Model.Lesson lesson:
						{
							var selectedDTO = selectedObject as Model.DTO.DTOLesson;


							var forUpdate = await db.Lesson.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id);
							if (forUpdate == null)
								throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
							forUpdate.IdSubject = selectedDTO.ModelRef.IdSubject;
							forUpdate.IdGroup = selectedDTO.ModelRef.IdGroup;
							forUpdate.Number = selectedDTO.ModelRef.Number;
							break;
						}
					case Model.Schedule schedule:
						{
							var selectedDTO = selectedObject as Model.DTO.DTOSchedule;


							var forUpdate = await db.Schedule.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id);
							if (forUpdate == null)
								throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
							forUpdate.IdLesson = selectedDTO.ModelRef.IdLesson;
							forUpdate.IdTeacher = selectedDTO.ModelRef.IdTeacher;
							forUpdate.StartTime = selectedDTO.ModelRef.StartTime;
							forUpdate.EndTime = selectedDTO.ModelRef.EndTime;
							forUpdate.Date = selectedDTO.ModelRef.Date;
							break;
						}
				}
				await db.SaveChangesAsync();
			}
		}
		#endregion
		#region Удаление
		public async Task DeleteEntitiesAsync(IList selectedObjects, Type targetType)
		{
			using (var db=new Model.SchoolScheduleEntities())
			{
				await EnsureConnectionOpenAsync(db);
				if (targetType == typeof(Model.DTO.DTOGroup))
				{
					var teachers = await db.Teacher.ToListAsync();
					var lessons = await db.Lesson.ToListAsync();
					var students = await db.Student.ToListAsync();
					foreach (var el in selectedObjects)
					{
						var selectedDTO = el as Model.DTO.DTOGroup;
						var teachersUses = FindTeachersUsesGroup(ref teachers, selectedDTO.ModelRef.Id);
						var lessonsUses = FindLessonsUsesGroup(ref lessons, selectedDTO.ModelRef.Id);
						var studentsUsees = FindStudentsUsesGroup(ref students, selectedDTO.ModelRef.Id);

						if (teachersUses.Count() != 0 || lessonsUses.Count() != 0 || studentsUsees.Count() != 0)
							throw new Exception($"Удалите записи всех уроков, учителей и всех студентов, ссылающихся на класс \"{selectedDTO.ModelRef}\"");

						var forDelete = await db.Group.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
						db.Group.Remove(forDelete);
					}
				}
				else if (targetType == typeof(Model.DTO.DTOSubject))
				{
					var teachers = await db.Teacher.ToListAsync();
					var lessons = await db.Lesson.ToListAsync();

					foreach (var el in selectedObjects)
					{
						var selectedDTO = el as Model.DTO.DTOSubject;
						var teachersUsesSubject = FindTeachersUsesSubject(ref teachers, selectedDTO.ModelRef.Id);
						var lessonsUsesSubject = FindLessonsUsesSubject(ref lessons, selectedDTO.ModelRef.Id);

						if (teachersUsesSubject.Count() != 0 || lessonsUsesSubject.Count() != 0)
							throw new Exception($"Удалите всех уроков и учителей, ссылающихся на предмет \"{selectedDTO.Name}\"");

						var forDelete = await db.Subject.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
						if (forDelete == null)
							throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
						db.Subject.Remove(forDelete);
					}
				}
				else if (targetType == typeof(Model.DTO.DTOSubject))
				{
					foreach (var el in selectedObjects)
					{
						var selectedDTO = el as Model.DTO.DTOStudent;
						var forDelete = await db.Student.FirstAsync() ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
						db.Student.Remove(forDelete);
					}
				}
				else if (targetType == typeof(Model.DTO.DTOTeacher))
				{
					var schedules = await db.Schedule.ToListAsync();
					var phones = await db.TeacherPhone.ToListAsync();
					foreach (var el in selectedObjects)
					{
						var selectedDTO = el as Model.DTO.DTOTeacher;
						var forDelete = await db.Teacher.FirstAsync(x => x.Id == selectedDTO.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
						var schedulesUsesTeacher = FindSchedulesUsesTeacher(ref schedules, forDelete.Id);
						var phonesUsesTeacher = FindTeacherPhonesUsesTeacher(ref phones, forDelete.Id);
						if (schedulesUsesTeacher.Any())
							throw new Exception($"Удалите все объекты расписания, в которых записан преподаватель {selectedDTO.ModelRef}");
						if (forDelete == null)
							throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");

						foreach (var p in phonesUsesTeacher)
							db.TeacherPhone.Remove(p);

						var teacherGroup = new List<Model.Group>(forDelete.Group);
						foreach (var group in teacherGroup)
							forDelete.Group.Remove(group);
						await db.SaveChangesAsync();
						var teacherSubject = new List<Model.Subject>(forDelete.Subject);
						foreach (var subject in teacherSubject)
							forDelete.Subject.Remove(subject);
						await db.SaveChangesAsync();

						db.Teacher.Remove(forDelete);
					}
				}
				else if (targetType == typeof(Model.DTO.DTOLesson))
				{
					var schedules = await db.Schedule.ToListAsync();
					foreach (var el in SelectedLessons)
					{
						var selectedDTO = el as Model.DTO.DTOLesson;
						var schedulesUseesLesson = FindSchedulesUsesLesson(ref schedules, el.Id);
						if (schedulesUseesLesson.Any())
							throw new Exception($"Удалите все объекты расписания, в которых проводится занятие по предмету \"{el.Subject}\" с классом \"{el.Group}\"");

						var forDelete = await db.Lesson.FirstAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
						db.Lesson.Remove(forDelete);
					}
				}
				else if (targetType == typeof(Model.DTO.DTOSchedule))
				{
					foreach (var el in selectedObjects)
					{
						var selectedDTO = el as Model.DTO.DTOSchedule;
						var forDelete = await db.Schedule.FirstAsync(x => x.Id == selectedDTO.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
						db.Schedule.Remove(forDelete);
					}
				}
				await db.SaveChangesAsync();
			}
		}
		#endregion
		#endregion
		#region CRUD команды
		#region Команда добавления
		private RelayCommand _createCommmand;
		public RelayCommand CreateCommand
		{
			get
			{
				return _createCommmand ?? (_createCommmand = new RelayCommand(
				async param => await ExecuteCommandAsync("Добавление новых записей",async () =>
				await Task.Run(async ()=>
				{
					if (!(param is Type targetType))
						throw new ArgumentException("Выбран неверный тип аргумента");

					var result = await ShowEditWindowAsync(targetType, null);
					if (result.DialogResult)
					{
						await CreateEntityAsync(result.EditObject, targetType);

						Thread thread = new Thread(() => { Thread.Sleep(200); _semaphore.Wait(); _semaphore.Release(); App.Current.Dispatcher.Invoke(() => { _updateCommand2.Execute(null); }); });
						thread.Start();
					}
				}))));
			}
		}
		#endregion
		#region Команда чтения
		private RelayCommand _updateCommand2;
		public RelayCommand UpdateCommand2
		{
			get
			{
				return _updateCommand2 ?? (_updateCommand2= new RelayCommand(
				async param => await ExecuteCommandAsync("Загрузка данных", async () =>
				{
					await LoadDataAsync();
				})));
			}
		}
		#endregion
		#region Команда редактирования
		private RelayCommand _editCommamd2;
		public RelayCommand EditCommad2
		{
			get
			{
				return _editCommamd2 ?? (_editCommamd2=new RelayCommand(
				async param => await ExecuteCommandAsync("Редактирование данных", async ()=>
				await Task.Run(async ()=>
				{

					if(param.GetType().GetGenericTypeDefinition()!=typeof(ObservableCollection<>))
						throw new ArgumentException("Выбран неверный тип аргумента");

					Type targetType = param.GetType().GetGenericArguments()[0];
					var targetCollection= FindTargetPropertyOfSelectedItems(targetType).GetValue(this) as IList;
					if (targetCollection.Count != 1)
					{
						MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
						return;
					}

					var selectedObject = (targetCollection)[0];
					var result=await ShowEditWindowAsync(targetType, selectedObject);
					if(result.DialogResult)
					{
						await UpdateEntityAsync(result.EditObject, selectedObject, targetType);
						Thread thread = new Thread(() => { Thread.Sleep(200); _semaphore.Wait(); _semaphore.Release(); App.Current.Dispatcher.Invoke(() => { _updateCommand2.Execute(null); }); });
						thread.Start();
					}
				}))));
			}
		}
		#endregion
		#region Команда удаления
		private RelayCommand _deleteCommand2;
		public RelayCommand DeleteCommand2
		{
			get
			{
				return _deleteCommand2 ?? (_deleteCommand2=new RelayCommand(
				async param => await ExecuteCommandAsync("Удаление объектов", async()=>
				await Task.Run(async ()=>
				{
					if (param.GetType().GetGenericTypeDefinition() != typeof(ObservableCollection<>))
						throw new ArgumentException("Выбран неверный тип аргумента");

					Type targetType = param.GetType().GetGenericArguments()[0];
					var targetCollection = FindTargetPropertyOfSelectedItems(targetType).GetValue(this) as IList;

					if(targetCollection.Count < 1)
					{
						MessageBox.Show("Выбрерте хотя бы 1 объект для удаления.", "Ошибка удаления данных", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}

					await DeleteEntitiesAsync(targetCollection, targetType);
					Thread thread = new Thread(() => { Thread.Sleep(200); _semaphore.Wait(); _semaphore.Release(); App.Current.Dispatcher.Invoke(() => { _updateCommand2.Execute(null); }); });
					thread.Start();
				}))));
			}
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#region Общие методы
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

		protected PropertyInfo FindTargetPropertyOfSelectedItems(Type targetType)
		{
			PropertyInfo property = null;
			foreach (var propertyInfo in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
				if (IsObservableCollection(propertyInfo.PropertyType, out Type elementType) && elementType == targetType)
				{
					if(propertyInfo.GetCustomAttributes().Contains(new CollectionOfSelectedItemsAttribute()))
					{
						property = propertyInfo;
						break;
					}
				}

			return property;
		}

		private void ShowBusyMessage()
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				MessageBox.Show
				(
					"Задача не может быть запущена. Дождитесь завершения текущей операции",
					"Операция заблокирована",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);
			});
		}
		protected object GetModelRefOfADTOObject(object dto, out Type extractedType)
		{
			extractedType = null;
			object sendingEntity= null;
			
			string typename = dto.GetType().Name;
			if (typename == typeof(DTOSubject).Name)
			{
				sendingEntity = (dto as DTOSubject).ModelRef;
				extractedType = typeof(Model.Subject);
			}
			else if (typename == typeof(DTOGroup).Name)
			{
				sendingEntity = (dto as DTOGroup).ModelRef;
				extractedType = typeof(Model.Group);
			}
			else if (typename == typeof(DTOLesson).Name)
			{
				sendingEntity = (dto as DTOLesson).ModelRef;
				extractedType = typeof(Model.Lesson);
			}
			else if (typename == typeof(DTOSchedule).Name)
			{
				sendingEntity = (dto as DTOSchedule).ModelRef;
				extractedType = typeof(Model.Schedule);
			}
			else if (typename == typeof(DTOStudent).Name)
			{
				sendingEntity = (dto as DTOStudent).ModelRef;
				extractedType = typeof(Model.Student);
			}
			else if (typename == typeof(DTOTeacher).Name)
			{
				sendingEntity = (dto as DTOTeacher).ModelRef;
				extractedType = typeof(Model.Teacher);
			}
			else if (typename == typeof(DTOTeacherPhone).Name)
			{
				sendingEntity = (dto as DTOTeacherPhone).ModelRef;
				extractedType = typeof(Model.TeacherPhone);
			}

			return sendingEntity;
		}
		private async Task<EditWindow> ShowEditWindowAsync(Type targetType, object entity)
		{
			EditWindow window = null;

			object sendingEntity = entity;
			if(entity != null)
			{
				sendingEntity = GetModelRefOfADTOObject(entity,out Type truthType);
				targetType = truthType;
			}

			await Application.Current.Dispatcher.InvokeAsync(() =>
			{
				window = new EditWindow(targetType, sendingEntity, _groups,_lessons,_schedules,_students,_subjects,_teachers,_teacherPhones,MainWindow);
				window.ShowDialog();
			});
			return window;
		}
		private void HandleException(Exception ex)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				if (ex is DbUpdateException)
				{
					// Распаковываем вложенные исключения
					Exception innerException = ex.InnerException;
					while (innerException.InnerException != null)
						innerException = innerException.InnerException;

					MessageBox.Show(innerException.InnerException != null ? innerException.InnerException.Message : innerException.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
				}
				else if (ex is SqlException)
				{
					SqlException sqlException = ex as SqlException;
					if (sqlException != null)
					{
						if (sqlException.Number == 2)
							MessageBox.Show("Ошибка: Файл базы данных не найден. Проверьте путь в строке подключения и подключение к серверу", "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						else
						if (sqlException.Number == 53 || sqlException.Number == -1)
							MessageBox.Show(ex.Message, "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						else
							MessageBox.Show($"SQL-ошибка (код {sqlException.Number}): {sqlException.Message}", "Ошибка базы данных");
					}
					ClearTables();
				}
				else
				{
					MessageBox.Show(ex.InnerException != null ? ex.InnerException.Message : ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
					if (ex.Message == "The underlying provider failed on Open.")
						ClearTables();
				}
			});
		}
		#endregion
		private void ClearTables()
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
				_teacherTable.Clear();
				_lessonTable.Clear();
				_scheduleTable.Clear();
			});

		}
		private void LoadData()
		{
			try
			{
				using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
				{
					dataBase.Database.Connection.Open();

					ClearTables();

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
						App.Current.Dispatcher.Invoke(() => { _studentTable.Entries.Add(new DTOStudent(el)); });
					foreach (var el in _groups)
						App.Current.Dispatcher.Invoke(() => { _groupTable.Entries.Add(new DTOGroup(el)); });
					foreach (var el in _subjects)
						App.Current.Dispatcher.Invoke(() => { _subjectTable.Entries.Add(new DTOSubject(el)); });
					foreach (var el in _teachers)
						App.Current.Dispatcher.Invoke(() => { _teacherTable.Entries.Add(new DTOTeacher(el)); });
					foreach (var el in _lessons)
						App.Current.Dispatcher.Invoke(() => { _lessonTable.Entries.Add(new DTOLesson(el)); });
					foreach (var el in _schedules)
						App.Current.Dispatcher.Invoke(() => { _scheduleTable.Entries.Add(new DTOSchedule(el)); });

					OnPropertyChanged(nameof(DTOGroup));
					OnPropertyChanged(nameof(DTOStudents));
					OnPropertyChanged(nameof(DTOSubject));
					OnPropertyChanged(nameof(DTOTeacher));
					OnPropertyChanged(nameof(DTOLesson));
					OnPropertyChanged(nameof(DTOSchedule));
				}
			}
			// Для того, чтобы не было ошибки в xaml
			catch (System.InvalidOperationException)
			{ }
			catch (DbUpdateException ex)
			{
				// Распаковываем вложенные исключения
				Exception innerException = ex.InnerException;
				while (innerException.InnerException != null)
					innerException = innerException.InnerException;

				Task.Run(() =>
				{
					MessageBox.Show(innerException.InnerException != null ? innerException.InnerException.Message : innerException.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
				});
			}
			catch (System.Data.Entity.Core.EntityException ex)
			{
				string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				Task.Run(() =>{MessageBox.Show("Ошибка чтения записей из базы данных:\n"+message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);});
			}
			catch (SqlException ex) when (ex.Number == 2)
			{
				Task.Run(() =>{MessageBox.Show("Ошибка: Файл базы данных не найден. Проверьте путь в строке подключения и подключение к серверу", "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);});
			}
			catch (SqlException ex) when (ex.Number == 53 || ex.Number == -1) // Ошибки подключения
			{
				Task.Run(() =>{MessageBox.Show(ex.Message, "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);});
			}
			catch (SqlException ex)
			{
				Console.WriteLine($"SQL-ошибка (код {ex.Number}): {ex.Message}");
			}
			catch (Exception ex)
			{
				Task.Run(() =>{MessageBox.Show(ex.InnerException != null ? ex.InnerException.Message : ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);});
			}
		}

		
		// Использовать в конструкторе при релизе
		private async Task LoadDataAsync()
		{
			await Task.Run(() =>
			{
				LoadData();
			});
		}
		#region Поиск связанных объектов
		#region Для удаления предмета
		IEnumerable<Model.Teacher> FindTeachersUsesSubject(ref List<Model.Teacher> list, int id) 
		{
			return list.Where(t => t.Subject.ToList().Exists(s => s.Id == id)); 
		}
		IEnumerable<Model.Lesson> FindLessonsUsesSubject(ref List<Model.Lesson> list, int id) 
		{ 
			return list.Where(l => l.Subject.Id == id); 
		}
		#endregion
		#region Для удаления класса
		IEnumerable<Model.Teacher> FindTeachersUsesGroup(ref List<Model.Teacher> list, int id) 
		{
			return list.Where(t => t.Group.ToList().Exists(g => g.Id == id)); 
		}
		IEnumerable<Model.Lesson> FindLessonsUsesGroup(ref List<Model.Lesson> list, int id) 
		{
			return list.Where(l => l.IdGroup==id); 
		}
		IEnumerable<Model.Student> FindStudentsUsesGroup(ref List<Model.Student> list, int id) 
		{
			return list.Where(s => s.IdGroup==id);
		}
		#endregion
		#region Для удаления урока
		IEnumerable<Model.Schedule> FindSchedulesUsesLesson(ref List<Model.Schedule> list, int id)
		{
			return list.Where(s => s.IdLesson == id);
		}
		#endregion
		#region Для удаления учителя
		IEnumerable<Model.Schedule> FindSchedulesUsesTeacher(ref List<Model.Schedule> list, int id)
		{
			return list.Where(s => s.IdTeacher == id);
		}
		IEnumerable<Model.TeacherPhone> FindTeacherPhonesUsesTeacher(ref List<Model.TeacherPhone> list, int id)
		{
			return list.Where(p => p.IdTeacher == id);
		}
		#endregion
		#endregion
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




		private TemplateTable<DTOStudent> _studentTable=new TemplateTable<DTOStudent>();
		public ObservableCollection<DTOStudent> DTOStudents
		{get { return _studentTable.Entries; } set { OnPropertyChanged(nameof(DTOStudents)); _studentTable.Entries=value; }}

		private TemplateTable<DTOGroup> _groupTable =new TemplateTable<DTOGroup>();
		public ObservableCollection<DTOGroup> DTOGroup
		{get { return _groupTable.Entries; } set { OnPropertyChanged(nameof(DTOGroup)); _groupTable.Entries=value; }}

		private TemplateTable<DTOSubject> _subjectTable =new TemplateTable<DTOSubject>();
		public ObservableCollection<DTOSubject> DTOSubject
		{get { return _subjectTable.Entries; } set { OnPropertyChanged(nameof(DTOSubject)); _subjectTable.Entries=value; }}

		private TemplateTable<DTOTeacher> _teacherTable =new TemplateTable<DTOTeacher>();
		public ObservableCollection<DTOTeacher> DTOTeacher
		{get { return _teacherTable.Entries; } set { OnPropertyChanged(nameof(DTOTeacher)); _teacherTable.Entries=value; }}

		private TemplateTable<DTOLesson> _lessonTable =new TemplateTable<DTOLesson>();
		public ObservableCollection<DTOLesson> DTOLesson
		{get { return _lessonTable.Entries; } set { OnPropertyChanged(nameof(DTOLesson)); _lessonTable.Entries=value; }}

		private TemplateTable<DTOSchedule> _scheduleTable =new TemplateTable<DTOSchedule>();
		public ObservableCollection<DTOSchedule> DTOSchedule
		{ get { return _scheduleTable.Entries; } set { OnPropertyChanged(nameof(DTOSchedule)); _scheduleTable.Entries = value; } }

	}
}
