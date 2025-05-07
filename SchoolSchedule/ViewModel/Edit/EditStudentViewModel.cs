using SchoolSchedule.Model;
using SchoolSchedule.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditStudentViewModel : ViewModel.ABaseViewModel
	{
		public EditStudentViewModel() { }

		public bool ObjectIsNew { get; set; }
		public Student CurrentModel { get; set; } = new Student();
		public List<Model.Student> ModelsForUniqueCheck { get; set; } 
		#region Свойства для ввода
		public string Surname{ get => CurrentModel.Surname; set
			{
				string inputValue = value is null ? "Иванов" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Surname = "Иванов";
				else
					CurrentModel.Surname = inputValue;

				OnPropertyChanged(nameof(Surname));
			} 
		}
		public string Name{ get => CurrentModel.Name; set
			{
				string inputValue = value is null ? "Иван" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Name = "Иван";
				else
					CurrentModel.Name = inputValue;

				OnPropertyChanged(nameof(Name));
			} 
		}
		public string Patronymic{ get => CurrentModel.Patronymic; set
			{
				string inputValue = value is null ? "Иванович" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Patronymic = "Иванович";
				else
					CurrentModel.Patronymic = inputValue;

				OnPropertyChanged(nameof(Patronymic));
			} 
		}
		public int IdGroup{ get => CurrentModel.IdGroup; set => CurrentModel.IdGroup = value; }
		public string Email{ get => CurrentModel.Email; set
			{
				CurrentModel.Email = ProcessEmail(value);
				OnPropertyChanged(nameof(Email));
			}
		}
		public DateTime BirthDay{ get => CurrentModel.BirthDay;set
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
		public string Gender{ get => CurrentModel.Gender; set
			{
				string inputValue=value is null ? string.Empty : value.Trim().ToUpper();
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
		#region Работа с почтой
		private string ProcessEmail(string input)
		{
			string cleanValue = (input ?? "").Trim();
			if (string.IsNullOrEmpty(cleanValue))
				return null;


			string[] parts = cleanValue.Split('@');
			string localPart = parts[0];
			string domain = parts.Length > 1 ? parts[1] : "";

			if (string.IsNullOrEmpty(localPart))
				localPart = "ivan";

			string[] domainParts = domain.Split('.');
			string mainDomain = domainParts.Length > 0 ? domainParts[0] : "";
			string tld = domainParts.Length > 1 ? string.Join(".", domainParts.Skip(1)) : "";

			
			if (string.IsNullOrEmpty(mainDomain))
			{
				mainDomain = "yandex";
				tld = "ru";
			}
			else if (string.IsNullOrEmpty(tld))
				tld = "ru";

			// Сборка финального email
			string finalEmail = $"{localPart}@{mainDomain}.{tld}";
			finalEmail = finalEmail.Trim().ToLower();

			// Применение ограничения длины (60 символов)
			finalEmail = finalEmail.Length <= 60 ? finalEmail : finalEmail.Substring(0, 60).Split('@')[0] + "@" + finalEmail.Split('@')[1].Substring(0, Math.Min(60 - finalEmail.Split('@')[0].Length - 1, 59));

			
			if (!EmailRegex.IsMatch(finalEmail))
				return null;

			return finalEmail;
		}

		// Обновленный regex с учетом ограничений БД
		private static readonly Regex EmailRegex = new Regex(
			@"^[a-zA-Z0-9._%+-]{1,58}@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
			RegexOptions.IgnoreCase | RegexOptions.Compiled
		);
		#endregion
		#endregion
		#region Списки для выбора
		public List<Model.Group> Groups { get; set; }
		#endregion


		public EditStudentViewModel(Student model, List<Student> modelsForUniqueCheck, List<Model.Group> groups, bool objectIsNew)
		{
			ObjectIsNew = objectIsNew;
			if (groups.Count == 0)
				throw new ArgumentNullException("В базе данных нет классов для зачисления учеников");
			CurrentModel = model;
			Groups = new List<Model.Group>(groups);

			CurrentModel.IdGroup=groups.First().Id;
			OnPropertyChanged(nameof(IdGroup));

			// Так надо
			if (Name is null)
				Name = Name;
			if (Surname is null)
				Surname = Surname;
			if (Patronymic is null)
				Patronymic = Patronymic;
			if(Gender is null)
				Gender = Gender;
			if(BirthDay == null || BirthDay==DateTime.MinValue)
				BirthDay = BirthDay;

			ModelsForUniqueCheck=new List<Student>(modelsForUniqueCheck);
			if(!ObjectIsNew)
				ModelsForUniqueCheck.Remove(model);
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (string.IsNullOrWhiteSpace(CurrentModel.Surname))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? " фамилию нового(ой)" : " новую фамилию") + " ученика(цы)");
			if (string.IsNullOrWhiteSpace(CurrentModel.Name))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? " имя нового(ой)" : " новое имя") + " ученика(цы)");
			if (string.IsNullOrWhiteSpace(CurrentModel.Patronymic))
				return new KeyValuePair<bool, string>(false, "Введите" + (ObjectIsNew ? " отчество нового(ой)" : " новое отчество") + " ученика(цы)");
			if (CurrentModel.IdGroup == 0)
				return new KeyValuePair<bool, string>(false, "Выберете класс для ученика(цы)");
			if (ModelsForUniqueCheck.Where(el => el.Name == CurrentModel.Name && el.Surname == CurrentModel.Surname && el.Patronymic == CurrentModel.Patronymic && el.Gender == CurrentModel.Gender && el.BirthDay == CurrentModel.BirthDay).Any())
				return new KeyValuePair<bool, string>(false, $"Ученик(ца) \"{CurrentModel.Surname} {CurrentModel.Name} {CurrentModel.Patronymic}\" уже присутствует в базе данных");
			if (CurrentModel.Email != null)
			{
				CurrentModel.Email = CurrentModel.Email.Trim();
				if (CurrentModel.Email == string.Empty)
					CurrentModel.Email = null;
				if (!string.IsNullOrWhiteSpace(CurrentModel.Email) && !EmailRegex.IsMatch(CurrentModel.Email))
					return new KeyValuePair<bool, string>(false, $"Неверный формат электронной почты!");
			}


			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
