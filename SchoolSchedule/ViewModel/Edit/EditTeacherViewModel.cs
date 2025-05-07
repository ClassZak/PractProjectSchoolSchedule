using SchoolSchedule.Model;
using SchoolSchedule.View.Edit;
using SchoolSchedule.View;
using SchoolSchedule.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SchoolSchedule.ViewModel.Attributes;
using System.Text.RegularExpressions;
using System.Security.Policy;
using SchoolSchedule.Model.Additional;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditTeacherViewModel : ABaseViewModel
	{
		public EditTeacherViewModel() { }
		public Window OwnerWindow{  get; set; }

		public bool ObjectIsNew { get; set; }
		public Teacher CurrentModel { get; set; } = new Teacher();
		public List<Model.Teacher> ModelsForUniqueCheck { get; set; }
		#region Свойства для ввода
		public string Surname
		{
			get => CurrentModel.Surname; set
			{
				string inputValue = value is null ? "Иванов" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Surname = "Иванов";
				else
					CurrentModel.Surname = inputValue;

				OnPropertyChanged(nameof(Surname));
			}
		}
		public string Name
		{
			get => CurrentModel.Name; set
			{
				string inputValue = value is null ? "Иван" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Name = "Иван";
				else
					CurrentModel.Name = inputValue;

				OnPropertyChanged(nameof(Name));
			}
		}
		public string Patronymic
		{
			get => CurrentModel.Patronymic; set
			{
				string inputValue = value is null ? "Иванович" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Patronymic = "Иванович";
				else
					CurrentModel.Patronymic = inputValue;

				OnPropertyChanged(nameof(Patronymic));
			}
		}
		public DateTime BirthDay
		{
			get => CurrentModel.BirthDay; set
			{
				if (value < new DateTime(2005, 1, 1))
					CurrentModel.BirthDay = new DateTime(2005, 1, 1);
				else
				if (value > DateTime.Now)
					CurrentModel.BirthDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				else
					CurrentModel.BirthDay = value;

				OnPropertyChanged(nameof(BirthDay));
			}
		}
		public string Gender
		{
			get => CurrentModel.Gender; set
			{
				string inputValue = value is null ? string.Empty : value.Trim().ToUpper();
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Gender = "М";
				else
				{

					inputValue = inputValue.Substring(0, 1);
					if (inputValue[0] == 'М' || inputValue[0] == 'M'/*English*/)
						CurrentModel.Gender = "М";
					else if (inputValue[0] == 'Ж' || inputValue[0] == 'F'/*English*/)
						CurrentModel.Gender = "Ж";
					else
						CurrentModel.Gender = "М";
				}
				OnPropertyChanged(nameof(Gender));
			}
		}
		#endregion
		#region Свойства для выбора
		public ObservableCollection<Model.Group> ChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		public ObservableCollection<Model.Group> NotChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		public ObservableCollection<Model.Subject> ChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();
		public ObservableCollection<Model.Subject> NotChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();


		[CollectionOfSelectedItems]
		public ObservableCollection<Model.Group> SelectedChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		[CollectionOfSelectedItems]
		public ObservableCollection<Model.Group> SelectedNotChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		[CollectionOfSelectedItems]
		public ObservableCollection<Model.Subject> SelectedChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();
		[CollectionOfSelectedItems]
		public ObservableCollection<Model.Subject> SelectedNotChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();


		private ObservableCollection<Model.DTO.DTOTeacherPhone> _dtoTeacherPhones=new ObservableCollection<Model.DTO.DTOTeacherPhone>();
		public ObservableCollection<Model.DTO.DTOTeacherPhone> TeacherPhones { get => _dtoTeacherPhones; set => SetPropertyChanged(ref _dtoTeacherPhones, value); } 
		List<Model.TeacherPhone> _teacherPhones { get; set; } = new List<Model.TeacherPhone>();
		public ObservableCollection<Model.DTO.DTOTeacherPhone> SelectedTeacherPhones { get; set; } = new ObservableCollection<Model.DTO.DTOTeacherPhone>();
		#endregion
		public EditTeacherViewModel
		(
			Model.Teacher model, 
			List<Model.Teacher> modelsForUniqueCheck,
			List<Model.Group> groups, 
			List<Model.Subject> subjects, 
			List<Model.TeacherPhone> teacherPhones,
			bool objectIsNew,
			Window window
		) : this()
		{
			ObjectIsNew = objectIsNew;
			CurrentModel = model;

			NotChoosenGroups = new ObservableCollection<Model.Group>(groups);
			NotChoosenSubjects = new ObservableCollection<Model.Subject>(subjects);
			foreach (var el in groups)
			{
				if (el.Teacher is null || el.Teacher.Count == 0)
					continue;

				List<Teacher> groupTeachers = new List<Teacher>();
				foreach (var teacher in el.Teacher)
					groupTeachers.Add(teacher);
				if (groupTeachers.Any(x=>x.Id!=CurrentModel.Id))
					NotChoosenGroups.Remove(el);
			}
			if (teacherPhones == null)
				_teacherPhones = new List<Model.TeacherPhone>();
			else
				_teacherPhones = new List<Model.TeacherPhone>(teacherPhones);

			foreach(var el in CurrentModel.TeacherPhone)
				TeacherPhones.Add(new Model.DTO.DTOTeacherPhone(el));
			foreach(var el in CurrentModel.Subject)
			{
				NotChoosenSubjects.Remove(el);
				ChoosenSubjects.Add(el);
			}
			foreach(var el in CurrentModel.Group)
			{
				NotChoosenGroups.Remove(el);
				ChoosenGroups.Add(el);
			}
			OwnerWindow = window;

			// Так надо
			if (Name is null)
				Name = Name;
			if (Surname is null)
				Surname = Surname;
			if (Patronymic is null)
				Patronymic = Patronymic;
			if (Gender is null)
				Gender = Gender;
			if (BirthDay == null || BirthDay == DateTime.MinValue)
				BirthDay = BirthDay;

			ModelsForUniqueCheck = new List<Teacher>(modelsForUniqueCheck);
			if (!ObjectIsNew)
				ModelsForUniqueCheck.Remove(model);
		}
		public KeyValuePair<bool,string> CheckInputRules()
		{
			if (string.IsNullOrWhiteSpace(CurrentModel.Surname))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? " фамилию нового" : " новую фамилию") + " учителя");
			if (string.IsNullOrWhiteSpace(CurrentModel.Name))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? " имя нового" : " новое имя") + " учителя");
			if (string.IsNullOrWhiteSpace(CurrentModel.Patronymic))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? " отчество нового " : " новое отчество ") + "учителя");
			if (ObjectIsNew && ModelsForUniqueCheck.Where(el => el.Name == CurrentModel.Name && el.Surname == CurrentModel.Surname && el.Patronymic == CurrentModel.Patronymic).Any())
				return new KeyValuePair<bool, string>(false, $"Учитель \"{CurrentModel.Surname} {CurrentModel.Name} {CurrentModel.Patronymic}\" уже присутствует в базе данных");

			return new KeyValuePair<bool, string>(true, null);
		}

		#region Выбор классов и предметов
		RelayCommand _sendToChoosenCommand;
		public RelayCommand SendToChoosenCommand
		{
			get
			{
				return _sendToChoosenCommand ?? (_sendToChoosenCommand = new RelayCommand(param =>
				{
					Type type = param as Type;
					if (type.Name == typeof(Model.Group).Name)
					{
						var selectedGroups = SelectedNotChoosenGroups.ToList();
						if(selectedGroups.Count==0)
						{
							foreach(var group in NotChoosenGroups)
								ChoosenGroups.Add(group);
							NotChoosenGroups.Clear();
						}
						else
						{
							foreach (var group in selectedGroups)
							{
								ChoosenGroups.Add(group);
								NotChoosenGroups.Remove(group);
							}
							SelectedNotChoosenGroups.Clear();
						}
					}
					else
					{
						var selectedSubjects = SelectedNotChoosenSubjects.ToList();
						if (selectedSubjects.Count == 0)
						{
							foreach (var subject in NotChoosenSubjects)
								ChoosenSubjects.Add(subject);
							NotChoosenSubjects.Clear();
						}
						else
						{
							foreach (var subject in selectedSubjects)
							{
								ChoosenSubjects.Add(subject);
								NotChoosenSubjects.Remove(subject);
							}
							SelectedNotChoosenSubjects.Clear();
						}
					}
				}));
			}
		}

		RelayCommand _sendToNotChoosenCommand;
		public RelayCommand SendToNotChoosenCommand
		{
			get
			{
				return _sendToNotChoosenCommand ?? (_sendToNotChoosenCommand = new RelayCommand(param =>
				{
					Type type = param as Type;
					if (type.Name == typeof(Model.Group).Name)
					{
						var selectedGroups = SelectedChoosenGroups.ToList();
						if (selectedGroups.Count == 0)
						{
							foreach(var group in ChoosenGroups)
								NotChoosenGroups.Add(group);
							ChoosenGroups.Clear();
						}
						else
						{
							foreach (var group in selectedGroups)
							{
								NotChoosenGroups.Add(group);
								ChoosenGroups.Remove(group);
							}
							SelectedChoosenGroups.Clear();
						}
					}
					else
					{
						var selectedSubjects = SelectedChoosenSubjects.ToList();
						if (selectedSubjects.Count == 0)
						{
							foreach(var subject in ChoosenSubjects)
								NotChoosenSubjects.Add(subject);
							ChoosenSubjects.Clear();
						}
						else
						{
							foreach (var subject in selectedSubjects)
							{
								NotChoosenSubjects.Add(subject);
								ChoosenSubjects.Remove(subject);
							}
							SelectedChoosenSubjects.Clear();
						}
					}
				}));
			}
		}
		#endregion
		#region Комманды для номера телефона
		RelayCommand _addPhoneCommamd;
		public RelayCommand AddPhoneCommamd
		{
			get
			{
				return _addPhoneCommamd ?? (_addPhoneCommamd=new RelayCommand(param=>
				{
					EditWindow addingWindow = null;

					var dTOList = TeacherPhones.ToList();
					List<Model.TeacherPhone> teacherPhones= new List<Model.TeacherPhone>();
					foreach(var dTO in dTOList)
						teacherPhones.Add(dTO.ModelRef);

					addingWindow = new EditWindow(typeof(Model.TeacherPhone), null, null, null, null, null, null, _teacherPhones,null,null,null, OwnerWindow);
					addingWindow.ShowDialog();

					if(addingWindow.DialogResult)
					{
						TeacherPhones.Add(new Model.DTO.DTOTeacherPhone(addingWindow.EditObject as Model.TeacherPhone));
					}
				}));
			}
		}
		RelayCommand _editPhoneCommand;
		public RelayCommand EditPhoneCommand
		{
			get
			{
				return _editPhoneCommand ?? (_editPhoneCommand = new RelayCommand(param=>
				{
					var selectedObjectsList = param as IList<Model.DTO.DTOTeacherPhone>;
					if(selectedObjectsList.Count!=1)
					{
						MessageBox.Show("Выбирете один объект для изменения", "Ошибка выбора объекта", MessageBoxButton.OK, MessageBoxImage.Stop);
						return;
					}

					var selectedObject = selectedObjectsList[0];

					EditWindow addingWindow = null;

					var dTOList = TeacherPhones.ToList();
					List<Model.TeacherPhone> teacherPhones = new List<Model.TeacherPhone>();
					foreach (var dTO in dTOList)
						teacherPhones.Add(dTO.ModelRef);


					var editObject = new TeacherPhone { Id = selectedObject.ModelRef.Id, IdTeacher = selectedObject.ModelRef.IdTeacher, PhoneNumber = selectedObject.ModelRef.PhoneNumber };
					addingWindow = new EditWindow(typeof(Model.TeacherPhone), editObject, null, null, null, null, null, _teacherPhones, null, null, null, OwnerWindow);
					addingWindow.ShowDialog();

					if(addingWindow.DialogResult)
					{
						//selectedObject=new Model.DTO.DTOTeacherPhone(addingWindow.EditObject as Model.TeacherPhone);
						int index = TeacherPhones.IndexOf(selectedObject);
						TeacherPhones[index] = new Model.DTO.DTOTeacherPhone(editObject);
					}
				}));
			}
		}
		RelayCommand _deletePhoneCommand;
		public RelayCommand DeletePhoneCommand
		{
			get
			{
				return _deletePhoneCommand ?? (_deletePhoneCommand = new RelayCommand(param=>
				{
					var selectedObjectsList = new List<Model.DTO.DTOTeacherPhone>( param as IList<Model.DTO.DTOTeacherPhone>);
					if(selectedObjectsList.Count==0)
					{
						MessageBox.Show("Выбирете хотя бы один объект для изменения", "Ошибка выбора объектов для удаления", MessageBoxButton.OK, MessageBoxImage.Stop);
						return;
					}

					foreach (var el in selectedObjectsList)
						TeacherPhones.Remove(el);
				}));
			}
		}
		#endregion
	}
}
