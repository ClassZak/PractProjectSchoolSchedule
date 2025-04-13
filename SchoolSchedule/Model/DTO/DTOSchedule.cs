using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSchedule.Model.DTO
{
	public class DTOSchedule : Model.Schedule, IDTO
	{
		// LessonLabels
		public string Subject{ set { } get { return $"{Lesson?.Subject?.Name}"; } }
		public string Group{ set { } get { return $"{Lesson?.Group?.Year}{Lesson?.Group?.Name}"; } }
		public int Number{ set { } get { return Lesson?.Number?? 0; }}
		// TeacherLabels
		public string Surname { set { } get { return Teacher?.Surname; } }
		public string Name{ set { } get { return Teacher?.Name; } }
		public string Patronymic { set { } get { return Teacher?.Patronymic; } }

		public DTOSchedule() 
		{
			Id = 0;
		}
		public DTOSchedule(Model.Schedule other)
		{
			Id=other.Id;
			IdLesson = other.IdLesson;
			IdTeacher = other.IdTeacher;
			StartTime = other.StartTime;
			EndTime = other.EndTime;

			Date = other.Date;
			Lesson = other.Lesson;
			Teacher = other.Teacher;

			LoadAllLabels();
		}


		void ReloadLesson()
		{
			try 
			{
				using (var dataBase = new SchoolScheduleEntities())
				{
					if (Lesson == null)
					{
						var list = dataBase.Lesson.ToList().Where(el => el.Id == IdLesson);
						var first = list.First();
						if (first != null)
							Lesson = first;
					}
					if(Lesson!=null)
					{
						if(Lesson.Group == null)
						{
							var list = dataBase.Group.ToList().Where(el => el.Id == Lesson.IdGroup);
							var first = list.First();
							if (first != null)
								Lesson.Group=first;
						}
					}
				}
			}
			catch (Exception) 
			{ }
		}

		public void LoadLessonLabels()
		{
			if (Lesson != null)
				return;
			if (IdLesson != 0)
			{
				try
				{
					using (var dataBase = new Model.SchoolScheduleEntities())
					{
						var list = dataBase.Lesson.ToList().Where(el => el.Id == IdLesson);
						var first = list.First();
						if (first != null)
							Lesson = first;
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
		public void LoadTeacherLabels() 
		{
			if (IdTeacher != 0)
			{
				try
				{
					using (var dataBase = new Model.SchoolScheduleEntities())
					{
						var list = dataBase.Teacher.ToList().Where(el => el.Id == IdTeacher);
						var first = list.First();
						if (first != null)
							Teacher = first;
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
			LoadLessonLabels();
			LoadTeacherLabels();
		}
	}
}
