using SchoolSchedule.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Table
{
	public class TemplateTable<T> : ATable<T> where T : class, new()
	{
		public override void CancelChanges()
		{
			base.CancelChanges();
			App.Current.Dispatcher.Invoke(() =>
			{
				foreach(var el in Entries)
					(el as IDTO).Restore();
			});
		}
	}
}
