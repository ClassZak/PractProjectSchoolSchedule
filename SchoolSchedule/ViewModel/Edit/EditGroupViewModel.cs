using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditGroupViewModel : ABaseViewModel
	{
		public EditGroupViewModel() { }

		public Model.Group CurrentModel { get; set; } = new Model.Group { Name = "А", Year = 1 };
		public List<Model.Group> ModelsForUniqueCheck { get; set; }
		#region Свойства для ввода
		public string Name { get => CurrentModel.Name; set 
			{
				string inputValue = value is null ? "А" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Name = "А"; 
				else
				{
					CurrentModel.Name = inputValue.Substring(0,1);
					if (Name[0] < 'А')
						Name = "А";
					else if (Name[0] > 'Е')
						Name = "Е";
				}

				OnPropertyChanged(nameof(Name));
			}
		}
		public int Year { get => CurrentModel.Year; set 
			{ 
				CurrentModel.Year = value;
				if (CurrentModel.Year < 1 || CurrentModel.Year > 11)
					CurrentModel.Year = CurrentModel.Year > 11 ? 11 : 1;

				OnPropertyChanged(nameof(Year));
			}
		}
		#endregion

		public EditGroupViewModel(Model.Group model, List<Model.Group> modelsForUniqueCheck)
		{
			CurrentModel = model;
			ModelsForUniqueCheck = modelsForUniqueCheck;

			// Так надо
			if (Name is null)
				Name = Name;
			if(Year==0)
				Year = Year;
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (ModelsForUniqueCheck.Where(el => el.Name == Name && el.Year == Year).Any())
				return new KeyValuePair<bool, string>(false, $"Класс \"{Year}{Name}\" уже присутствует в базе данных");

			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
