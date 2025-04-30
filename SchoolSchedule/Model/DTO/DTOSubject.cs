using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOSubject : ADTO<Model.Subject>
	{
		#region Свойства DTOSubject
		#region Свойства Subject
		public int Id { get => ModelRef.Id; set { ModelRef.Id = value; } }
		public string Name { get => ModelRef.Name; set { ModelRef.Name = value; } }
		#region Поля для предыдущих значений
		int _prevId = 0;
		string _prevName = null;
		#endregion
		#endregion
		#endregion


		static int _lastDTOId = 0;
		public DTOSubject() 
		{
			DTOId=++_lastDTOId;
		}
		public DTOSubject(Model.Subject other)
		{
			base.ModelRef = other;
			_prevId = other.Id;
			_prevName = other.Name;
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return false;
		}
		public override void Restore()
		{
			if (_prevId != 0)
				ModelRef.Id= _prevId;
			if(_prevName != null)
				ModelRef.Name= _prevName;
		}
	}
}
