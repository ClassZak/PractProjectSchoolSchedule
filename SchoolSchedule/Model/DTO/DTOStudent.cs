using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSchedule.Model.DTO
{
	public class DTOStudent : ADTO<Model.Student>
	{
		#region Свойства DTOStudent
		#region Свойства Student
		public int Id{ get => ModelRef.Id; set { _prevId = ModelRef.Id; ModelRef.Id = value; } }
		public string Surname{ get { return ModelRef.Surname; } set { _prevSurname = ModelRef.Surname;  ModelRef.Surname = value; } }
		public string Name{ get { return ModelRef.Name; } set { _prevName = ModelRef.Name;  ModelRef.Name = value; } }
		public string Patronymic{ get { return ModelRef.Patronymic; } set { _prevPatronymic = ModelRef.Patronymic;  ModelRef.Patronymic = value; } }
		public string Email { get { return ModelRef.Email; } set { _prevEmail = ModelRef.Email; ModelRef.Email = value; } }
		public DateTime BirthDay { get { return ModelRef.BirthDay; } set { _prevBirthDay = ModelRef.BirthDay; ModelRef.BirthDay = value; } }
		public string Gender{ get { return ModelRef.Gender=="М" ? "Мужской" : (ModelRef.Gender == "Ж" ? "Женский" : string.Empty); } set { _prevGender = ModelRef.Gender; ModelRef.Gender = value.Substring(0,1).ToUpper(); } }
		#endregion
		#region Поля предыдущих значений
		private int _prevId = 0;
		private string _prevSurname = null;
		private string _prevName = null;
		private string _prevPatronymic = null;
		private string _prevEmail = null;
		private DateTime _prevBirthDay = DateTime.MinValue;
		private string _prevGender = null;
		#endregion
		#endregion



		protected const string UNKNOWN_GROUP = "Неизвестен";

		public int DTOIdGroup { get; set; } = 0;

		string _groupLabel=UNKNOWN_GROUP;
		public string GroupLabel { get { return _groupLabel; } set { } }


		static int _lastDTOId = 0;
		public DTOStudent()
		{
			DTOId=++_lastDTOId;
			BirthDay = new DateTime(2005,1,1);
			Gender = "М";
		}
		public DTOStudent(Model.Student other) 
		{
			ModelRef = other;
			_groupLabel = $"{ModelRef.Group.Year}{ModelRef.Group.Name}";

			_prevId = other.Id;
			_prevSurname=other.Surname;
			_prevName=other.Name;
			_prevPatronymic=other.Patronymic;
			_prevEmail=other.Email;
			_prevBirthDay=other.BirthDay;
			_prevGender=other.Gender;
		}
		
		public override bool HasReferenceOfNotExistingObject()
		{
			return DTOIdGroup != 0;
		}
		public override void Restore()
		{
			if (_prevId!=0)
				ModelRef.Id = _prevId;
			if (_prevSurname != null)
				ModelRef.Surname = _prevSurname;
			if (_prevName != null)
				ModelRef.Name = _prevName;
			if (_prevPatronymic != null)
				ModelRef.Patronymic=_prevPatronymic;
			if(_prevEmail != null)
				ModelRef.Email = _prevEmail;
			if(_prevGender != null)
				ModelRef.Gender = _prevGender;
			if (_prevBirthDay != DateTime.MinValue)
				ModelRef.BirthDay =_prevBirthDay;
		}
	}
}
