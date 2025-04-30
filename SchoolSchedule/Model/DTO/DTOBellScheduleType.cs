using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOBellScheduleType : ADTO<Model.BellScheduleType>
	{
		#region Свойства DTOBellScheduleType 
		#region Свойства BellScheduleType 
		public int Id{ get => ModelRef.Id; set { _prevId = Id;ModelRef.Id = value; } }
		public string Name{ get => ModelRef.Name; set { _prevName = Name;ModelRef.Name = value; } }
		#endregion
		#region Поля для предыдущих значений
		int _prevId = 0;
		string _prevName = null;
		#endregion
		#endregion

		static int _lastDTOId = 0;
		public DTOBellScheduleType()
		{
			DTOId = ++_lastDTOId;
		}
		public DTOBellScheduleType(BellScheduleType modelRef) : base(modelRef)
		{
			_prevId=modelRef.Id;
			_prevName=modelRef.Name;
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return false;
		}

		public override void Restore()
		{
			if (_prevId!=0)
				ModelRef.Id = _prevId;
			if(_prevName!=null)
				ModelRef.Name = _prevName;
		}
	}
}
