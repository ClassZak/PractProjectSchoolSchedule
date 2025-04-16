using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOGroup : ADTO<Model.Group>
	{
		public int Year
		{
			get => ModelRef.Year;
			set => ModelRef.Year = value;
		}

		public string Name
		{
			get => ModelRef.Name;
			set => ModelRef.Name = value.ToUpper();
		}

		static int _lastDTOId = 0;
		public DTOGroup() 
		{
			DTOId=++_lastDTOId;
		}
		public DTOGroup(Model.Group other) 
		{
			ModelRef=other;
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return false;
		}
	}
}
