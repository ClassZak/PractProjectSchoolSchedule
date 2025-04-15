using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOTeacher : ADTO<Model.Teacher>
	{
		protected const string UNKNOWN_SUBJECT = "Неизвестен";
		public string SubjectLabel { get; set; }=UNKNOWN_SUBJECT;
		public DTOTeacher(Model.Teacher other) 
		{
			ModelRef = other;

			LoadSubjectLabel();
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return false;
		}
		void LoadSubjectLabel()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var el in ModelRef.Subject)
			{
				stringBuilder.Append(el.Name);
				if (!ReferenceEquals(el, ModelRef.Subject.ToList().Last()))
					stringBuilder.Append(", ");
			}
			string buildedString = stringBuilder.ToString();
			SubjectLabel = buildedString != string.Empty ? buildedString : UNKNOWN_SUBJECT;
		}
		protected override void LoadAllLabels(ref Model.SchoolScheduleEntities dataBase)
		{
		}
	}
}
