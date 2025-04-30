using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOGroup : ADTO<Model.Group>
	{
		#region Свойства DTOGroup
		#region Свойства Group
		public int Id { get=>ModelRef.Id; set { _prevId = ModelRef.Id; ModelRef.Id = value; } }
		public int Year { get => ModelRef.Year; set { _prevYear = ModelRef.Year; ModelRef.Year = value; } }
		public string Name { get => ModelRef.Name; set { _prevName = ModelRef.Name; ModelRef.Name = value.ToUpper(); } }
		#endregion
		#region Поля предыдущих значений
		int _prevId = 0;
		int _prevYear = 0;
		string _prevName=null;
		#endregion
		#endregion
		static int _lastDTOId = 0;
		public DTOGroup() 
		{
			DTOId=++_lastDTOId;
		}
		public DTOGroup(Model.Group other) 
		{
			ModelRef=other;
			_prevId = other.Id;
			_prevYear = other.Year;
			_prevName = other.Name;
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return false;
		}
		public override void Restore()
		{
			if(_prevId!=0)
			{
				Id=_prevId;
				_prevId=0;
			}
			if(_prevYear!=0)
			{
				Year=_prevYear;
				_prevYear=0;
			}
			if (_prevName != null)
			{
				Name=_prevName;
				_prevName=null;
			}
		}
	}
}
