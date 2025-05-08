using SchoolSchedule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditBellScheduleViewModel : ABaseViewModel
	{
		public bool ObjectIsNew {  get; set; }=true;
		public Model.BellSchedule CurrentModel { get; set; } = new Model.BellSchedule
		{
			StartTime = new TimeSpan(8, 0, 0),
			EndTime = new TimeSpan(8, 40, 0)
		};
		#region Свойства для ввода
		public List<Model.BellSchedule> ModelsForUniqueCheck { get; set; }
		public int IdBellScheduleType { get => CurrentModel.IdBellScheduleType; set => CurrentModel.IdBellScheduleType = value; }
		public int LessonNumber { get => CurrentModel.LessonNumber; set 
			{ 
				CurrentModel.LessonNumber = value;
				if (CurrentModel.LessonNumber < 1)
					CurrentModel.LessonNumber = 1;
				else if (CurrentModel.LessonNumber > 8)
					CurrentModel.LessonNumber = 8;

				OnPropertyChanged(nameof(LessonNumber));
			}
		}
		public System.TimeSpan StartTime { get => CurrentModel.StartTime; set 
			{
				CurrentModel.StartTime = value;
				if(CurrentModel.StartTime > CurrentModel.EndTime)
					CurrentModel.StartTime=CurrentModel.EndTime - new TimeSpan(0,1,0);

				OnPropertyChanged(nameof(StartTime));
			}
		}
		public System.TimeSpan EndTime { get => CurrentModel.EndTime; set 
			{ 
				CurrentModel.EndTime = value; 
				if(CurrentModel.EndTime < CurrentModel.StartTime)
					CurrentModel.EndTime = CurrentModel.StartTime + new TimeSpan(0,1,0);

				OnPropertyChanged(nameof(EndTime));
			} 
		}
		#endregion
		#region Свойства для выбора
		public List<Model.BellScheduleType> BellScheduleTypes { get; set; }
		#endregion

		public EditBellScheduleViewModel() {}

		public EditBellScheduleViewModel
		(
			Model.BellSchedule model,
			List<Model.BellSchedule> modelsForUniqueCheck,
			List<Model.BellScheduleType> bellScheduleTypes,
			bool objectIsNew
		) : this()
		{
			ObjectIsNew = objectIsNew;
			if (bellScheduleTypes.Count == 0)
				throw new ArgumentException("В базе данных нет ни одного расписания звонков.\nДобавьте новое расписание звонков во вкладке \"Типы расписаний звонков\"");
			CurrentModel = model ?? new BellSchedule { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(8, 40, 0), LessonNumber = 1 };
			BellScheduleTypes = new List<Model.BellScheduleType>(bellScheduleTypes);

			if (ObjectIsNew)
			{
				CurrentModel.StartTime = new TimeSpan(8, 0, 0);
				CurrentModel.EndTime = new TimeSpan(8, 40, 0);
				OnPropertyChanged(nameof(StartTime));
				OnPropertyChanged(nameof(EndTime));

				LessonNumber = 1;
				CurrentModel.IdBellScheduleType=BellScheduleTypes.First().Id;
				OnPropertyChanged(nameof(IdBellScheduleType));
			}

			ModelsForUniqueCheck = new List<BellSchedule>(modelsForUniqueCheck);
			if (!ObjectIsNew)
				ModelsForUniqueCheck.Remove(model);
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			LessonNumber = LessonNumber;
			if (ObjectIsNew && ModelsForUniqueCheck.Where(x => x.LessonNumber == CurrentModel.LessonNumber && x.IdBellScheduleType == CurrentModel.IdBellScheduleType).Any())
				return new KeyValuePair<bool, string>(false, $"Расписание \"{BellScheduleTypes.FirstOrDefault(x => x.Id == CurrentModel.IdBellScheduleType)?.Name}\" для {CurrentModel.LessonNumber} урока уже есть в базе данных");

			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
