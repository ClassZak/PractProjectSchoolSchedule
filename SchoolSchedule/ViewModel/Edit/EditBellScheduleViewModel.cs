using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditBellScheduleViewModel : ABaseViewModel
	{
		public bool IsNewObject{  get; set; }=true;
		public Model.BellSchedule CurrentBellSchedule { get; set; }
		public List<Model.BellScheduleType> BellScheduleTypes { get; set; }

		public EditBellScheduleViewModel()
		{
			// Инициализация по умолчанию
			CurrentBellSchedule = new Model.BellSchedule
			{
				StartTime = new TimeSpan(8, 0, 0),
				EndTime = new TimeSpan(8, 40, 0)
			};
		}

		public EditBellScheduleViewModel
		(
			bool isNewObject,
			Model.BellSchedule bellSchedule,
			List<Model.BellScheduleType> bellScheduleTypes
		) : this()
		{	
			IsNewObject = isNewObject;
			CurrentBellSchedule = bellSchedule ?? throw new ArgumentNullException(nameof(bellSchedule));
			BellScheduleTypes = new List<Model.BellScheduleType>(bellScheduleTypes);
		}
	}
}
