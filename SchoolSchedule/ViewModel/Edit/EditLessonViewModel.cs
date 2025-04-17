using SchoolSchedule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditLessonViewModel : ViewModel.ABaseViewModel
	{
		public Lesson CurrentLesson{ get; set; }
		public List<Group> Groups { get; set; }
		public List<Model.Subject> Subjects{ get; set; }
		public EditLessonViewModel() { }
		public EditLessonViewModel(Lesson lesson, List<Group> groups, List<Subject> subjects) : this()
		{
			CurrentLesson = lesson;
			Groups = new List<Group>(groups);
			Subjects = new List<Subject>(subjects);
		}
	}
}
