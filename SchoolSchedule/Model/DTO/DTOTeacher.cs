using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOTeacher : ADTO<Model.Teacher>
	{
		#region Свойства DTOTeacher
		public int Id
		{
			get=>ModelRef.Id; set=>ModelRef.Id = value;	
		}
		public string Surname
		{
			get => ModelRef.Surname; set => ModelRef.Surname= value;
		}
		public string Name
		{
			get => ModelRef.Name; set => ModelRef.Name = value;
		}
		public string Patronymic
		{
			get => ModelRef.Patronymic; set => ModelRef.Patronymic = value;
		}


		#endregion


		protected const string UNKNOWN= "Неизвестно";
		public string Subject { get; set; }=UNKNOWN;
		public string Group{  get; set; }=UNKNOWN;
		public string PhoneNumber{  get; set; }=UNKNOWN;

		static int _lastDTOId = 0;
		public DTOTeacher()
		{
			DTOId = ++_lastDTOId;
		}
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
			Subject = buildedString != string.Empty ? buildedString : UNKNOWN;
			
			stringBuilder.Clear();
			foreach(var el in ModelRef.Group)
			{
				stringBuilder.Append(el.Name);
				if (!ReferenceEquals(el, ModelRef.Group.ToList().Last()))
					stringBuilder.Append(", ");
			}
			buildedString = stringBuilder.ToString();
			Group = buildedString != string.Empty ? buildedString : UNKNOWN;

			stringBuilder.Clear();
			foreach(var el in ModelRef.TeacherPhone)
			{
				stringBuilder.Append(el.PhoneNumber);
				if (!ReferenceEquals(el, ModelRef.TeacherPhone.ToList().Last()))
					stringBuilder.Append(", ");
			}
			buildedString = stringBuilder.ToString();
			PhoneNumber = buildedString != string.Empty ? buildedString : UNKNOWN;
		}
	}
}
