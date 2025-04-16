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
		public int Id
		{ 
			get=>ModelRef.Id; set => ModelRef.Id = value; 
		}
		public int IdLesson
		{
			get => ModelRef.IdLesson; set => ModelRef.IdLesson = value;
		}
		public int IdTeacher
		{
			get => ModelRef.IdTeacher; set => ModelRef.IdTeacher = value;
		}
		public TimeSpan StartTime
		{
			get => ModelRef.StartTime; set => ModelRef.StartTime = value;
		}
		public TimeSpan EndTime
		{
			get => ModelRef.EndTime; set => ModelRef.EndTime = value;
		}
		public DateTime Date
		{
			get => ModelRef.Date; set => ModelRef.Date = value;
		}


		public int LessonNumber
		{
			get => ModelRef.Lesson.Number; set { }
		}
		public string Subject
		{
			get => ModelRef.Lesson.Subject.Name; set { }
		}
		public string Group
		{
			get => $"{ModelRef.Lesson.Group.Year}{ModelRef.Lesson.Group.Name}"; set { }
		}

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
		public DTOSchedule(Model.Schedule schedule) : base(schedule){}
		public override bool HasReferenceOfNotExistingObject()
		{
			return ModelRef.IdTeacher == 0 || ModelRef.IdLesson==0;
		}
	}
}
