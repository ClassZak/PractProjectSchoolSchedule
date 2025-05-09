using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Event
{
	// Базовый класс для всех событий обновления таблиц
	public abstract class TableUpdatedEventArgs : EventArgs
	{
		public abstract Type TableType { get; }
	}

	// Классы для конкретных типов данных
	public class TeachersUpdatedEventArgs : TableUpdatedEventArgs
	{
		public IEnumerable<Model.Teacher> NewValues { get; }

		public override Type TableType => typeof(Model.Teacher);

		public TeachersUpdatedEventArgs(IEnumerable<Model.Teacher> newValues)
		{
			NewValues = newValues;
		}
	}

	public class GroupsUpdatedEventArgs : TableUpdatedEventArgs
	{
		public IEnumerable<Model.Group> NewValues { get; }

		public override Type TableType => typeof(Model.Group);

		public GroupsUpdatedEventArgs(IEnumerable<Model.Group> newValues)
		{
			NewValues = newValues;
		}
	}

	public class BellScheduleTypesUpdatedEventArgs : TableUpdatedEventArgs
	{
		public IEnumerable<Model.BellScheduleType> NewValues { get; }

		public override Type TableType => typeof(Model.BellScheduleType);

		public BellScheduleTypesUpdatedEventArgs(IEnumerable<Model.BellScheduleType> newValues)
		{
			NewValues = newValues;
		}
	}
}
