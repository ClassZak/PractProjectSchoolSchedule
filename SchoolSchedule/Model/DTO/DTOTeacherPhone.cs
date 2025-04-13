using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSchedule.Model.DTO
{
	public class DTOTeacherPhone : Model.TeacherPhone, IDTO
	{
		public string TeacherLabel{  get; set; }
		public DTOTeacherPhone()
		{
			Id = 0; 
		}
		public DTOTeacherPhone(Model.TeacherPhone phone)
		{
			Id=phone.Id;
			IdTeacher=phone.IdTeacher;
			PhoneNumber = phone.PhoneNumber;

			LoadAllLabels();
		}
		public void LoadTeacherLabel()
		{
			if (Teacher != null)
				TeacherLabel = $"{Teacher.Surname} {Teacher.Name} {Teacher.Patronymic}";
			if(IdTeacher!=0)
			{
				try
				{
					using (var dataBase = new SchoolSchedule.Model.SchoolScheduleEntities())
					{
						var list = dataBase.Teacher.ToList().Where(el => el.Id==IdTeacher);
						var first=list.First();
						if (first != null)
							TeacherLabel = $"{first.Surname} {first.Name} {first.Patronymic}";
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
			LoadTeacherLabel();
		}
	}
}
