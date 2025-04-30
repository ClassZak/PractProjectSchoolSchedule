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
		#endregion
		#region Поля предыдущих значений
		private int _prevId = 0;
		private string _prevSurname = null;
		private string _prevName = null;
		private string _prevPatronymic = null;
		private string _prevEmail = null;
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
		}
		
		public override bool HasReferenceOfNotExistingObject()
		{
			return DTOIdGroup != 0;
		}
		public override void Restore()
		{
			if (_prevId!=0)
			{
				ModelRef.Id = _prevId;
				_prevId=0;
			}
			if (_prevSurname != null)
			{
				ModelRef.Surname = _prevSurname;
				_prevSurname = null;
			}
			if (_prevName != null)
			{
				ModelRef.Name = _prevName;
				_prevName = null;
			}
			if (_prevPatronymic != null)
			{
				ModelRef.Patronymic=_prevPatronymic;
				_prevPatronymic = null;
			}
			if(_prevEmail != null)
			{
				ModelRef.Email = _prevEmail;
				_prevEmail = null;
			}
		}
	}
}
