using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOLessonSubsitutionSchedule : ADTO<Model.LessonSubsitutionSchedule>
	{
		#region Свойства DTOLessonSubsitutionSchedule
		#region Свойства LessonSubsitutionSchedule
		public int Id{  get=>ModelRef.Id; set { _prevId = ModelRef.Id;ModelRef.Id = value; } }
		public DateTime Date{ get => ModelRef.Date;set { _prevDate = ModelRef.Date; ModelRef.Date = value; } }
		public int? IdSubject {  get=>ModelRef.IdSubject; set { _prevIdSubject = ModelRef.IdSubject; ModelRef.IdSubject = value; } }
		public int? IdGroup {  get=>ModelRef.IdGroup; set { _prevIdGroup = ModelRef.IdGroup; ModelRef.IdGroup = value; } }
		public int? IdTeacher {  get=>ModelRef.Id; set { _prevIdTeacher = ModelRef.IdTeacher; ModelRef.IdTeacher = value; } }
		public int? ClassRoom{  get=>ModelRef.ClassRoom; set { _prevClassRoom = ModelRef.ClassRoom; ModelRef.ClassRoom = value; } }
		public int LessonNumber{  get=>ModelRef.LessonNumber; set { _prevLessonNumber = ModelRef.LessonNumber; ModelRef.LessonNumber= value; } }
		#endregion
		#region Свойства для дополнительной информации
		public string Subject { get => ModelRef.Subject.ToString(); set { } }
		public string Group{ get => ModelRef.Group.ToString(); set { } }


		public string TeacherSurname{ get => ModelRef.Teacher.Surname;set { } }
		public string TeacherName{ get => ModelRef.Teacher.Name;set { } }
		public string TeacherPatronymic { get => ModelRef.Teacher.Patronymic;set { } }
		#endregion
		#region Поля для предыдущих значений
		int _prevId = 0;
		DateTime _prevDate = DateTime.MinValue;
		int? _prevIdSubject = 0;
		int? _prevIdGroup = 0;
		int? _prevIdTeacher = 0;
		int? _prevClassRoom = 0;
		int _prevLessonNumber= 0;
		#endregion
		#endregion
		static int _lastId = 0;
		public DTOLessonSubsitutionSchedule()
		{
			DTOId = ++_lastId;
		}
		public DTOLessonSubsitutionSchedule(LessonSubsitutionSchedule modelRef) : base(modelRef)
		{
			_prevId = modelRef.Id;
			_prevDate=modelRef.Date;
			_prevIdSubject=modelRef.IdSubject;
			_prevIdGroup=modelRef.IdGroup;
			_prevIdTeacher=modelRef.IdTeacher;
			_prevClassRoom = modelRef.ClassRoom;
			_prevLessonNumber=modelRef.LessonNumber;
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return 
				(_prevIdSubject != null && ((int)_prevIdSubject) != 0) || 
				(_prevIdGroup != null && ((int)_prevIdGroup) != 0) || 
				(_prevIdTeacher != null && ((int)_prevIdTeacher) != 0);
		}

		public override void Restore()
		{
			if(_prevId!=0)
				ModelRef.Id=_prevId;
			if(_prevDate!=DateTime.MinValue)
				ModelRef.Date=_prevDate;
			if (_prevIdSubject != null)
				ModelRef.IdSubject = _prevIdSubject;
			if (_prevIdGroup != null)
				ModelRef.IdGroup = _prevIdGroup;
			if(_prevIdTeacher!=null)
				ModelRef.IdTeacher=_prevIdTeacher.Value;
		}
	}
}
