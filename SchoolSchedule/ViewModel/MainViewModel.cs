using SchoolSchedule.Model;
using SchoolSchedule.Model.DTO;
using SchoolSchedule.View;
using SchoolSchedule.View.Edit;
using SchoolSchedule.ViewModel.Attributes;
using SchoolSchedule.ViewModel.Commands;
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
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SchoolSchedule.ViewModel
{
	public partial class MainViewModel : ABaseViewModel
	{
		#region Окна
		AboutWindow AboutWindow { get; set; } = null;
		#region Команды запуска окон
		RelayCommand _showAboutWindow;
		public RelayCommand ShowAboutWindow
		{
			get
			{
				return _showAboutWindow ?? (_showAboutWindow=new RelayCommand(param=> 
				{
					if (AboutWindow != null)
						AboutWindow.Focus();
					else
					{
						AboutWindow = new AboutWindow();
						AboutWindow.Owner = MainWindow;
						AboutWindow.Closed += WindowClosedEventHandeler;
						AboutWindow.Show();
					}
				}));
			}
		}
		#endregion
		#region Обработчики событий закрытия окна
		void WindowClosedEventHandeler(object sender, EventArgs eventArgs)
		{
			if (ReferenceEquals(sender, AboutWindow))
				AboutWindow = null;
		}
		#endregion
		#endregion
		#region Копирование в буфер обмена
		private List<string> GetDisplayedProperties(DataGrid dataGrid)
		{
			return dataGrid.Columns
				.OfType<DataGridBoundColumn>()
				.Select(column => (column.Binding as Binding)?.Path.Path)
				.Where(path => !string.IsNullOrEmpty(path))
				.ToList();
		}
		private object GetPropertyValue(object source, string propertyPath)
		{
			if (source == null || string.IsNullOrEmpty(propertyPath)) return null;

			var parts = propertyPath.Split('.');
			object current = source;

			foreach (var part in parts)
			{
				var type = current.GetType();
				var prop = type.GetProperty(part);

				if (prop == null) return null;
				current = prop.GetValue(current);

				if (current == null) break;
			}

			return current;
		}
		private List<Dictionary<string, object>> GetSelectedItemsData(DataGrid dataGrid)
		{
			var result = new List<Dictionary<string, object>>();
			var properties = GetDisplayedProperties(dataGrid);

			foreach (var item in dataGrid.SelectedItems)
			{
				var itemData = new Dictionary<string, object>();
				foreach (var propertyPath in properties)
				{
					itemData[propertyPath] = GetPropertyValue(item, propertyPath);
				}
				result.Add(itemData);
			}

			return result;
		}
		RelayCommand _dataGridDataToClipboard;
		public RelayCommand DataGridDataToClipboard
		{
			get
			{
				return _dataGridDataToClipboard ?? (_dataGridDataToClipboard = new RelayCommand(param => 
				{
					if (param is null)
						throw new ArgumentNullException(nameof(param));
					if (!(param is DataGrid dataGrid))
						throw new ArgumentException("Параметр должен быть DataGrid", nameof(param));

					// Получаем видимые элементы в правильном порядке
					var orderedSelectedItems = dataGrid.Items.Cast<object>()
						.Where(item => dataGrid.ItemContainerGenerator.ContainerFromItem(item) != null)
						.ToList()
						.Where(item => dataGrid.SelectedItems.Contains(item))
						.ToList();

					var columns = dataGrid.Columns
						.OfType<DataGridBoundColumn>()
						.Select(c => (c.Binding as Binding)?.Path.Path)
						.Where(p => !string.IsNullOrEmpty(p))
						.ToList();

					var stringBuilder = new StringBuilder();
					foreach (var item in orderedSelectedItems)
					{
						var values = new List<string>();
						foreach (var propertyPath in columns)
						{
							object value = item;
							foreach (var part in propertyPath.Split('.'))
							{
								var prop = value?.GetType().GetProperty(part);
								value = prop?.GetValue(value);
								if (value == null) break;
							}
							values.Add(value?.ToString() ?? string.Empty);
						}
						stringBuilder.AppendLine(string.Join("\t", values));
					}

					if (stringBuilder.Length > 0)
						Clipboard.SetText(stringBuilder.ToString().TrimEnd());
				}));
			}
		}
		#endregion

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
		public ObservableCollection<Model.DTO.DTOSchedule> SelectedSchedules { get; set; } = new ObservableCollection<Model.DTO.DTOSchedule>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOBellScheduleType> SelectedBellScheduleTypes { get; set; } = new ObservableCollection<Model.DTO.DTOBellScheduleType>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOBellSchedule> SelectedBellSchedules { get; set; } = new ObservableCollection<Model.DTO.DTOBellSchedule>();
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public ObservableCollection<Model.DTO.DTOLessonSubsitutionSchedule> SelectedLessonSubsitutionSchedules { get; set; } = new ObservableCollection<Model.DTO.DTOLessonSubsitutionSchedule>();
		public MainViewModel()
		{
#if DEBUG
			LoadData();
#else
			UpdateCommand.Execute(null);
#endif
		}

		#region Управление задачами
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
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
				SaveTableValues();

				TaskName = commandName;
				ETaskStatus = ETaskStatus.InProgress;
				OnPropertyChanged(nameof(TaskStatus));
				ErrorMessage = null;

				await commandLogic();

				ETaskStatus = ETaskStatus.Completed;
				OnPropertyChanged(nameof(TaskStatus));
			}
			catch (Exception ex)
			{
				ETaskStatus = ETaskStatus.Failed;
				OnPropertyChanged(nameof(TaskStatus));
				ErrorMessage = GetMessageFromException(ex);
				HandleException(ex);

				CancelTableChanges();
			}
			finally
			{
				_semaphore.Release();
			}
		}
		#endregion
		#region CRUD команды
		#region Команда добавления
		private RelayCommand _createCommmand;
		public RelayCommand CreateCommand
		{
			get
			{
				return _createCommmand ?? (_createCommmand = new RelayCommand(
				async param =>
				{
					if (!(param is Type targetType))
						throw new ArgumentException("Выбран неверный тип аргумента");

					var result = await ShowEditWindowAsync(targetType, null);
					if(result.DialogResult && MessageBox.Show("Вы действительно хотите добавить новый объект?","Добаваление объектов",MessageBoxButton.YesNoCancel,MessageBoxImage.Question)==MessageBoxResult.Yes)
						await ExecuteCommandAsync("Добавление новых записей", async () => await Task.Run(async () =>
						{
							await CreateEntityAsync(result.EditObject, targetType);

							Thread thread = new Thread(() => { Thread.Sleep(200); _semaphore.Wait(); _semaphore.Release(); App.Current.Dispatcher.Invoke(() => { _updateCommand.Execute(null); }); });
							thread.Start();
						}));
				}));
			}
		}
		#endregion
		#region Команда чтения
		private RelayCommand _updateCommand;
		public RelayCommand UpdateCommand
		{
			get
			{
				return _updateCommand ?? (_updateCommand= new RelayCommand(
				async param => await ExecuteCommandAsync("Загрузка данных", async () =>
				{
					await LoadDataAsync();
				})));
			}
		}
		#endregion
		#region Команда редактирования
		private RelayCommand _editCommamd2;
		public RelayCommand EditCommad
		{
			get
			{
				return _editCommamd2 ?? (_editCommamd2 = new RelayCommand(
				async param =>
				{
					if (param.GetType().GetGenericTypeDefinition() != typeof(ObservableCollection<>))
						throw new ArgumentException("Выбран неверный тип аргумента");
					Type targetType = param.GetType().GetGenericArguments()[0];
					var targetCollection = FindTargetPropertyOfSelectedItems(targetType).GetValue(this) as IList;
					if (targetCollection.Count != 1)
					{
						MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
						return;
					}
					var selectedObject = (targetCollection)[0];
					var result = await ShowEditWindowAsync(targetType, selectedObject);

					if (result.DialogResult && MessageBox.Show("Вы действительно хотите сохранить изменения?", "Редактирование данных", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
						await ExecuteCommandAsync("Редактирование данных", async () => await Task.Run(async () =>
						{
							await UpdateEntityAsync(result.EditObject, selectedObject, targetType);
							Thread thread = new Thread(() => { Thread.Sleep(200); _semaphore.Wait(); _semaphore.Release(); App.Current.Dispatcher.Invoke(() => { _updateCommand.Execute(null); }); });
							thread.Start();
						})); 
				}));
			}
		}
		#endregion
		#region Команда удаления
		private RelayCommand _DeleteCommand;
		public RelayCommand DeleteCommand
		{
			get
			{
				return _DeleteCommand ?? (_DeleteCommand = new RelayCommand(
				async param => 
				{
					if (param.GetType().GetGenericTypeDefinition() != typeof(ObservableCollection<>))
						throw new ArgumentException("Выбран неверный тип аргумента");

					Type targetType = param.GetType().GetGenericArguments()[0];
					var targetCollection = FindTargetPropertyOfSelectedItems(targetType).GetValue(this) as IList;

					if (targetCollection.Count < 1)
					{
						MessageBox.Show("Выбрерте хотя бы 1 объект для удаления.", "Ошибка удаления данных", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					if(MessageBox.Show("Вы действительно хотите удалить выбранные объекты?", "Удаление объектов", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
						await ExecuteCommandAsync("Удаление объектов", async () => await Task.Run(async () =>
						{
							await DeleteEntitiesAsync(targetCollection, targetType);
							Thread thread = new Thread(() => { Thread.Sleep(200); _semaphore.Wait(); _semaphore.Release(); App.Current.Dispatcher.Invoke(() => { _updateCommand.Execute(null); }); });
							thread.Start();
						})); 
				}));
			}
		}
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
			else if(typename==typeof(DTOBellScheduleType).Name)
			{
				sendingEntity = (dto as DTOBellScheduleType).ModelRef;
				extractedType = (typeof(Model.BellScheduleType));
			}
			else if(typename==typeof(DTOBellSchedule).Name)
			{
				sendingEntity = (dto as DTOBellSchedule).ModelRef;
				extractedType = (typeof(Model.BellSchedule));
			}
			else if(typename==typeof(DTOLessonSubsitutionSchedule).Name)
			{
				sendingEntity = (dto as DTOLessonSubsitutionSchedule).ModelRef;
				extractedType = (typeof(Model.LessonSubsitutionSchedule));
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
				window = new EditWindow(targetType, sendingEntity, _groups,_schedules,_students,_subjects,_teachers,_teacherPhones,_bellScheduleTypes,_bellSchedules,_lessonSubsitutionSchedules,MainWindow);
				window.ShowDialog();
			});
			return window;
		}
		string GetMessageFromException(Exception ex)
		{
			if (ex is DbUpdateException)
			{
				// Распаковываем вложенные исключения
				Exception innerException = ex.InnerException;
				while (innerException.InnerException != null)
					innerException = innerException.InnerException;

				return (innerException.InnerException != null ? innerException.InnerException.Message : innerException.Message);
			}
			else if (ex is SqlException)
			{
				if (ex is SqlException sqlException)
				{
					if (sqlException.Number == 2)
						return "Ошибка: Файл базы данных не найден. Проверьте путь в строке подключения и подключение к серверу";
					else
					if (sqlException.Number == 53 || sqlException.Number == -1)
						return ex.Message;
					else
						return $"SQL-ошибка (код {sqlException.Number}): {sqlException.Message}";
				}
			}
			
			return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
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
					if (ex is SqlException sqlException)
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
				_schedules.Clear();
				_students.Clear();
				_subjects.Clear();
				_teachers.Clear();
				_teacherPhones.Clear();
				_bellSchedules.Clear();
				_bellScheduleTypes.Clear();
				_lessonSubsitutionSchedules.Clear();



				_studentTable.Clear();
				_groupTable.Clear();
				_subjectTable.Clear();
				_teacherTable.Clear();
				_scheduleTable.Clear();
				_bellScheduleTableType.Clear();
				_bellScheduleTable.Clear();
				_lessonSubsitutionScheduleTable.Clear();
			});
		}
		private void SaveTableValues()
		{
			App.Current.Dispatcher.Invoke(() =>
			{
				_studentTable.SaveChanges();
				_groupTable.SaveChanges();
				_subjectTable.SaveChanges();
				_teacherTable.SaveChanges();
				_scheduleTable.SaveChanges();
				_bellScheduleTableType.SaveChanges();
				_bellScheduleTable.SaveChanges();
				_lessonSubsitutionScheduleTable.SaveChanges();
			});
		}
		private void CancelTableChanges()
		{
			App.Current.Dispatcher.Invoke(() =>
			{
				_studentTable.CancelChanges();
				_groupTable.CancelChanges();
				_subjectTable.CancelChanges();
				_teacherTable.CancelChanges();
				_scheduleTable.CancelChanges();
				_bellScheduleTableType.CancelChanges();
				_bellScheduleTable.CancelChanges();
				_lessonSubsitutionScheduleTable.CancelChanges();
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

					foreach (var el in dataBase.Group.ToList()						.OrderBy(x=>x.Year).ThenBy(x=>x.Name))
						_groups.Add(el);
					foreach (var el in dataBase.Schedule.ToList()					.OrderBy(x=>x.IdBellSchedule).ThenBy(x=>x.DayOfTheWeek).ThenBy(x=>x.IdGroup))
						_schedules.Add(el);
					foreach (var el in dataBase.Student.ToList()					.OrderBy(x => x.Gender).ThenBy(x => x.IdGroup).ThenBy(x=>x.BirthDay).ThenBy(x=>x.Surname).ThenBy(x=>x.Name).ThenBy(x=>x.Patronymic))
						_students.Add(el);
					foreach (var el in dataBase.Subject.ToList()					.OrderBy(x => x.Name))
						_subjects.Add(el);
					foreach (var el in dataBase.Teacher.ToList()					.OrderBy(x => x.Gender).ThenBy(x => x.BirthDay).ThenBy(x => x.Surname).ThenBy(x => x.Name).ThenBy(x => x.Patronymic))
						_teachers.Add(el);
					foreach (var el in dataBase.TeacherPhone.ToList()				.OrderBy(x => x.IdTeacher).ThenBy(x => x.PhoneNumber))
						_teacherPhones.Add(el);
					foreach (var el in dataBase.BellScheduleType.ToList()			.OrderBy(x => x.Name))
						_bellScheduleTypes.Add(el);
					foreach (var el in dataBase.BellSchedule.ToList()				.OrderBy(x => x.IdBellScheduleType).ThenBy(x => x.LessonNumber))
						_bellSchedules.Add(el);
					foreach (var el in dataBase.LessonSubsitutionSchedule.ToList()	.OrderBy(x => x.Date).ThenBy(x => x.LessonNumber).ThenBy(x=>x.IdGroup))
						_lessonSubsitutionSchedules.Add(el);

					foreach (var el in _students)
						App.Current.Dispatcher.Invoke(() => { _studentTable.Entries.Add(new DTOStudent(el)); });
					foreach (var el in _groups)
						App.Current.Dispatcher.Invoke(() => { _groupTable.Entries.Add(new DTOGroup(el)); });
					foreach (var el in _subjects)
						App.Current.Dispatcher.Invoke(() => { _subjectTable.Entries.Add(new DTOSubject(el)); });
					foreach (var el in _teachers)
						App.Current.Dispatcher.Invoke(() => { _teacherTable.Entries.Add(new DTOTeacher(el)); });
					foreach (var el in _schedules)
						App.Current.Dispatcher.Invoke(() => { _scheduleTable.Entries.Add(new DTOSchedule(el)); });
					foreach (var el in _bellScheduleTypes)
						App.Current.Dispatcher.Invoke(() => { _bellScheduleTableType.Entries.Add(new DTOBellScheduleType(el)); });
					foreach (var el in _bellSchedules)
						App.Current.Dispatcher.Invoke(() => { _bellScheduleTable.Entries.Add(new DTOBellSchedule(el)); });
					foreach (var el in _lessonSubsitutionSchedules)
						App.Current.Dispatcher.Invoke(() => { _lessonSubsitutionScheduleTable.Entries.Add(new DTOLessonSubsitutionSchedule(el)); });

					OnPropertyChanged(nameof(DTOGroup));
					OnPropertyChanged(nameof(DTOStudents));
					OnPropertyChanged(nameof(DTOSubject));
					OnPropertyChanged(nameof(DTOTeacher));
					OnPropertyChanged(nameof(DTOSchedule));
					OnPropertyChanged(nameof(DTOBellScheduleType));
					OnPropertyChanged(nameof(DTOBellSchedule));
					OnPropertyChanged(nameof(DTOLessonSubsitutionSchedule));
				}
			}
			// Для того, чтобы не было ошибки в xaml
			catch (System.InvalidOperationException)
			{ }
			#if DEBUG
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
			#endif
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
		//IEnumerable<Model.Lesson> FindLessonsUsesSubject(ref List<Model.Lesson> list, int id) 
		//{ 
		//	return list.Where(l => l.Subject.Id == id); 
		//}
		#endregion
		#region Для удаления класса
		IEnumerable<Model.Teacher> FindTeachersUsesGroup(ref List<Model.Teacher> list, int id) 
		{
			return list.Where(t => t.Group.ToList().Exists(g => g.Id == id)); 
		}
		//IEnumerable<Model.Lesson> FindLessonsUsesGroup(ref List<Model.Lesson> list, int id) 
		//{
		//	return list.Where(l => l.IdGroup==id); 
		//}
		IEnumerable<Model.Student> FindStudentsUsesGroup(ref List<Model.Student> list, int id) 
		{
			return list.Where(s => s.IdGroup==id);
		}
		#endregion
		//#region Для удаления урока
		//IEnumerable<Model.Schedule> FindSchedulesUsesLesson(ref List<Model.Schedule> list, int id)
		//{
		//	return list.Where(s => s.IdLesson == id);
		//}
		//#endregion
		#region Для удаления типа расписания звонков
		IEnumerable<Model.BellSchedule> FindBellSchedulesUsesBellScheduleType(ref List<Model.BellSchedule> list, int id)
		{
			return list.Where(s => s.IdBellScheduleType == id);
		}
		#endregion
		#region Для удаления расписания звонков
		IEnumerable<SchoolSchedule.Model.Schedule> FindSchedulesUsesBellSchedule(ref List<Model.Schedule> list, int id)
		{
			return list.Where(x=>x.IdBellSchedule==id);
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
		private List<Model.Group> _groups=new List<Group>();
		private List<Model.Schedule> _schedules = new List<Model.Schedule>();
		private List<Model.Student> _students = new List<Student>();
		private List<Model.Subject> _subjects = new List<Subject>();
		private List<Model.Teacher> _teachers = new List<Teacher>();
		private List<Model.TeacherPhone> _teacherPhones = new List<TeacherPhone>();
		private List<Model.BellScheduleType> _bellScheduleTypes =new List<BellScheduleType>();
		private List<Model.BellSchedule> _bellSchedules = new List<BellSchedule>();
		private List<Model.LessonSubsitutionSchedule> _lessonSubsitutionSchedules=new List<LessonSubsitutionSchedule>();




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


		private TemplateTable<DTOSchedule> _scheduleTable =new TemplateTable<DTOSchedule>();
		public ObservableCollection<DTOSchedule> DTOSchedule
		{ get { return _scheduleTable.Entries; } set { OnPropertyChanged(nameof(DTOSchedule)); _scheduleTable.Entries = value; } }


		private TemplateTable<DTOBellSchedule> _bellScheduleTable=new TemplateTable<DTOBellSchedule> ();
		public ObservableCollection<DTOBellSchedule> DTOBellSchedule
		{ get { return _bellScheduleTable.Entries; } set { OnPropertyChanged(nameof(DTOBellSchedule)); _bellScheduleTable.Entries = value; } }


		private TemplateTable<DTOBellScheduleType> _bellScheduleTableType=new TemplateTable<DTOBellScheduleType> ();
		public ObservableCollection<DTOBellScheduleType> DTOBellScheduleType
		{ get { return _bellScheduleTableType.Entries; } set { OnPropertyChanged(nameof(DTOBellScheduleType)); _bellScheduleTableType.Entries = value; } }


		private TemplateTable<DTOLessonSubsitutionSchedule> _lessonSubsitutionScheduleTable = new TemplateTable<DTOLessonSubsitutionSchedule> ();
		public ObservableCollection<DTOLessonSubsitutionSchedule> DTOLessonSubsitutionSchedule
		{ get { return _lessonSubsitutionScheduleTable.Entries; } set { OnPropertyChanged(nameof(DTOLessonSubsitutionSchedule)); _lessonSubsitutionScheduleTable.Entries = value; } }
	}
}
