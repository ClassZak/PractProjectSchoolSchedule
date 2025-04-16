using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOSubject : ADTO<Model.Subject>
	{
		//Свойства от ссылки
		public string Name
		{
			get => ModelRef.Name;
			set => ModelRef.Name = value;
		}
		
		
		static int _lastDTOId = 0;
		public DTOSubject() 
		{
			DTOId=++_lastDTOId;
		}
		public DTOSubject(Model.Subject other)
		{
			base.ModelRef = other;
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return false;
		}
	}
}
