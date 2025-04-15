using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOGroup : ADTO<Model.Group>
	{
		public int Year
		{
			get => ModelRef.Year;
			set
			{
				if (value < 1 || value > 11)
					AddError(nameof(ModelRef.Year), "Год должен быть между 1 и 11 включительно!");
				else
					ClearErrors(nameof(Year));

				ModelRef.Year = value;
			}
		}

		public string Name
		{
			get => ModelRef.Name;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					AddError(nameof(ModelRef.Name), "Буква класса не может быть пустой!");
					return;
				}
				else
				if (value.Length!=1)
				{
					AddError(nameof(ModelRef.Name), "В записи класса требуется одна буква!");
					return;
				}
				else
					ClearErrors(nameof(ModelRef.Name));

				ModelRef.Name = value.ToUpper();
			}
		}

		static int _lastDTOId = 0;
		public DTOGroup() 
		{
			DTOId=++_lastDTOId;
		}
		public DTOGroup(Model.Group other) 
		{
			ModelRef=other;
		}

		public override bool HasReferenceOfNotExistingObject()
		{
			return false;
		}

		protected override void LoadAllLabels(ref Model.SchoolScheduleEntities dataBase)
		{
		}
	}
}
