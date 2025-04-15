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
		public string Surname{ get { return ModelRef.Surname; } set { ModelRef.Surname = value; } }
		public string Name{ get { return ModelRef.Name; } set { ModelRef.Name = value; } }
		public string Patronymic{ get { return ModelRef.Patronymic; } set { ModelRef.Patronymic = value; } }
		public string Email { get { return ModelRef.Email; } set { ModelRef.Email = value; } }




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
			_groupLabel = $"{other.Group.Year}{other.Group.Name}";
		}
		
		public override bool HasReferenceOfNotExistingObject()
		{
			return DTOIdGroup != 0;
		}

		protected override void LoadAllLabels(ref Model.SchoolScheduleEntities dataBase)
		{
			
		}
	}
}
