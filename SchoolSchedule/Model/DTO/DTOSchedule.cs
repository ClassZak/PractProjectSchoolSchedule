using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SchoolSchedule.Model.DTO
{
	public class DTOSchedule : ADTO<Model.Schedule>
	{
		#region Свойства DTOSchedule
		#region Свойства Schedule
		public int Id { get => ModelRef.Id; set { _prevId = ModelRef.Id; ModelRef.Id = value; } }
		public int IdSubject { get => ModelRef.IdSubject; set { _prevIdSubject = ModelRef.IdSubject; ModelRef.IdSubject = value; } }
		public int IdGroup { get => ModelRef.IdGroup; set { _prevIdGroup = ModelRef.IdGroup; ModelRef.IdGroup = value; } }
		public int IdTeacher { get => ModelRef.IdTeacher; set { _prevIdTeacher = ModelRef.IdTeacher; ModelRef.IdTeacher = value; } }
		public int IdBellSchedule { get => ModelRef.IdBellSchedule; set { _prevIdBellSchedule = ModelRef.IdBellSchedule; ModelRef.IdBellSchedule = value; } }
		public int DayOfTheWeek { get => ModelRef.DayOfTheWeek; set { _prevDayOfTheWeek = ModelRef.DayOfTheWeek; ModelRef.DayOfTheWeek = value; } }
		public int ClassRoom { get => ModelRef.ClassRoom; set { _prevClassRoom = ModelRef.ClassRoom; ModelRef.ClassRoom = value; } }
		#endregion
		#region Свойства для дополнительной информации
		public string Subject{ get => ModelRef.Subject.ToString(); set { } }
		public string Group{ get => ModelRef.Group.ToString(); set { } }
		public int LessonNumber{ get => ModelRef.BellSchedule.LessonNumber;set{ } }
		public TimeSpan StartTime { get => ModelRef.BellSchedule.StartTime;set { } }
		public TimeSpan EndTime { get => ModelRef.BellSchedule.EndTime;set { } }
		public string BellScheduleType{ get => ModelRef.BellSchedule.BellScheduleType.ToString();set{ } }

		#endregion
		#region Поля предыдущих значений
		int _prevId = 0;
		int _prevIdSubject = 0;
		int _prevIdGroup = 0;
		int _prevIdTeacher = 0;
		int _prevIdBellSchedule = 0;
		int _prevDayOfTheWeek = 0;
		int _prevClassRoom = 0;
		#endregion

		public string TeacherSurname
		{
			get => ModelRef.Teacher.Surname; set { }
		}
		public string TeacherName
		{
			get => ModelRef.Teacher.Name; set { }
		}
		public string TeacherPatronymic
		{
			get => ModelRef.Teacher.Patronymic; set { }
		}
		#endregion
		static int _lastDTOId = 0;
		public DTOSchedule()
		{
			DTOId=++_lastDTOId;
		}
		public DTOSchedule(Model.Schedule other) : base(other)
		{
			_prevId = other.Id;
			_prevIdSubject=other.IdSubject;
			_prevIdGroup=other.IdGroup;
			_prevIdTeacher=other.IdTeacher;
			_prevIdBellSchedule=other.IdBellSchedule;
			_prevDayOfTheWeek=other.DayOfTheWeek;
			_prevClassRoom=other.ClassRoom;
		}
		public override bool HasReferenceOfNotExistingObject()
		{
			return ModelRef.IdTeacher == 0 /*|| ModelRef.IdLesson==0*/;
		}
		public override void Restore()
		{
			if(_prevId!=0)
				ModelRef.Id = _prevId;
			if( _prevIdSubject!=0)
				ModelRef.IdSubject= _prevIdSubject;
			if( _prevIdGroup!=0)
				ModelRef.IdGroup= _prevIdGroup;
			if( _prevIdTeacher!=0)
				ModelRef.IdTeacher= _prevIdTeacher;
			if( _prevIdBellSchedule!=0)
				ModelRef.IdBellSchedule= _prevIdBellSchedule;
			if(_prevDayOfTheWeek!=0)
				ModelRef.DayOfTheWeek= _prevDayOfTheWeek;
			if( _prevClassRoom!=0)
				ModelRef.ClassRoom= _prevClassRoom;
		}
	}
}
