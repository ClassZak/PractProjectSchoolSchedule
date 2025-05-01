using SchoolSchedule.Model.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditScheduleViewModel : ABaseViewModel
	{
		public bool IsNewObject { get; set; } = true;
		public Model.Schedule CurrentSchedule { get; set; }
		public List<Model.Subject> Subjects { get; set; }
		public List<Model.Group> Groups{ get; set; }
		public List<Model.Teacher> Teachers { get; set; }
		public List<Model.BellSchedule> BellSchedules{ get; set; }

		public EditScheduleViewModel()
		{
			CurrentSchedule = new Model.Schedule();
		}

		public EditScheduleViewModel
		(
			bool isNewObject,
			Model.Schedule schedule,
			List<Model.Subject> subjects,
			List<Model.Group> groups,
			List<Model.Teacher> teachers,
			List<Model.BellSchedule> bellSchedules
		) : this()
		{
			IsNewObject = isNewObject;
			CurrentSchedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
			Subjects = new List<Model.Subject>(subjects);
			Groups = new List<Model.Group>(groups);
			Teachers = new List<Model.Teacher>(teachers);
			BellSchedules = new List<Model.BellSchedule>(bellSchedules);

			if (IsNewObject)
				CurrentSchedule.DayOfTheWeek = (int)DayOfTheWeek.Monday;
		}
	}
}
