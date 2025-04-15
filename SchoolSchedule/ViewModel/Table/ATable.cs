using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Table
{
	public abstract class ATable<T> where T : class
	{

		public ObservableCollection<T> Entries { get; set; } = new ObservableCollection<T>();
		public ObservableCollection<T> PreviousEntries { get; set; } = new ObservableCollection<T>();

		public virtual void Update(ref SchoolSchedule.Model.SchoolScheduleEntities dataBaseRef)
		{
		}
		public void CancelChanges()
		{
			Entries.Clear();

			T[] array=new T[PreviousEntries.Count];
			PreviousEntries.ToList().CopyTo(array);
			for(int i = 0; i < array.Length; i++)
				Entries.Add(array[i]);
		}
		public void Clear()
		{
			if(Entries.Count > 0)
			{
				T[] array = new T[Entries.Count];
				Entries.ToList().CopyTo(array);
				for (int i = 0; i < array.Length; i++)
					PreviousEntries.Add(array[i]);
			}

			Entries.Clear();
		}

		bool Changed()
		{
			return PreviousEntries.Any();
		}
	}
}
