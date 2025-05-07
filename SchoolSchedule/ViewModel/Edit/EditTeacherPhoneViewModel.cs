using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditTeacherPhoneViewModel : ABaseViewModel
	{
		public EditTeacherPhoneViewModel() { }

		public Model.TeacherPhone CurrentModel { get; set; } = new Model.TeacherPhone { PhoneNumber= "+7 123 456-78-90" };
		#region Свойства для ввода
		public string PhoneNumber { get => CurrentModel?.PhoneNumber; set 
			{
				string inputValue = value?.Trim() ?? "";

				if (inputValue.Length < 2)
				{
					CurrentModel.PhoneNumber = "+7 ";
					return;
				}

				string rawDigits = Regex.Replace(inputValue.Substring(2), @"[^\d]", "");

				rawDigits = rawDigits.Length >= 10 ? rawDigits.Substring(0, 10) : rawDigits.PadRight(10, '0');

				CurrentModel.PhoneNumber = $"+7 {rawDigits.Substring(0, 3)} " + $"{rawDigits.Substring(3, 3)}-" + $"{rawDigits.Substring(6, 2)}-" + $"{rawDigits.Substring(8, 2)}";
				OnPropertyChanged(nameof(PhoneNumber));
			}
		}
		#endregion

		public List<Model.TeacherPhone> ModelsForUniqueCheck { get; set; }
		public EditTeacherPhoneViewModel(Model.TeacherPhone currentModel, List<Model.TeacherPhone> modelsForUniqueCheck)
		{
			CurrentModel = currentModel;
			ModelsForUniqueCheck = modelsForUniqueCheck;

			// Так надо
			if (PhoneNumber==null)
				PhoneNumber = PhoneNumber;
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			CurrentModel.PhoneNumber = CurrentModel.PhoneNumber.Trim();
			if (string.IsNullOrWhiteSpace(CurrentModel.PhoneNumber) || CurrentModel.PhoneNumber == string.Empty)
			{
				PhoneNumber = "+7 123 456-78-90";
				OnPropertyChanged(PhoneNumber);
				return new KeyValuePair<bool, string>(false, "Введите номер телефона!");
			}
			if (!Regex.IsMatch(CurrentModel.PhoneNumber, @"^\+7 \d{3} \d{3}-\d{2}-\d{2}$"))
				return new KeyValuePair<bool, string>(false, "Неверный формат номера телефона!\nПравильный формат: +7 XXX XXX-XX-XX");
			if (ModelsForUniqueCheck.Any(x=>x.PhoneNumber==CurrentModel.PhoneNumber))
				return new KeyValuePair<bool, string>(false, $"Номер {CurrentModel.PhoneNumber} уже существует");
				

			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
