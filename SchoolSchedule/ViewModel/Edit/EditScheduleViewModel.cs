using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditScheduleViewModel : ABaseViewModel
	{
		public Model.Schedule CurrentSchedule { get; set; }
		public List<Model.Lesson> Lessons { get; set; }
		public List<Model.Teacher> Teachers { get; set; }

		public EditScheduleViewModel()
		{
			// Инициализация по умолчанию
			CurrentSchedule = new Model.Schedule
			{
				Date = DateTime.Today,
				StartTime = new TimeSpan(9, 0, 0),
				EndTime = new TimeSpan(10, 0, 0)
			};
		}

		public EditScheduleViewModel
		(
			Model.Schedule schedule,
			List<Model.Lesson> lessons,
			List<Model.Teacher> teachers
		) : this()
		{
			CurrentSchedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
			Lessons = new List<Model.Lesson>(lessons);
			Teachers = new List<Model.Teacher>(teachers);
		}
	}
}
