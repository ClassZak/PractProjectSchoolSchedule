using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public abstract class ADTO<T> : IDTO
	where T : class, new()
	{
		public T ModelRef { get; set; } = new T();
		public int DTOId { get; set; } = 0;


		protected ADTO(){}
		public ADTO(T modelRef)
		{
			ModelRef = modelRef;
		}

		public bool ExistsInDataBase()
		{
			return DTOId != 0;
		}
		public abstract bool HasReferenceOfNotExistingObject();
		public abstract void Restore();
	}
}
