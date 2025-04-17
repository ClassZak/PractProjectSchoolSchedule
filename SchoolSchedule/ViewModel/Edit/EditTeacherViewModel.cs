using SchoolSchedule.Model;
using SchoolSchedule.View.Edit;
using SchoolSchedule.View;
using SchoolSchedule.ViewModel.Comands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditTeacherViewModel : ABaseViewModel
	{
		public EditTeacherViewModel()
		{
		}
		public Model.Teacher CurrentTeacher { get; set; }
		public List<Model.Group> Groups { get; set; }
		public List<Model.Subject> Subjects{ get; set; }



		public ObservableCollection<Model.Group> ChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		public ObservableCollection<Model.Group> NotChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		public ObservableCollection<Model.Subject> ChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();
		public ObservableCollection<Model.Subject> NotChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();


		public ObservableCollection<Model.Group> SelectedChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		public ObservableCollection<Model.Group> SelectedNotChoosenGroups { get; set; } = new ObservableCollection<Model.Group>();
		public ObservableCollection<Model.Subject> SelectedChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();
		public ObservableCollection<Model.Subject> SelectedNotChoosenSubjects { get; set; } = new ObservableCollection<Model.Subject>();


		private ObservableCollection<Model.DTO.DTOTeacherPhone> _dtoTeacherPhones=new ObservableCollection<Model.DTO.DTOTeacherPhone>();
		public ObservableCollection<Model.DTO.DTOTeacherPhone> TeacherPhones { get => _dtoTeacherPhones; set => SetPropertyChanged(ref _dtoTeacherPhones, value); } 
		public List<Model.TeacherPhone> _teacherPhones { get; set; } = new List<Model.TeacherPhone>();
		public ObservableCollection<Model.DTO.DTOTeacherPhone> SelectedTeacherPhones { get; set; } = new ObservableCollection<Model.DTO.DTOTeacherPhone>();

		public EditTeacherViewModel(Model.Teacher teacher, List<Model.Group> groups, List<Model.Subject> subjects, List<Model.TeacherPhone> teacherPhones) : this()
		{
			CurrentTeacher = teacher;
			NotChoosenGroups = new ObservableCollection<Model.Group>(groups);
			NotChoosenSubjects = new ObservableCollection<Model.Subject>(subjects);
			if (teacherPhones == null)
				_teacherPhones = new List<Model.TeacherPhone>();
			else
				_teacherPhones = new List<Model.TeacherPhone>(teacherPhones);


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

					addingWindow = new EditWindow(typeof(Model.TeacherPhone), null, null, null, null, null, null, null, _teacherPhones);
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
					addingWindow = new EditWindow(typeof(Model.TeacherPhone), editObject, null, null, null, null, null, null, _teacherPhones);
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
