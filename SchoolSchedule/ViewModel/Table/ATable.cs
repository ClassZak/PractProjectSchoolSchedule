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
			App.Current.Dispatcher.Invoke(() =>
			{ 
				Entries.Clear();

				T[] array=new T[PreviousEntries.Count];
				PreviousEntries.ToList().CopyTo(array);
				for(int i = 0; i < array.Length; i++)
					Entries.Add(array[i]);
			});
		}
		public void SaveChanges()
		{
			App.Current.Dispatcher.Invoke(() =>
			{ 
				PreviousEntries.Clear();

				T[] array = new T[Entries.Count];
				Entries.ToList().CopyTo(array);
				for (int i = 0; i < array.Length; i++)
					PreviousEntries.Add(array[i]);
			});
		}
		public void Clear()
		{
			App.Current.Dispatcher.Invoke(() =>
			{ 
				if(Entries.Count > 0)
					SaveChanges();

				Entries.Clear();
			});
		}
		public void Remove(ICollection<T> values)
		{
			if (values == null)
				return;
			App.Current.Dispatcher.Invoke(() =>
			{ 
				if (values.Count == 0)
					return;
				SaveChanges();
				for(int i = 0; i < values.Count; ++i)
					Entries.Remove(values.ElementAt(i));
			});
		}

		bool Changed()
		{
			return PreviousEntries.Any();
		}
	}
}
