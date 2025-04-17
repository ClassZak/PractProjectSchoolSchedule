using SchoolSchedule.Model;
using SchoolSchedule.ViewModel.Comands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditStudentViewModel : ViewModel.ABaseViewModel
	{
		public EditStudentViewModel() { }	

		public Student CurrentStudent { get; set; }
		public ObservableCollection<Group> Groups { get; set; }

		public EditStudentViewModel(Student student, List<Group> groups)
		{
			CurrentStudent = student;
			Groups = new ObservableCollection<Group>(groups);
		}
	}
}
