using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSchedule.Model.DTO
{
	public class DTOStudent : Model.Student, IDTO
	{
		protected const string UNKNOWN_GROUP = "Неизвестен";
		public string GroupLabel { get; set; } = UNKNOWN_GROUP;
		public DTOStudent() 
		{
			Id = 0;
			IdGroup = 0; 
		}
		public DTOStudent(Model.Student other) : this()
		{
			Id = other.Id;
			IdGroup = other.IdGroup;

			Surname = other.Surname;
			Name = other.Name;
			Patronymic = other.Patronymic;
			Email = other.Email;

			Group = other.Group;

			LoadGroupLabel();
		}
		public void LoadAllLabels()
		{
			LoadGroupLabel();
		}
		public void LoadGroupLabel()
		{
			if (GroupLabel != null)
				GroupLabel = $"{Group.Year}{Group.Name}";

			try
			{
				if(IdGroup==0)
				{
					GroupLabel = UNKNOWN_GROUP;
					return;
				}
				using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
				{
					var groupsList = dataBase.Group.ToList().Where(el => el.Id == IdGroup);
					if (!groupsList.Any())
					{
						GroupLabel = UNKNOWN_GROUP;
						return;
					}
					var group= groupsList.First();

					GroupLabel = $"{group.Year}{group.Name}";
				}
			}
			catch (System.Data.EntityException ex)
			{
				MessageBox.Show
				(
					ex.InnerException != null ? ex.InnerException.Message : ex.Message,
					"Ошибка базы данных",
					MessageBoxButton.OK,
					MessageBoxImage.Stop
				);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Stop);
			}
		}
	}
}
