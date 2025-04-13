using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSchedule.Model.DTO
{
	public class DTOLesson : Model.Lesson, IDTO
	{
		public string GroupLabel { get; set; }
		public string SubjectLabel { get; set; }
		public DTOLesson() 
		{
			Id = 0; 
		}
		public DTOLesson(Model.Lesson other)
		{
			Id=other.Id;
			IdSubject = other.IdSubject;
			IdGroup = other.IdGroup;
			Number = other.Number;


			Group = other.Group;
			Subject = other.Subject;

			Schedule = other.Schedule;

			LoadAllLabels();
		}

		public void LoadGroupLabel()
		{
			if (Group != null)
			{
				GroupLabel = $"{Group.Year}{Group.Name}";
				return;
			}
			if (IdGroup!=0)
			{
				try
				{
					using (var dataBase = new Model.SchoolScheduleEntities())
					{
						var list = dataBase.Group.ToList().Where(el => el.Id==IdGroup);
						var first=list.First();
						if (first != null)
							GroupLabel = $"{first.Year}{first.Name}";
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
		public void LoadSubjectLabel()
		{
			if (Subject != null)
			{
				SubjectLabel = $"{Subject.Name}";
				return;
			}
			if (IdSubject != 0)
			{
				try
				{
					using (var dataBase = new Model.SchoolScheduleEntities())
					{
						var list = dataBase.Subject.ToList().Where(el => el.Id == IdSubject);
						var first = list.First();
						if (first != null)
							SubjectLabel = $"{first.Name}";
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
		public void LoadAllLabels()
		{
			LoadGroupLabel();
			LoadSubjectLabel();
		}
	}
}
