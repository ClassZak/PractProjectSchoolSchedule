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
		public int Id { get => ModelRef.Id; set { _prevId = ModelRef.Id; ModelRef.Id = value; } }
		//public int IdLesson { get => ModelRef.IdLesson; set { _prevIdLesson = ModelRef.IdLesson; ModelRef.IdLesson = value; } }
		public int IdTeacher { get => ModelRef.IdTeacher; set { _prevIdTeacher = ModelRef.IdTeacher; ModelRef.IdTeacher = value; } }
		//public TimeSpan StartTime { get => ModelRef.StartTime; set { _prevStartTime = ModelRef.StartTime; ModelRef.StartTime = value; } }
		//public TimeSpan EndTime { get => ModelRef.EndTime; set { _prevEndTime = ModelRef.EndTime; ModelRef.EndTime = value; } }
		//public DateTime Date { get => ModelRef.Date; set { _prevDate = ModelRef.Date; ModelRef.Date = value; } }

		#region Поля предыдущих значений
		int _prevId = 0;
		int _prevIdLesson = 0;
		int _prevIdTeacher = 0;
		TimeSpan _prevStartTime=TimeSpan.MinValue;
		TimeSpan _prevEndTime=TimeSpan.MinValue;
		DateTime _prevDate=DateTime.MinValue;
		#endregion
		//public int LessonNumber
		//{
		//	get => ModelRef.Lesson.Number; set { }
		//}
		//public string Subject
		//{
		//	get => ModelRef.Lesson.Subject.Name; set { }
		//}
		//public string Group
		//{
		//	get => $"{ModelRef.Lesson.Group.Year}{ModelRef.Lesson.Group.Name}"; set { }
		//}

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
			//_prevIdLesson=other.IdLesson;
			//_prevIdTeacher=other.IdTeacher;
			//_prevStartTime = other.StartTime;
			//_prevEndTime = other.EndTime;
			//_prevDate = other.Date;
		}
		public override bool HasReferenceOfNotExistingObject()
		{
			return ModelRef.IdTeacher == 0 /*|| ModelRef.IdLesson==0*/;
		}
		public override void Restore()
		{
			if(_prevId!=0)
				ModelRef.Id = _prevId;
			//if(_prevIdLesson!=0)
			//	ModelRef.IdLesson= _prevIdLesson;
			//if( _prevIdTeacher!=0)
			//	ModelRef.IdTeacher= _prevIdTeacher;
			//if(_prevStartTime != TimeSpan.MinValue)
			//	ModelRef.StartTime=_prevStartTime;
			//if(_prevEndTime != TimeSpan.MinValue)
			//	ModelRef.EndTime =_prevEndTime;
			//if(_prevDate!=DateTime.MinValue)
			//	ModelRef.Date = _prevDate;
		}
	}
}
