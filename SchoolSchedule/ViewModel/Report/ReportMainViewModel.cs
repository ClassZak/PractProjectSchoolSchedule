using Microsoft.Win32;
using SchoolSchedule.Model;
using SchoolSchedule.Model.DTO;
using SchoolSchedule.ViewModel.Commands;
using SchoolSchedule.ViewModel.TaskModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SchoolSchedule.ViewModel.Report
{
	public class ReportMainViewModel : ABaseViewModel
	{
		public ReportMainViewModel() { }
		private int _selectedTabIndex;

		public int SelectedTabIndex
		{
			get => _selectedTabIndex;
			set
			{
				_selectedTabIndex = value;
				OnPropertyChanged(nameof(SelectedTabIndex));
			}
		}
		#region Статус задачи
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
		#endregion
		#region Списки для выбора
		public ObservableCollection<Model.Group> Groups { get; set; } = new ObservableCollection<Model.Group>();
		public ObservableCollection<Model.Teacher> Teachers{ get;set; } = new ObservableCollection<Model.Teacher>();
		public ObservableCollection<Model.BellScheduleType> BellScheduleTypes { get; set; } = new ObservableCollection<Model.BellScheduleType>();
		#endregion
		#region Выбранные объекты
		public int IdTeacher { get; set; }
		public int IdGroup { get; set; }
		public int IdBellScheduleType { get; set; }
		public DateTime Date { get; set; } = DateTime.Today;
		#endregion
		#region Команды
		RelayCommand _generateTeacherReportCommand;
		public RelayCommand GenerateTeacherReportCommand { get 
			{
				return _generateTeacherReportCommand ?? (_generateTeacherReportCommand = new RelayCommand(
				async param => 
				{
					await ExecuteReportAsync(GenerateTeacherReport);
				}));
			}
		}
		RelayCommand _generateGroupReportCommand;
		public RelayCommand GenerateGroupReportCommand{ get 
			{
				return _generateGroupReportCommand ?? (_generateGroupReportCommand = new RelayCommand(
				async param => 
				{
					await ExecuteReportAsync(GenerateGroupReport);
				}));
			}
		}
		#region Команды сохранения
		RelayCommand _saveToTxtCommand;
		public RelayCommand SaveToTxtCommand { get 
			{
				return _saveToTxtCommand ?? (_saveToTxtCommand=new RelayCommand(async param=>
				{
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.Title = "Сохранение отчёта";
					saveFileDialog.Filter = "Текстовый файл (*.txt)|*.txt";
					if (saveFileDialog.ShowDialog()==true)
					{
						switch(SelectedTabIndex)
						{
							case 0:
								await IO.Saver.SaveToFileAsync(ShowLessonsAtDayForTeacherByIdTeacher_ProcResults, saveFileDialog.FileName);
								break;
							case 1:
								await IO.Saver.SaveToFileAsync(ShowStudentsByGroupByIdGroup_ProcResults, saveFileDialog.FileName);
								break;
							default:
								throw new ArgumentException("Неверный индекс вкладки");
						}
					}
				}));
			}
		}
		#endregion
		#endregion
		#region Буфер обмена
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
		#region Результат отчёта
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public List<ShowLessonsAtDayForTeacherByIdTeacher_Result> SelectedShowLessonsAtDayForTeacherByIdTeacher_ProcResults { get; set; } = new List<ShowLessonsAtDayForTeacherByIdTeacher_Result>();
		List<ShowLessonsAtDayForTeacherByIdTeacher_Result> _showLessonsAtDayForTeacherByIdTeacher_ProcResults = new List<ShowLessonsAtDayForTeacherByIdTeacher_Result>();
		public List<ShowLessonsAtDayForTeacherByIdTeacher_Result> ShowLessonsAtDayForTeacherByIdTeacher_ProcResults{ get=>_showLessonsAtDayForTeacherByIdTeacher_ProcResults; set {  _showLessonsAtDayForTeacherByIdTeacher_ProcResults = value; OnPropertyChanged(nameof(ShowLessonsAtDayForTeacherByIdTeacher_ProcResults));} }
		
		[ViewModel.Attributes.CollectionOfSelectedItems]
		public List<ShowStudentsByGroupByIdGroup_Result> SelectedShowStudentsByGroupByIdGroup_ProcResults = new List<ShowStudentsByGroupByIdGroup_Result>();
		List<ShowStudentsByGroupByIdGroup_Result> _showStudentsByGroupByIdGroup_ProcResults = new List<ShowStudentsByGroupByIdGroup_Result>();
		public List<ShowStudentsByGroupByIdGroup_Result> ShowStudentsByGroupByIdGroup_ProcResults { get => _showStudentsByGroupByIdGroup_ProcResults; set {  _showStudentsByGroupByIdGroup_ProcResults = value; OnPropertyChanged(nameof(ShowStudentsByGroupByIdGroup_ProcResults));} }
		#endregion
		public ReportMainViewModel(MainViewModel mainViewModel)
		{
			Groups=new ObservableCollection<Model.Group>(mainViewModel.GetGroups());
			Teachers=new ObservableCollection<Model.Teacher>(mainViewModel.GetTeachers());
			BellScheduleTypes=new ObservableCollection<Model.BellScheduleType>(mainViewModel.GetBellScheduleTypes());
		}
		public ReportMainViewModel(List<Group> groups, List<Teacher> teachers, List<BellScheduleType> bellScheduleTypes)
		{
			Groups = new ObservableCollection<Group>(groups);
			Teachers = new ObservableCollection<Teacher>(teachers);
			BellScheduleTypes = new ObservableCollection<BellScheduleType>(bellScheduleTypes);
		}

		private async Task ExecuteReportAsync(Func<Task> reportAction)
		{
			try
			{
				ETaskStatus = ETaskStatus.InProgress;
				TaskName = "Выполнение отчёта";
				ErrorMessage = null;

				await reportAction();

				ETaskStatus = ETaskStatus.Completed;
				TaskStatus = "Готово";
			}
			catch (Exception ex)
			{
				ETaskStatus = ETaskStatus.Failed;
				ErrorMessage = HandleException(ex);
				MessageBox.Show(ErrorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async Task GenerateTeacherReport()
		{
			if (IdTeacher== 0)
				throw new ArgumentException("Выберите учителя для отчёта");
			if (IdBellScheduleType== 0)
				throw new ArgumentException("Выберите расписание звонков для отчёта");

			if (Teachers.Count== 0)
				throw new ArgumentException("В базе данных нет учителей! Загрузите данные из базы данных и повторите попытку");
			if (BellScheduleTypes.Count== 0)
				throw new ArgumentException("В базе данных нет расписаний звонков! Загрузите данные из базы данных и повторите попытку");

			using (var context = new SchoolScheduleEntities())
			{
				var result = await context.Database.SqlQuery<Model.ShowLessonsAtDayForTeacherByIdTeacher_Result>(
					"EXEC ShowLessonsAtDayForTeacherByIdTeacher @idTeacher, @date, @idBellScheduleType",
					new SqlParameter("@idTeacher", IdTeacher),
					new SqlParameter("@date", Date),
					new SqlParameter("@idBellScheduleType", IdBellScheduleType)
				).ToListAsync();

				ShowLessonsAtDayForTeacherByIdTeacher_ProcResults=result;
			}
		}

		private async Task GenerateGroupReport()
		{
			if (IdGroup==0)
				throw new ArgumentException("Выберите класс для отчёта");

			using (var context = new SchoolScheduleEntities())
			{
				var result = await context.Database.SqlQuery<Model.ShowStudentsByGroupByIdGroup_Result>(
					"EXEC ShowStudentsByGroupByIdGroup @idGroup",
					new SqlParameter("@idGroup", IdGroup)
				).ToListAsync();

				ShowStudentsByGroupByIdGroup_ProcResults=result;
			}
		}
		#region Обработка исключений
		private string HandleException(Exception ex)
		{
			if (ex is SqlException sqlEx) return HandleSqlException(sqlEx);
			return ex.InnerException?.Message ?? ex.Message;
		}
		private string HandleSqlException(SqlException ex)
		{
			switch (ex.Number)
			{
				case 50000: 
					return ex.Message.Split('\n')[0];
				case 547: 
					return "Ошибка связанных данных. Загрузите данные из базы данных и повторите попытку";
				default: 
					return $"Ошибка SQL [{ex.Number}]: {ex.Message}";
			}
		}
		#endregion
	}
}