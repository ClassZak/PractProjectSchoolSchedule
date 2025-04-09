using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel
{
	internal class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}


		public MainViewModel()
		{
			using(ClassesScheduleEntities dataBase=new ClassesScheduleEntities())
			{
				_groups = new ObservableCollection<Group>(dataBase.Groups.ToList());
			}
		}

		private ObservableCollection<Group> _groups;
		public ObservableCollection<Group> Groups
		{
			get
			{
				return _groups;
			}
			set{ }
		}
	}
}
