using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public List<Model.BellScheduleType> BellScheduleTypes { get; set; }


		public List<Model.BellSchedule> ModelsForUniqueCheck { get; set; }
		public int IdBellScheduleType { get => CurrentModel.IdBellScheduleType; set => CurrentModel.IdBellScheduleType = value; }
		public int LessonNumber{ get => CurrentModel.LessonNumber; set=>CurrentModel.LessonNumber = value; }
		public System.TimeSpan StartTime{ get => CurrentModel.StartTime; set => CurrentModel.StartTime = value;}
		public System.TimeSpan EndTime{ get=>CurrentModel.EndTime; set => CurrentModel.EndTime = value;}


		public EditBellScheduleViewModel() {}

		public EditBellScheduleViewModel
		(
			bool objectIsNew,
			Model.BellSchedule bellSchedule,
			List<Model.BellScheduleType> bellScheduleTypes
		) : this()
		{
			ObjectIsNew = objectIsNew;
			CurrentModel = bellSchedule ?? new Model.BellSchedule();
			BellScheduleTypes = new List<Model.BellScheduleType>(bellScheduleTypes);
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (ObjectIsNew && ModelsForUniqueCheck.Where(x => x.LessonNumber == CurrentModel.LessonNumber && x.IdBellScheduleType == CurrentModel.IdBellScheduleType).Any())
				return new KeyValuePair<bool, string>(false, $"Расписание \"{BellScheduleTypes.FirstOrDefault(x => x.Id == CurrentModel.IdBellScheduleType)?.Name}\" для {CurrentModel.LessonNumber} урока уже есть в базе данных");

			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
