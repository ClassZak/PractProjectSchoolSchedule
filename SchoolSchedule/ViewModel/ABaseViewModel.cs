using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel
{
	public abstract class ABaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name=null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		protected virtual bool SetPropertyChanged<T>(ref T source, T value, [CallerMemberName] string propertyName=null)
		{
			if (EqualityComparer<T>.Default.Equals(source, value))
				return false;

			source = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
