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
		#region Свойства Teacher
		public int Id { get => ModelRef.Id; set { ModelRef.Id = value; } }
		public string Surname { get => ModelRef.Surname; set { ModelRef.Surname = value; } }
		public string Name { get => ModelRef.Name; set { ModelRef.Name = value; } }
		public string Patronymic { get => ModelRef.Patronymic; set { ModelRef.Patronymic = value; } }
		public DateTime BirthDay{ get { return ModelRef.BirthDay; } set { _prevBirthDay = ModelRef.BirthDay; ModelRef.BirthDay = value; } }
		public string Gender { get { return ModelRef.Gender; } set { _prevGender = ModelRef.Gender; ModelRef.Gender = value.Substring(0, 1).ToUpper(); } }
		#endregion
		#region Поля для предыдущих значний
		int _prevId = 0;
		private string _prevSurname = null;
		private string _prevName = null;
		private string _prevPatronymic = null;
		private DateTime _prevBirthDay = DateTime.MinValue;
		private string _prevGender = null;
		#endregion
		#endregion


		protected const string UNKNOWN= "Неизвестно";
		public string Subject { get; set; }=UNKNOWN;
		public string Group{  get; set; }=UNKNOWN;
		public string PhoneNumber{  get; set; }=UNKNOWN;

		static int _lastDTOId = 0;
		public DTOTeacher()
		{
			DTOId = ++_lastDTOId;
			BirthDay = new DateTime(1970, 1, 1);
			Gender = "М";
		}
		public DTOTeacher(Model.Teacher other) 
		{
			ModelRef = other;

			_prevId = other.Id;
			_prevSurname = other.Surname;
			_prevName = other.Name;
			_prevPatronymic = other.Patronymic;
			_prevBirthDay = other.BirthDay;
			_prevGender = other.Gender;

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
				stringBuilder.Append(el.Year+el.Name);
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

		public override void Restore()
		{
			if (_prevId != 0)
				ModelRef.Id = _prevId;
			if (_prevSurname != null)
				ModelRef.Surname = _prevSurname;
			if (_prevName != null)
				ModelRef.Name = _prevName;
			if (_prevPatronymic != null)
				ModelRef.Patronymic = _prevPatronymic;
			if (_prevGender != null)
				ModelRef.Gender = _prevGender;
			if (_prevBirthDay != DateTime.MinValue)
				ModelRef.BirthDay = _prevBirthDay;
		}
	}
}
