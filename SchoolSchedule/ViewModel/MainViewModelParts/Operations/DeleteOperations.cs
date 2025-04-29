using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;

namespace SchoolSchedule.ViewModel
{
	public partial class MainViewModel
	{
		public async Task DeleteEntitiesAsync(IList selectedObjects, Type targetType)
		{
			using (var db = new Model.SchoolScheduleEntities())
			{
				await EnsureConnectionOpenAsync(db);
				if (targetType == typeof(Model.DTO.DTOGroup))
					await DeleteGroups(selectedObjects as IList<Model.DTO.DTOGroup>, db);
				else if (targetType == typeof(Model.DTO.DTOSubject))
					await DeleteSubjects(selectedObjects as IList<Model.DTO.DTOSubject>, db);
				else if (targetType == typeof(Model.DTO.DTOStudent))
					await DeleteStudents(selectedObjects as IList<Model.DTO.DTOStudent>, db);
				else if (targetType == typeof(Model.DTO.DTOTeacher))
					await DeleteTeachers(selectedObjects as IList<Model.DTO.DTOTeacher>, db);
				else if (targetType == typeof(Model.DTO.DTOLesson))
					await DeleteLessons(selectedObjects as IList<Model.DTO.DTOLesson>, db);
				else if (targetType == typeof(Model.DTO.DTOSchedule))
					await DeleteSchedules(selectedObjects as IList<Model.DTO.DTOSchedule>, db);
				await db.SaveChangesAsync();
			}
		}
		private async Task DeleteGroups(IList<Model.DTO.DTOGroup> selectedObjects, Model.SchoolScheduleEntities db)
		{
			var teachers = await db.Teacher.ToListAsync();
			var lessons = await db.Lesson.ToListAsync();
			var students = await db.Student.ToListAsync();
			foreach (var el in selectedObjects)
			{
				var selectedDTO = el as Model.DTO.DTOGroup;
				var teachersUses = FindTeachersUsesGroup(ref teachers, selectedDTO.ModelRef.Id);
				var lessonsUses = FindLessonsUsesGroup(ref lessons, selectedDTO.ModelRef.Id);
				var studentsUsees = FindStudentsUsesGroup(ref students, selectedDTO.ModelRef.Id);

				if (teachersUses.Count() != 0 || lessonsUses.Count() != 0 || studentsUsees.Count() != 0)
					throw new Exception($"Удалите записи всех уроков, учителей и всех студентов, ссылающихся на класс \"{selectedDTO.ModelRef}\"");

				var forDelete = await db.Group.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
				db.Group.Remove(forDelete);
			}
		}
		private async Task DeleteSubjects(IList<Model.DTO.DTOSubject> selectedObjects, Model.SchoolScheduleEntities db)
		{
			var teachers = await db.Teacher.ToListAsync();
			var lessons = await db.Lesson.ToListAsync();

			foreach (var el in selectedObjects)
			{
				var selectedDTO = el as Model.DTO.DTOSubject;
				var teachersUsesSubject = FindTeachersUsesSubject(ref teachers, selectedDTO.ModelRef.Id);
				var lessonsUsesSubject = FindLessonsUsesSubject(ref lessons, selectedDTO.ModelRef.Id);

				if (teachersUsesSubject.Count() != 0 || lessonsUsesSubject.Count() != 0)
					throw new Exception($"Удалите всех уроков и учителей, ссылающихся на предмет \"{selectedDTO.Name}\"");

				var forDelete = await db.Subject.FirstAsync(x => x.Id == selectedDTO.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				if (forDelete == null)
					throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
				db.Subject.Remove(forDelete);
			}
		}
		private async Task DeleteStudents(IList<Model.DTO.DTOStudent> selectedObjects, Model.SchoolScheduleEntities db)
		{
			foreach (var el in selectedObjects)
			{
				var selectedDTO = el as Model.DTO.DTOStudent;
				var forDelete = await db.Student.FirstAsync() ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				db.Student.Remove(forDelete);
			}
		}
		private async Task DeleteTeachers(IList<Model.DTO.DTOTeacher> selectedObjects, Model.SchoolScheduleEntities db)
		{
			var schedules = db.Schedule.ToListAsync().Result;
			var phones = db.TeacherPhone.ToListAsync().Result;
			foreach (var el in selectedObjects)
			{
				var selectedDTO = el as Model.DTO.DTOTeacher;
				var forDelete = await db.Teacher.FirstAsync(x => x.Id == selectedDTO.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				var schedulesUsesTeacher = FindSchedulesUsesTeacher(ref schedules, forDelete.Id);
				var phonesUsesTeacher = FindTeacherPhonesUsesTeacher(ref phones, forDelete.Id);
				if (schedulesUsesTeacher.Any())
					throw new Exception($"Удалите все объекты расписания, в которых записан преподаватель {selectedDTO.ModelRef}");
				if (forDelete == null)
					throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");

				foreach (var p in phonesUsesTeacher)
					db.TeacherPhone.Remove(p);

				var teacherGroup = new List<Model.Group>(forDelete.Group);
				foreach (var group in teacherGroup)
					forDelete.Group.Remove(group);
				await db.SaveChangesAsync();
				var teacherSubject = new List<Model.Subject>(forDelete.Subject);
				foreach (var subject in teacherSubject)
					forDelete.Subject.Remove(subject);
				await db.SaveChangesAsync();

				db.Teacher.Remove(forDelete);
			}
		}
		private async Task DeleteLessons(IList<Model.DTO.DTOLesson> selectedObjects,Model.SchoolScheduleEntities db)
		{
			var schedules = db.Schedule.ToListAsync().Result;
			foreach (var el in selectedObjects)
			{
				var schedulesUseesLesson = FindSchedulesUsesLesson(ref schedules, el.Id);
				if (schedulesUseesLesson.Any())
					throw new Exception($"Удалите все объекты расписания, в которых проводится занятие по предмету \"{el.Subject}\" с классом \"{el.Group}\"");

				var forDelete = await db.Lesson.FirstAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
				db.Lesson.Remove(forDelete);
			}
		}
		private async Task DeleteSchedules(IList<Model.DTO.DTOSchedule> selectedObjects, Model.SchoolScheduleEntities db)
		{
			foreach (var el in selectedObjects)
			{
				var selectedDTO = el as Model.DTO.DTOSchedule;
				var forDelete = await db.Schedule.FirstAsync(x => x.Id == selectedDTO.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				db.Schedule.Remove(forDelete);
			}
		}
	}
}