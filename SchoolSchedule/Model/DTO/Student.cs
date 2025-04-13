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
		public DTOStudent() : base() { Id = 0; IdGroup = 0; }
		public DTOStudent(Model.Student student, bool loadLabels=false) : this()
		{
			Id = student.Id;
			IdGroup = student.IdGroup;

			Surname = student.Surname;
			Name = student.Name;
			Patronymic = student.Patronymic;
			Email = student.Email;

			if(loadLabels)
				LoadGroupLabel();
		}
		public void LoadAllLabels()
		{
			LoadGroupLabel();
		}
		public void LoadGroupLabel()
		{
			try
			{
				if(IdGroup==0)
				{
					GroupLabel = UNKNOWN_GROUP;
					return;
				}
				using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
				{
					var groupsList = dataBase.Groups.ToList().Where(el => el.Id == IdGroup);
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
