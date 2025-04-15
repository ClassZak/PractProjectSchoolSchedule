using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public abstract class ADTO<T>
	where T : class, new()
	{
		public T ModelRef { get; set; } = new T();
		public int DTOId { get; set; } = 0;
		protected abstract void LoadAllLabels(ref Model.SchoolScheduleEntities dataBase);

		public bool ExistsInDataBase()
		{
			return DTOId != 0;
		}
		public abstract bool HasReferenceOfNotExistingObject();
	}
}
