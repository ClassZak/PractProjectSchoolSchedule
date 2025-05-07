using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditBellScheduleTypeViewModel : ABaseViewModel
	{
		public EditBellScheduleTypeViewModel() { }

		public Model.BellScheduleType CurrentModel { get; set; } = new Model.BellScheduleType { Name="Расписание звонков"};
		#region Свойства для ввода
		public string Name{ get => CurrentModel.Name; set 
			{
				string inputValue = value is null ? "Расписание звонков" : Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-\d]", "");
				if (string.IsNullOrEmpty(inputValue))
					CurrentModel.Name = "Расписание звонков";
				else
					CurrentModel.Name = inputValue;

				OnPropertyChanged(nameof(Name));
			} 
		}
		#endregion

		public List<Model.BellScheduleType> ModelsForUniqueCheck { get; set; }

		public EditBellScheduleTypeViewModel(Model.BellScheduleType model, List<Model.BellScheduleType> modelsForUniqueCheck)
		{
			CurrentModel=model;
			ModelsForUniqueCheck = modelsForUniqueCheck;

			// Так надо
			if(Name is null)
				Name = Name;
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (string.IsNullOrWhiteSpace(CurrentModel.Name))
			{
				Name = "Расписание звонков";
				OnPropertyChanged(nameof(Name));
				return new KeyValuePair<bool, string>(false, "Введите не пустое значение для названия расписания");
			}
			if (ModelsForUniqueCheck.Where(el => el.Name == CurrentModel.Name).Any())
				return new KeyValuePair<bool, string>(false, $"Расписание \"{CurrentModel.Name}\" уже присутствует в базе данных");

			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
