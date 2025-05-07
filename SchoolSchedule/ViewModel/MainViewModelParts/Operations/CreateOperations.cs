using SchoolSchedule.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SchoolSchedule.ViewModel
{
	public partial class MainViewModel
	{
		private async Task CreateEntityAsync(object entity, Type targetType)
		{
			using (var db = new SchoolScheduleEntities())
			{
				await EnsureConnectionOpenAsync(db);
				switch (entity)
				{
					case Model.Group group:
						CreateGroup(group, db);
						break;
					case Model.Subject subject:
						CreateSubject(subject, db);
						break;
					case Model.Student student:
						CreateStudent(student, db);
						break;
					case Model.Teacher teacher:
						await CreateTeacher(teacher, db);
						break;
					case Model.Schedule schedule:
						CreateSchedule(schedule, db);
						break;
					case Model.BellScheduleType bellScheduleType:
						CreateBellScheduleType(bellScheduleType, db);
						break;
					case Model.BellSchedule bellSchedule:
						CreateBellSchedule(bellSchedule, db);
						break;
					case Model.LessonSubsitutionSchedule lessonSubsitutionSchedule:
						CreateLessonSubsitutionSchedule(lessonSubsitutionSchedule, db);
						break;
				}
				await db.SaveChangesAsync();
			}
		}
		private void CreateGroup(Model.Group group, SchoolScheduleEntities db)
		{
			db.Group.Add(group);
		}
		private void CreateSubject(Model.Subject subject, SchoolScheduleEntities db)
		{
			db.Subject.Add(subject);
		}
		private void CreateStudent(Model.Student student, SchoolScheduleEntities db)
		{
			db.Student.Add(student);
		}
		private async Task CreateTeacher(Model.Teacher teacher, SchoolScheduleEntities db)
		{
			List<Model.TeacherPhone> phones = new List<Model.TeacherPhone>(teacher.TeacherPhone);
			List<Model.Group> groups = new List<Model.Group>(teacher.Group);
			List<Model.Subject> subjects = new List<Model.Subject>(teacher.Subject);

			teacher.Subject.Clear();
			teacher.Group.Clear();
			teacher.TeacherPhone.Clear();


			var forEdit = db.Teacher.Add(teacher);

			if (forEdit == null)
				throw new Exception("Не удалось добавить предметы и классы для учителей");
			foreach (var el in subjects)
				forEdit.Subject.Add(await db.Subject.FindAsync(el.Id));
			foreach (var el in groups)
				forEdit.Group.Add(await db.Group.FindAsync(el.Id));
			foreach (var el in phones)
				db.TeacherPhone.Add(new Model.TeacherPhone { IdTeacher = teacher.Id, PhoneNumber = el.PhoneNumber });
		}
		private void CreateSchedule(Model.Schedule schedule, SchoolScheduleEntities db)
		{
			db.Schedule.Add(schedule);
		}
		private void CreateBellScheduleType(Model.BellScheduleType bellScheduleType, SchoolScheduleEntities db)
		{
			db.BellScheduleType.Add(bellScheduleType);
		}
		private void CreateBellSchedule(Model.BellSchedule bellSchedule, SchoolScheduleEntities db)
		{
			db.BellSchedule.Add(bellSchedule);
		}
		private void CreateLessonSubsitutionSchedule(Model.LessonSubsitutionSchedule lessonSubsitutionSchedule, SchoolScheduleEntities db)
		{
			db.LessonSubsitutionSchedule.Add(lessonSubsitutionSchedule);
		}
	}
}
