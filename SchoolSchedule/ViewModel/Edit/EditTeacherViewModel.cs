using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditTeacherViewModel : ABaseViewModel
	{
		public EditTeacherViewModel() { }
		public Model.Teacher CurrentTeacher { get; set; }
		public List<Model.Group> Groups { get; set; }
		public List<Model.Subject> Subjects{ get; set; }
		public ObservableCollection<Model.TeacherPhone> TeacherPhones { get; set; }

		public EditTeacherViewModel(Model.Teacher teacher, List<Model.Group> groups, List<Model.Subject> subjects, List<Model.TeacherPhone> teacherPhones)
		{
			CurrentTeacher = teacher;
			Groups = new List<Model.Group>(groups);
			Subjects = new List<Model.Subject>(subjects);
			if (teacherPhones == null)
				TeacherPhones = new ObservableCollection<Model.TeacherPhone>();
			else
				TeacherPhones = new ObservableCollection<Model.TeacherPhone>(teacherPhones);
		}

	}
}
