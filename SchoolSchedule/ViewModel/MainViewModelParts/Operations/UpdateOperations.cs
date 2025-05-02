using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel
{
	public partial class MainViewModel
	{
		public async Task UpdateEntityAsync(object entity, object selectedObject, Type targetType)
		{
			using (var db = new SchoolSchedule.Model.SchoolScheduleEntities())
			{
				await EnsureConnectionOpenAsync(db);
				switch (entity)
				{
					case Model.Group group:
						await UpdateGroup(group, selectedObject as Model.DTO.DTOGroup, db);
						break;
					case Model.Subject subject:
						await UpdateSubject(subject, selectedObject as Model.DTO.DTOSubject, db);
						break;
					case Model.Student student:
						await UpdateStudent(student, selectedObject as Model.DTO.DTOStudent, db);
						break;
					case Model.Teacher teacher:
						await UpdateTeacher(teacher, selectedObject as Model.DTO.DTOTeacher, db);
						break;
					case Model.Schedule schedule:
						await UpdateSchedule(schedule, selectedObject as Model.DTO.DTOSchedule, db);
						break;
					case Model.BellScheduleType bellScheduleType:
						await UpdateBellScheduleType(bellScheduleType, selectedObject as Model.DTO.DTOBellScheduleType, db);
						break;
					case Model.BellSchedule bellSchedule:
						await UpdateBellSchedule(bellSchedule, selectedObject as Model.DTO.DTOBellSchedule, db);
						break;
					case Model.LessonSubsitutionSchedule lessonSubsitutionSchedule:
						await UpdateLessonSubsitutionSchedule(lessonSubsitutionSchedule, selectedObject as Model.DTO.DTOLessonSubsitutionSchedule, db);
						break;
				}
				await db.SaveChangesAsync();
			}
		}
		private async Task UpdateGroup(Model.Group group, Model.DTO.DTOGroup selectedObject, Model.SchoolScheduleEntities db)
		{
			var forUpdate = await db.Group.FirstAsync(x => x.Id == selectedObject.ModelRef.Id);

			if (forUpdate == null)
				throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
			forUpdate.Name = group.Name;
			forUpdate.Year = group.Year;
		}
		private async Task UpdateSubject(Model.Subject subject, Model.DTO.DTOSubject selectedObject, Model.SchoolScheduleEntities db)
		{
			var forUpdate = await db.Subject.FirstAsync(x => x.Id == selectedObject.ModelRef.Id);
			if (forUpdate == null)
				throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
			forUpdate.Name = subject.Name;
		}
		private async Task UpdateStudent(Model.Student student, Model.DTO.DTOStudent selectedObject, Model.SchoolScheduleEntities db)
		{
			var forUpdate = await db.Student.FirstAsync(x => x.Id == selectedObject.ModelRef.Id);
			if (forUpdate == null)
				throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
			forUpdate.Surname = student.Surname;
			forUpdate.Name = student.Name;
			forUpdate.Patronymic = student.Patronymic;
			forUpdate.IdGroup = student.IdGroup;
			forUpdate.Email = student.Email;
			forUpdate.Gender = student.Gender;
			forUpdate.BirthDay = student.BirthDay;
		}
		private async Task UpdateTeacher(Model.Teacher teacher, Model.DTO.DTOTeacher selectedObject, Model.SchoolScheduleEntities db)
		{
			List<Model.TeacherPhone> phones = new List<Model.TeacherPhone>(teacher.TeacherPhone);
			List<Model.Group> groups = new List<Model.Group>(teacher.Group);
			List<Model.Subject> subjects = new List<Model.Subject>(teacher.Subject);

			teacher.Subject.Clear();
			teacher.Group.Clear();
			teacher.TeacherPhone.Clear();

			var forUpdate = await db.Teacher
				.Include(t => t.TeacherPhone) // Добавляем Include для загрузки связанных телефонов
				.FirstAsync(x => x.Id == selectedObject.ModelRef.Id);

			if (forUpdate == null)
				throw new Exception("Объект не найден");

			forUpdate.Surname = teacher.Surname;
			forUpdate.Name = teacher.Name;
			forUpdate.Patronymic = teacher.Patronymic;
			forUpdate.Gender = teacher.Gender;
			forUpdate.BirthDay = teacher.BirthDay;

			foreach (var phone in forUpdate.TeacherPhone.ToList())
			{
				if (phone.IdTeacher == forUpdate.Id)
					db.TeacherPhone.Remove(phone);
			}

			foreach (var el in phones)
			{
				db.TeacherPhone.Add(new Model.TeacherPhone
				{
					IdTeacher = forUpdate.Id,
					PhoneNumber = el.PhoneNumber
				});
			}

			// Обновляем Subject и Group (many-to-many)
			forUpdate.Subject.Clear();
			foreach (var el in subjects)
			{
				var subject = await db.Subject.FindAsync(el.Id);
				if (subject != null)
					forUpdate.Subject.Add(subject);
			}

			forUpdate.Group.Clear();
			foreach (var el in groups)
			{
				var group = await db.Group.FindAsync(el.Id);
				if (group != null)
					forUpdate.Group.Add(group);
			}
		}
		private async Task UpdateSchedule(Model.Schedule schedule, Model.DTO.DTOSchedule selectedObject, Model.SchoolScheduleEntities db)
		{
			var forUpdate = await db.Schedule.FirstAsync(x => x.Id == selectedObject.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
		}
		private async Task UpdateBellScheduleType(Model.BellScheduleType bellScheduleType, Model.DTO.DTOBellScheduleType selectedObject, Model.SchoolScheduleEntities db)
		{
			var forUpdate = await db.BellScheduleType.FirstAsync(x => x.Id == selectedObject.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
			forUpdate.Name = bellScheduleType.Name;
		}
		private async Task UpdateBellSchedule(Model.BellSchedule bellSchedule, Model.DTO.DTOBellSchedule selectedObject, Model.SchoolScheduleEntities db)
		{
			var forUpdate = await db.BellSchedule.FirstAsync(x => x.Id == selectedObject.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
			forUpdate.StartTime = bellSchedule.StartTime;
			forUpdate.EndTime = bellSchedule.EndTime;
		}
		private async Task UpdateLessonSubsitutionSchedule(Model.LessonSubsitutionSchedule lessonSubsitutionSchedule, Model.DTO.DTOLessonSubsitutionSchedule selectedObject, Model.SchoolScheduleEntities db)
		{
			var forUpdate = await db.LessonSubsitutionSchedule.FirstAsync(x => x.Id == selectedObject.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для его редактирования. Возможно, редактируемый объект удалён. Попробуйте обновить данные с серва");
			forUpdate.Date = lessonSubsitutionSchedule.Date;
			forUpdate.IdSubject = lessonSubsitutionSchedule.IdSubject;
			forUpdate.IdGroup=lessonSubsitutionSchedule.IdGroup;
			forUpdate.IdTeacher = lessonSubsitutionSchedule.IdTeacher;
			forUpdate.ClassRoom=lessonSubsitutionSchedule.ClassRoom;
			forUpdate.LessonNumber = lessonSubsitutionSchedule.LessonNumber;
		}
	}
}