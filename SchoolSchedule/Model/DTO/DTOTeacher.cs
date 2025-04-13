using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOTeacher : Model.Teacher, IDTO
	{
		protected const string UNKNOWN_SUBJECT = "Неизвестен";
		public string SubjectLabel { get; set; }=UNKNOWN_SUBJECT;
		public DTOTeacher()
		{ 
			Id = 0; 
		}
		public DTOTeacher(Model.Teacher teacher)
		{
			Id = teacher.Id;
			Surname = teacher.Surname;
			Name = teacher.Name;
			Patronymic = teacher.Patronymic;

			SpecialityTeacher = teacher.SpecialityTeacher;
			Schedule=teacher.Schedule;
			TeacherPhone=teacher.TeacherPhone;

			LoadAllLabels();
		}
		protected void LoadSubjectLabel()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var el in SpecialityTeacher)
			{
				stringBuilder.Append(el.Subject.Name);
				if(!ReferenceEquals(el, SpecialityTeacher.ToList().Last()))
					stringBuilder.Append(", ");
			}
			string buildedString=stringBuilder.ToString();
			SubjectLabel = buildedString!=string.Empty ? buildedString : UNKNOWN_SUBJECT;
		}
		public void LoadAllLabels()
		{
			LoadSubjectLabel();
		}
	}
}
