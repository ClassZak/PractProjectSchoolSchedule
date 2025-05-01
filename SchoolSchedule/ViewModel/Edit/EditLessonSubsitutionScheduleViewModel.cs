using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditLessonSubsitutionScheduleViewModel : ABaseViewModel
	{
		public bool IsNewObject{  get; set; }=true;
		public Model.LessonSubsitutionSchedule CurrentLessonSubsitutionSchedule { get; set; }
		public List<Model.Subject> Subjects { get; set; }
		public List<Model.Group> Groups { get; set; }
		public List<Model.Teacher> Teachers{ get; set; }

		public EditLessonSubsitutionScheduleViewModel()
		{
		}

		public EditLessonSubsitutionScheduleViewModel
		(
			bool isNewObject,
			Model.LessonSubsitutionSchedule  lessonSubsitutionSchedule,
			List<Model.Subject> subjects,
			List<Model.Group> groups,
			List<Model.Teacher> teachers
		) : this()
		{
			IsNewObject = isNewObject;
			CurrentLessonSubsitutionSchedule = lessonSubsitutionSchedule ?? throw new ArgumentNullException(nameof(lessonSubsitutionSchedule));
			Subjects = new List<Model.Subject>(subjects);
			Groups = new List<Model.Group>(groups);
			Teachers = new List<Model.Teacher>(teachers);

			if (IsNewObject)
				CurrentLessonSubsitutionSchedule.Date = DateTime.Today;
		}
	}
}
