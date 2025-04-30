using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOBellSchedule : ADTO<Model.BellSchedule>
	{
		#region Свойства DTOBellSchedule
		#region Свойства BellSchedule
		public int Id { get => ModelRef.Id; set { _prevId = Id; ModelRef.Id = value; } }
		public int IdBellScheduleType { get => ModelRef.IdBellScheduleType; set { _prevIdBellScheduleType = IdBellScheduleType; ModelRef.IdBellScheduleType= value; } }
		public int LessonNumber { get => ModelRef.LessonNumber; set { _prevLessonNumber= LessonNumber; ModelRef.LessonNumber= value; } }
		public TimeSpan StartTime { get => ModelRef.StartTime; set { _prevStartTime = StartTime; ModelRef.StartTime = value; } }
		public TimeSpan EndTime{ get => ModelRef.EndTime; set { _prevEndTime= StartTime; ModelRef.EndTime= value; } }
		#endregion
		#region Поля для предыдущих значений
		int _prevId = 0;
		int _prevIdBellScheduleType = 0;
		int _prevLessonNumber = 0;
		TimeSpan _prevStartTime = TimeSpan.MinValue;
		TimeSpan _prevEndTime = TimeSpan.MinValue;
		#endregion
		#endregion
		static int _lastDTOId = 0;
		public DTOBellSchedule()
		{
			DTOId = ++_lastDTOId;
		}
		public DTOBellSchedule(Model.BellSchedule other) : base(other)
		{
			_prevId = other.Id;
			_prevIdBellScheduleType = other.IdBellScheduleType;
			_prevLessonNumber= other.LessonNumber;
			_prevStartTime= other.StartTime;
			_prevEndTime= other.EndTime;
		}


		public override bool HasReferenceOfNotExistingObject()
		{
			return IdBellScheduleType == 0;
		}
		public override void Restore()
		{
			if(_prevId!=0)
				ModelRef.Id = _prevId;
			if(_prevIdBellScheduleType!=0)
				ModelRef.IdBellScheduleType= _prevIdBellScheduleType;
			if(_prevLessonNumber!=0)
				ModelRef.LessonNumber=_prevLessonNumber;
			if(_prevStartTime!=TimeSpan.MinValue)
				ModelRef.StartTime=_prevStartTime;
			if(_prevEndTime!=TimeSpan.MaxValue)
				ModelRef.EndTime=_prevEndTime;
		}
	}
}
