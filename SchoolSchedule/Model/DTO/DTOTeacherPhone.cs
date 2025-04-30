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
		#region Свойства TeacherPhone
		public int Id {get=>ModelRef.Id; set=>ModelRef.Id=value; }
		public int IdTeacher{get => ModelRef.IdTeacher;set=>ModelRef.IdTeacher=value;}
		public string PhoneNumber{get=>ModelRef.PhoneNumber;set=>ModelRef.PhoneNumber=value;}
		#endregion
		#region Поля для предыдущих значений
		int _prevId = 0;
		int _prevIdTeacher = 0;
		string _prevPhoneNumber = null;
		#endregion
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

			_prevId = other.Id;
			_prevIdTeacher=other.IdTeacher;
			_prevPhoneNumber = other.PhoneNumber;
		}
		public override bool HasReferenceOfNotExistingObject()
		{
			return ModelRef.IdTeacher == 0;
		}

		public override string ToString()
		{
			return ModelRef.ToString();
		}
		public override void Restore()
		{
			if(_prevId!=0)
			{
				ModelRef.Id=_prevId;
				_prevId=0;
			}
			if(_prevIdTeacher!=0)
			{
				ModelRef.IdTeacher = _prevIdTeacher;
				_prevIdTeacher = 0;
			}
			if(_prevPhoneNumber!=null)
			{
				ModelRef.PhoneNumber = _prevPhoneNumber;
				_prevPhoneNumber = null;
			}
		}
	}
}
