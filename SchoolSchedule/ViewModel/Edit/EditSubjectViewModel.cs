using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditSubjectViewModel : ABaseViewModel
	{
		public EditSubjectViewModel() { }

		public Model.Subject CurrentModel { get; set; } = new Model.Subject { Name = "Предмет"};
		public List<Model.Subject> ModelsForUniqueCheck{ get; set; }
		#region Свойства для ввода
		const int MAX_NAME_LENGTH = 70;
		public string Name{ get { return CurrentModel.Name; } set
			{ 
				string inputValue = value is null ? "Предмет" :  Regex.Replace(value.Trim(), @"[^\p{IsCyrillic}\s\-\d]","");
				if(string.IsNullOrEmpty(inputValue) )
					CurrentModel.Name = "Предмет";
				else
					CurrentModel.Name = inputValue.Length > MAX_NAME_LENGTH ?
					inputValue.Substring(0,MAX_NAME_LENGTH) : inputValue;
				
				OnPropertyChanged(nameof(Name));
			} 
		}
		#endregion


		public EditSubjectViewModel(Model.Subject model, List<Model.Subject> modelsForUniqueCheck)
		{
			CurrentModel = model;
			ModelsForUniqueCheck = modelsForUniqueCheck;

			// Так надо
			if (Name is null)
				Name = Name;
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (CurrentModel.Name != null)
				CurrentModel.Name = CurrentModel.Name.Trim();

			if (string.IsNullOrWhiteSpace(CurrentModel.Name))
			{
				Name = "Предмет";
				OnPropertyChanged(nameof(Name));
				return new KeyValuePair<bool, string>(false, "Введите не пустое значение для названия предмета");
			}
			if (ModelsForUniqueCheck.Where(el => el.Name == CurrentModel.Name).Any())
				return new KeyValuePair<bool, string>(false, $"Предмет \"{CurrentModel.Name}\" уже присутствует в базе данных");

			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
