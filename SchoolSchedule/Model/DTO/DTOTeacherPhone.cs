using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOTeacherPhone : ADTO<Model.TeacherPhone>
	{
		#region Свойства DTOTeacherPhone
		public int Id 
		{
			get=>ModelRef.Id; set=>ModelRef.Id=value; 
		}
		public int IdTeacher
		{
			get => ModelRef.IdTeacher;set=>ModelRef.IdTeacher=value;
		}
		public string PhoneNumber
		{
			get=>ModelRef.PhoneNumber;set=>ModelRef.PhoneNumber=value;
		}
		#endregion
		static int _lastDTOId = 0;
		public DTOTeacherPhone()
		{
			DTOId = ++_lastDTOId;
		}
		public DTOTeacherPhone(Model.TeacherPhone other)
		{
			ModelRef = other;
			if(ModelRef.Id==0)
				DTOId = ++_lastDTOId;
		}
		public override bool HasReferenceOfNotExistingObject()
		{
			return ModelRef.IdTeacher == 0;
		}

		public override string ToString()
		{
			return ModelRef.ToString();
		}
	}
}
