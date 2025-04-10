using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel
{
	public class MainViewModel : ABaseViewModel
	{
		public MainViewModel()
		{
			using(Model.SchoolScheduleEntities dataBase=new Model.SchoolScheduleEntities())
			{
				_groups = new ObservableCollection<Model.Group>(dataBase.Groups.ToList());
			}
		}

		private ObservableCollection<Model.Group> _groups;
		public ObservableCollection<Model.Group> Groups
		{
			get {return _groups;}
			set {SetPropertyChanged(ref _groups, value);}
		}
	}
}
