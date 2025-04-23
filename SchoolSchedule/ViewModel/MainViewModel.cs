using SchoolSchedule.Model;
using SchoolSchedule.Model.DTO;
using SchoolSchedule.View;
using SchoolSchedule.View.Edit;
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
		public ObservableCollection<Object> SelectedSubjects { get; set; } = new ObservableCollection<Object>();
		public ObservableCollection<Object> SelectedGroups { get; set; } = new ObservableCollection<Object>();
		public ObservableCollection<Model.DTO.DTOStudent> SelectedStudents { get; set; } = new ObservableCollection<Model.DTO.DTOStudent>();
		public ObservableCollection<Model.DTO.DTOTeacher> SelectedTeachers { get; set; } = new ObservableCollection<Model.DTO.DTOTeacher>();
		public ObservableCollection<Object> SelectedTheacherPhones { get; set; } = new ObservableCollection<Object>();
		public ObservableCollection<Model.DTO.DTOLesson> SelectedLessons{ get; set; } = new ObservableCollection<Model.DTO.DTOLesson>();
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

		TaskViewModel _taskViewModel=new TaskViewModel();
		public string TaskName{ get => _taskViewModel.TaskName;set {  } }
		#region Запуск команд
		private async Task ExecuteCommandAsync(string commandName, Func<Task> commandLogic)
		{
			// Пытаемся захватить семафор без ожидания
			if (!await _semaphore.WaitAsync(TimeSpan.Zero))
			{
				ShowBusyMessage();
				return;
			}

			var taskInfo = new SchoolSchedule.ViewModel.TaskModel.TaskViewModel (commandName,ETaskStatus.Unknown);

			try
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					TaskName = commandName;
				});

				_taskViewModel.SetTaskStatus(ETaskStatus.InProgress);
				await commandLogic();
				_taskViewModel.SetTaskStatus(ETaskStatus.Completed);
			}
			catch (Exception ex)
			{
				_taskViewModel.SetTaskStatus(ETaskStatus.Failed);
				_taskViewModel.ErrorMessage = $"Ошибка в задаче '{commandName}': {ex.Message}";
				HandleException(ex);
			}
			finally
			{
				_semaphore.Release();
				Application.Current.Dispatcher.Invoke(() =>
				{
					_taskViewModel.SetTaskStatus(ETaskStatus.Completed);
				});
			}
		}
		#region CRUD команды
		private async Task SaveEntityAsync(SchoolScheduleEntities db, object entity, Type targetType)
		{
			switch (entity)
			{
				case Model.Group group:
					db.Group.Add(group);
					break;
				case Model.Subject subject:
					db.Subject.Add(subject);
					break;
					// ... другие типы
			}
			await db.SaveChangesAsync();
		}
		#region Команда добавления
		private RelayCommand _addCommand2;
		public RelayCommand AddCommand2
		{
			get
			{
				return _addCommand2 ?? new RelayCommand(
				async param => await ExecuteCommandAsync("Добавление новых записей",async () =>
				{
					if (!(param is Type targetType))
						throw new ArgumentException("Выбран неверный тип аргумента");

					// Общая логика для всех типов
					var result = await ShowEditWindowAsync(targetType, null);
					if (result.DialogResult)
					{
						using (var dataBase=new Model.SchoolScheduleEntities())
						{
							await SaveEntityAsync(dataBase, result.EditObject, targetType);
						}
						_updateData.Execute(targetType);
					}
				}));
			}
		}
		private RelayCommand _updateCommand2;
		public RelayCommand UpdateCommand2
		{
			get
			{
				return _updateCommand2 ?? new RelayCommand(
				async param => await ExecuteCommandAsync("Загрузка данных", async () =>
				{
					await LoadDataAsync();
				}));
			}
		}
		#endregion
		#endregion
		#endregion
		#region Общие методы
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
		private async Task<EditWindow> ShowEditWindowAsync(Type targetType, object entity)
		{
			EditWindow window = null;
			await Application.Current.Dispatcher.InvokeAsync(() =>
			{
				window = new EditWindow(targetType, entity, _groups,_lessons,_schedules,_students,_subjects,_teachers,_teacherPhones,MainWindow);
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
				}
				else
				{
					MessageBox.Show(ex.InnerException != null ? ex.InnerException.Message : ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
					//_updateData.Execute(null);
				}
			});
		}
		#endregion
		#endregion
		private void LoadData()
		{
			try
			{
				using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
				{
					dataBase.Database.Connection.Open();

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

		#region Команды
		#region Команда добавления
		// Тип аргумента: Type
		private RelayCommand _addCommand;
		public RelayCommand AddCommand
		{
			get
			{
				return _addCommand ??
				(_addCommand = new RelayCommand(
					async param => await Task.Run(async () =>
					{
						try
						{
							if (!(param is Type targetType))
								throw new ArgumentException("Выбран неверный тип аргумента");

							if (targetType.Name == typeof(Model.Group).Name)
							{
								EditWindow addingWindow = null;

								App.Current.Dispatcher.Invoke(() =>
								{
									addingWindow = new EditWindow(typeof(Model.Group), null, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow)
									{
										Owner = MainWindow
									};
									addingWindow.ShowDialog();
								});


								if (addingWindow.DialogResult)
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										dataBase.Group.Add(addingWindow.EditObject as Model.Group);
										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(typeof(Model.Group));
								}
							}
							if (targetType.Name == typeof(Model.Subject).Name)
							{
								EditWindow addingWindow = null;

								App.Current.Dispatcher.Invoke(() =>
								{
									addingWindow = new EditWindow(typeof(Model.Subject), null, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow)
									{
										Owner = MainWindow
									};
									addingWindow.ShowDialog();
								});

								if (addingWindow.DialogResult)
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										dataBase.Subject.Add(addingWindow.EditObject as Model.Subject);
										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(typeof(Model.Subject));
								}
							}
							if (targetType.Name == typeof(Model.Student).Name)
							{
								EditWindow addingWindow = null;

								App.Current.Dispatcher.Invoke(() =>
								{
									addingWindow = new EditWindow(typeof(Model.Student), null, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow)
									{
										Owner = MainWindow
									};
									addingWindow.ShowDialog();
								});

								if (addingWindow.DialogResult)
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										dataBase.Student.Add(addingWindow.EditObject as Model.Student);
										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(typeof(Model.Student));
								}
							}
							if (targetType.Name == typeof(Model.Teacher).Name)
							{
								EditWindow addingWindow = null;

								App.Current.Dispatcher.Invoke(() =>
								{
									addingWindow = new EditWindow(typeof(Model.Teacher), null, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow)
									{
										Owner = MainWindow
									};
									addingWindow.ShowDialog();
								});

								if(addingWindow.DialogResult)
								{
									var newTeacher = addingWindow.EditObject as Model.Teacher;
									List<Model.TeacherPhone> phones=new List<Model.TeacherPhone>(newTeacher.TeacherPhone);
									List<Model.Group> groups=new List<Model.Group>(newTeacher.Group);
									List<Model.Subject> subjects=new List<Model.Subject>(newTeacher.Subject);

									newTeacher.Subject.Clear();
									newTeacher.Group.Clear();
									newTeacher.TeacherPhone.Clear();
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										var forEdit = dataBase.Teacher.Add(addingWindow.EditObject as Model.Teacher);
										await dataBase.SaveChangesAsync();

										if (forEdit == null)
											throw new Exception("Не удалось добавить предметы и классы для учителей");
										foreach (var el in subjects)
											forEdit.Subject.Add(await dataBase.Subject.FindAsync(el.Id));
										foreach (var el in groups)
											forEdit.Group.Add(await dataBase.Group.FindAsync(el.Id));
										foreach (var el in phones)
											dataBase.TeacherPhone.Add(new Model.TeacherPhone { IdTeacher=el.IdTeacher,PhoneNumber=el.PhoneNumber });

										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
							}
							if (targetType.Name == typeof(Model.Lesson).Name)
							{
								EditWindow addingWindow = null;

								App.Current.Dispatcher.Invoke(() =>
								{
									addingWindow = new EditWindow(typeof(Model.Lesson), null, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow);
									addingWindow.Owner = MainWindow;
									addingWindow.ShowDialog();
								});
								if (addingWindow.DialogResult)
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										dataBase.Lesson.Add(addingWindow.EditObject as Model.Lesson);
										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
							}
							if (targetType.Name == typeof(Model.Schedule).Name)
							{
								EditWindow addingWindow = null;

								App.Current.Dispatcher.Invoke(() =>
								{
									addingWindow = new EditWindow(typeof(Model.Schedule), null, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow);
									addingWindow.Owner = MainWindow;
									addingWindow.ShowDialog();
								});
								if (addingWindow.DialogResult)
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										dataBase.Schedule.Add(addingWindow.EditObject as Model.Schedule);
										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
							}
						}
						catch (DbUpdateException ex)
						{
							// Распаковываем вложенные исключения
							Exception innerException = ex.InnerException;
							while (innerException.InnerException != null)
								innerException = innerException.InnerException;

							MessageBox.Show(innerException.InnerException != null ? innerException.InnerException.Message : innerException.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex) when (ex.Number == 2)
						{
							MessageBox.Show("Ошибка: Файл базы данных не найден. Проверьте путь в строке подключения и подключение к серверу", "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex) when (ex.Number == 53 || ex.Number == -1) // Ошибки подключения
						{
							MessageBox.Show(ex.Message, "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex)
						{
							Console.WriteLine($"SQL-ошибка (код {ex.Number}): {ex.Message}");
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.InnerException != null ? ex.InnerException.Message : ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
							_updateData.Execute(null);
						}
					}
				)));
			}
		}
		#endregion
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
					async param => await Task.Run(() =>
					{
						LoadData();
						return Task.CompletedTask;
					}
				)));
			}
		}
		#endregion
		#region Команда изменения
		RelayCommand _editCommand;
		public RelayCommand EditCommand
		{
			get
			{
				return _editCommand ??
				(_editCommand=new RelayCommand(
					async param => await Task.Run(async ()=>
					{
						try
						{
							if (param != null)
							{
								var selectedObjects = param as IList;
								if (ReferenceEquals(param, SelectedSubjects))
								{
									List<Model.DTO.DTOSubject> selectedObjectsList = new List<Model.DTO.DTOSubject>();
									foreach (var el in selectedObjects)
										selectedObjectsList.Add(el as Model.DTO.DTOSubject);

									if (selectedObjectsList.Count != 1)
									{
										MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
										return;
									}

									var selectedObject = selectedObjectsList[0];
									EditWindow addingWindow = null;

									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new EditWindow(typeof(Model.Subject), selectedObject.ModelRef, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow)
										{
											Owner = MainWindow
										};
										addingWindow.ShowDialog();
									});

									if (addingWindow.DialogResult)
									{
										var newObject = (addingWindow.EditObject as Model.Subject);
										using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
										{
											var forUpdate = await dataBase.Subject.FirstOrDefaultAsync(x => x.Id == selectedObject.ModelRef.Id);
											if (forUpdate == null)
												throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
											forUpdate.Name = newObject.Name;

											await dataBase.SaveChangesAsync();
										}
										_updateData.Execute(null);
									}
								}
								if (ReferenceEquals(param, SelectedGroups))
								{
									List<Model.DTO.DTOGroup> selectedObjectsList = new List<Model.DTO.DTOGroup>();
									foreach (var el in selectedObjects)
										selectedObjectsList.Add(el as Model.DTO.DTOGroup);

									if (selectedObjectsList.Count != 1)
									{
										MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
										return;
									}

									var selectedObject = selectedObjectsList[0];
									EditWindow addingWindow = null;

									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new EditWindow(typeof(Model.Group), selectedObject.ModelRef, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow)
										{
											Owner = MainWindow
										};
										addingWindow.ShowDialog();
									});
									if (addingWindow.DialogResult)
									{
										var newObject = (addingWindow.EditObject as Model.Group);
										using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
										{
											var forUpdate = await dataBase.Group.FirstOrDefaultAsync(x => x.Id == selectedObject.ModelRef.Id);
											if (forUpdate == null)
												throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
											forUpdate.Name = newObject.Name;
											forUpdate.Year = newObject.Year;

											await dataBase.SaveChangesAsync();
										}
										_updateData.Execute(null);
									}
								}
								if (ReferenceEquals(param, SelectedStudents))
								{
									List<Model.DTO.DTOStudent> selectedObjectsList = new List<Model.DTO.DTOStudent>();
									foreach (var el in selectedObjects)
										selectedObjectsList.Add(el as Model.DTO.DTOStudent);

									if (selectedObjectsList.Count != 1)
									{
										MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
										return;
									}

									var selectedObject = selectedObjectsList[0];
									EditWindow addingWindow = null;

									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new EditWindow(typeof(Model.Student), selectedObject.ModelRef, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow);
										addingWindow.Owner = MainWindow;
										addingWindow.ShowDialog();
									});
									if (addingWindow.DialogResult)
									{
										var newObject = (addingWindow.EditObject as Model.Student);
										using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
										{
											var forUpdate = await dataBase.Student.FirstOrDefaultAsync(x => x.Id == selectedObject.ModelRef.Id);
											if (forUpdate == null)
												throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
											forUpdate.Surname = newObject.Surname;
											forUpdate.Name = newObject.Name;
											forUpdate.Patronymic = newObject.Patronymic;
											forUpdate.IdGroup = newObject.IdGroup;
											forUpdate.Email = newObject.Email;

											await dataBase.SaveChangesAsync();
										}
										_updateData.Execute(null);
									}
								}
								if (ReferenceEquals(param, SelectedLessons))
								{
									if (SelectedLessons.Count != 1)
									{
										MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
										return;
									}
									var selectedObject = SelectedLessons[0];

									EditWindow addingWindow = null;
									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new EditWindow(typeof(Model.Lesson), selectedObject.ModelRef, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow);
										addingWindow.Owner = MainWindow;
										addingWindow.ShowDialog();
									});
									if (addingWindow.DialogResult)
									{
										var newObject = (addingWindow.EditObject as Model.Lesson);
										using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
										{
											var forUpdate = await dataBase.Lesson.FirstOrDefaultAsync(x => x.Id == selectedObject.ModelRef.Id);
											if (forUpdate == null)
												throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
											forUpdate.IdSubject = selectedObject.ModelRef.IdSubject;
											forUpdate.IdGroup = selectedObject.ModelRef.IdGroup;
											forUpdate.Number = selectedObject.ModelRef.Number;

											await dataBase.SaveChangesAsync();
										}
										_updateData.Execute(null);
									}
								}
								if (ReferenceEquals(param, SelectedSchedules))
								{
									if (SelectedSchedules.Count != 1)
									{
										MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
										return;
									}
									var selectedObject = SelectedSchedules[0];

									EditWindow addingWindow = null;
									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new EditWindow(typeof(Model.Schedule), selectedObject.ModelRef, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow);
										addingWindow.Owner = MainWindow;
										addingWindow.ShowDialog();
									});
									if (addingWindow.DialogResult)
									{
										var newObject = (addingWindow.EditObject as Model.Schedule);
										using (var dataBase = new Model.SchoolScheduleEntities())
										{
											var forUpdate = await dataBase.Schedule.FirstOrDefaultAsync(x => x.Id == selectedObject.ModelRef.Id);
											if (forUpdate == null)
												throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
											forUpdate.IdLesson = selectedObject.ModelRef.IdLesson;
											forUpdate.IdTeacher = selectedObject.ModelRef.IdTeacher;
											forUpdate.StartTime = selectedObject.ModelRef.StartTime;
											forUpdate.EndTime = selectedObject.ModelRef.EndTime;
											forUpdate.Date = selectedObject.ModelRef.Date;

											await dataBase.SaveChangesAsync();
										}
										_updateData.Execute(null);
									}
								}
								if (ReferenceEquals(param, SelectedTeachers))
								{
									if(SelectedTeachers.Count!=1)
									{
										MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
										return;
									}
									var selectedObject = SelectedTeachers[0];

									EditWindow addingWindow = null;
									App.Current.Dispatcher.Invoke(() =>
									{
										addingWindow = new EditWindow(typeof(Model.Teacher), selectedObject.ModelRef, _groups, _lessons, _schedules, _students, _subjects, _teachers, _teacherPhones, MainWindow);
										addingWindow.Owner = MainWindow;
										addingWindow.ShowDialog();
									});
									if (addingWindow.DialogResult)
									{
										var newTeacher = addingWindow.EditObject as Model.Teacher;
										List<Model.TeacherPhone> phones = new List<Model.TeacherPhone>(newTeacher.TeacherPhone);
										List<Model.Group> groups = new List<Model.Group>(newTeacher.Group);
										List<Model.Subject> subjects = new List<Model.Subject>(newTeacher.Subject);

										newTeacher.Subject.Clear();
										newTeacher.Group.Clear();
										newTeacher.TeacherPhone.Clear();
										using (var dataBase = new Model.SchoolScheduleEntities())
										{
											var forUpdate = await dataBase.Teacher
												.Include(t => t.TeacherPhone) // Добавляем Include для загрузки связанных телефонов
												.FirstOrDefaultAsync(x => x.Id == selectedObject.ModelRef.Id);

											if (forUpdate == null)
												throw new Exception("Объект не найден");

											forUpdate.Surname = newTeacher.Surname;
											forUpdate.Name = newTeacher.Name;
											forUpdate.Patronymic = newTeacher.Patronymic;

											foreach (var phone in forUpdate.TeacherPhone.ToList())
											{
												if(phone.IdTeacher==forUpdate.Id)
													dataBase.TeacherPhone.Remove(phone);
											}

											foreach (var el in phones)
											{
												dataBase.TeacherPhone.Add(new Model.TeacherPhone
												{
													IdTeacher = forUpdate.Id,
													PhoneNumber = el.PhoneNumber
												});
											}

											// Обновляем Subject и Group (many-to-many)
											forUpdate.Subject.Clear();
											foreach (var el in subjects)
											{
												var subject = await dataBase.Subject.FindAsync(el.Id);
												if (subject != null)
													forUpdate.Subject.Add(subject);
											}

											forUpdate.Group.Clear();
											foreach (var el in groups)
											{
												var group = await dataBase.Group.FindAsync(el.Id);
												if (group != null)
													forUpdate.Group.Add(group);
											}

											await dataBase.SaveChangesAsync();
										}
										_updateData.Execute(null);
									}
								}
							}
						}
						catch (DbUpdateException ex)
						{
							// Распаковываем вложенные исключения
							Exception innerException = ex.InnerException;
							while (innerException.InnerException != null)
								innerException = innerException.InnerException;

							MessageBox.Show(innerException.InnerException != null ? innerException.InnerException.Message : innerException.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex) when (ex.Number == 2)
						{
							MessageBox.Show("Ошибка: Файл базы данных не найден. Проверьте путь в строке подключения и подключение к серверу", "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex) when (ex.Number == 53 || ex.Number == -1) // Ошибки подключения
						{
							MessageBox.Show(ex.Message, "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex)
						{
							Console.WriteLine($"SQL-ошибка (код {ex.Number}): {ex.Message}");
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.InnerException != null ? ex.InnerException.Message : ex.Message, "Ошибка редактирования данных", MessageBoxButton.OK, MessageBoxImage.Stop);
							_updateData.Execute(null);
						}
					}
				)));
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
					async param => await Task.Run(async () =>
					{
						try
						{
							if (param != null)
							{
								if (ReferenceEquals(param, SelectedSubjects))
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										var teachers = await dataBase.Teacher.ToListAsync();
										var lessons = await dataBase.Lesson.ToListAsync();
										foreach (var el in SelectedSubjects)
										{
											var elRef = (el as Model.DTO.DTOSubject);
											var teachersUsesSubject = FindTeachersUsesSubject(ref teachers, elRef.ModelRef.Id);
											var lessonsUsesSubject = FindLessonsUsesSubject(ref lessons, elRef.ModelRef.Id);

											if (teachersUsesSubject.Count() != 0 || lessonsUsesSubject.Count() != 0)
												throw new Exception($"Удалите всех уроков и учителей, ссылающихся на предмет \"{elRef.Name}\"");

											var forDelete = await dataBase.Subject.FirstOrDefaultAsync(x => x.Id == elRef.ModelRef.Id);
											if (forDelete == null)
												throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
											dataBase.Subject.Remove(forDelete);
										}

										await dataBase.SaveChangesAsync();
									}
									_subjectTable.Remove((param as ObservableCollection<DTOSubject>));
									_updateData.Execute(null);
								}
								if (ReferenceEquals(param, SelectedGroups))
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										var teachers = await dataBase.Teacher.ToListAsync();
										var lessons = await dataBase.Lesson.ToListAsync();
										var students = await dataBase.Student.ToListAsync();
										foreach (var el in SelectedGroups)
										{
											var elRef = (el as Model.DTO.DTOGroup);
											var teachersUses = FindTeachersUsesGroup(ref teachers, elRef.ModelRef.Id);
											var lessonsUses = FindLessonsUsesGroup(ref lessons, elRef.ModelRef.Id);
											var studentsUsees = FindStudentsUsesGroup(ref students, elRef.ModelRef.Id);

											if (teachersUses.Count() != 0 || lessonsUses.Count() != 0 || studentsUsees.Count() != 0)
												throw new Exception($"Удалите записи всех уроков, учителей и всех студентов, ссылающихся на класс \"{elRef.ModelRef}\"");

											var forDelete = await dataBase.Group.FirstOrDefaultAsync(x => x.Id == elRef.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
											dataBase.Group.Remove(forDelete);
										}

										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
								if (ReferenceEquals(param, SelectedStudents))
								{
									MessageBoxResult messageBoxResult =
									MessageBox.Show("Вы уверены начать отчисление учеников?", "Отчисление", MessageBoxButton.YesNo, MessageBoxImage.Question);
									if (messageBoxResult != MessageBoxResult.Yes)
										return;

									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										foreach (var el in SelectedStudents)
										{
											var forDelete = await dataBase.Student.FirstOrDefaultAsync(x => x.Id == el.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
											dataBase.Student.Remove(forDelete);
										}

										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
								if (ReferenceEquals(param, SelectedLessons))
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										var schedules = await dataBase.Schedule.ToListAsync();
										foreach (var el in SelectedLessons)
										{
											var schedulesUseesLesson = FindSchedulesUsesLesson(ref schedules, el.Id);
											if (schedulesUseesLesson.Any())
												throw new Exception($"Удалите все объекты расписания, в которых проводится занятие по предмету \"{el.Subject}\" с классом \"{el.Group}\"");

											var forDelete = await dataBase.Lesson.FirstOrDefaultAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
											dataBase.Lesson.Remove(forDelete);
										}

										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
								if (ReferenceEquals(param, SelectedSchedules))
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										foreach (var el in SelectedSchedules)
										{
											var forDelete = await dataBase.Schedule.FirstOrDefaultAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
											dataBase.Schedule.Remove(forDelete);
										}

										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
								if (ReferenceEquals(param, SelectedTeachers))
								{
									using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
									{
										var schedules = await dataBase.Schedule.ToListAsync();
										var phones = await dataBase.TeacherPhone.ToListAsync();
										foreach (var el in SelectedTeachers)
										{
											var forDelete = await dataBase.Teacher.FirstOrDefaultAsync(x => x.Id == el.Id);
											var schedulesUsesTeacher = FindSchedulesUsesTeacher(ref schedules, forDelete.Id);
											var phonesUsesTeacher = FindTeacherPhonesUsesTeacher(ref phones, forDelete.Id);
											if(schedulesUsesTeacher.Any())
												throw new Exception($"Удалите все объекты расписания, в которых записан преподаватель {el.ModelRef}");
											if (forDelete == null)
												throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");

											foreach (var p in phonesUsesTeacher)
												dataBase.TeacherPhone.Remove(p);

											dataBase.Teacher.Remove(forDelete);
										}

										await dataBase.SaveChangesAsync();
									}
									_updateData.Execute(null);
								}
							}
						}
						catch (DbUpdateException ex)
						{
							// Распаковываем вложенные исключения
							Exception innerException = ex.InnerException;
							while (innerException.InnerException != null)
								innerException = innerException.InnerException;

							MessageBox.Show(innerException.InnerException != null ? innerException.InnerException.Message : innerException.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex) when (ex.Number == 2)
						{
							MessageBox.Show("Ошибка: Файл базы данных не найден. Проверьте путь в строке подключения и подключение к серверу", "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex) when (ex.Number == 53 || ex.Number == -1) // Ошибки подключения
						{
							MessageBox.Show(ex.Message, "Ошибка подключения к базе данных", MessageBoxButton.OK, MessageBoxImage.Stop);
						}
						catch (SqlException ex)
						{
							Console.WriteLine($"SQL-ошибка (код {ex.Number}): {ex.Message}");
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.InnerException != null ? ex.InnerException.Message : ex.Message, "Ошибка удаления данных", MessageBoxButton.OK, MessageBoxImage.Stop);
							_updateData.Execute(null);
						}
					}
				)));
			}
		}
		#endregion
		#endregion
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
