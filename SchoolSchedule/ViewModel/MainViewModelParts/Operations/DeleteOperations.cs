using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Windows;

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
				else if (targetType == typeof(Model.DTO.DTOSchedule))
					await DeleteSchedules(selectedObjects as IList<Model.DTO.DTOSchedule>, db);
				else if (targetType == typeof(Model.DTO.DTOBellScheduleType))
					await DeleteBellScheduleTypes(selectedObjects as IList<Model.DTO.DTOBellScheduleType>, db);
				else if (targetType == typeof(Model.DTO.DTOBellSchedule))
					await DeleteBellSchedules(selectedObjects as IList<Model.DTO.DTOBellSchedule>, db);
				else if (targetType == typeof(Model.DTO.DTOLessonSubsitutionSchedule))
					await DeleteLessonSubsitutionSchedules(selectedObjects as IList<Model.DTO.DTOLessonSubsitutionSchedule>, db);

				await db.SaveChangesAsync();
			}
		}
		private async Task DeleteGroups(IList<Model.DTO.DTOGroup> selectedObjects, Model.SchoolScheduleEntities db)
		{
			var teachers = await db.Teacher.ToListAsync();
			var students = await db.Student.ToListAsync();
			foreach (var el in selectedObjects)
			{
				var teachersUses = FindTeachersUsesGroup(ref teachers, el.ModelRef.Id);
				var studentsUsees = FindStudentsUsesGroup(ref students, el.ModelRef.Id);

				if (teachersUses.Count() != 0 || studentsUsees.Count() != 0)
					throw new Exception($"Удалите записи всех уроков, учителей и всех студентов, ссылающихся на класс \"{el.ModelRef}\"");

				var forDelete = await db.Group.FirstAsync(x => x.Id == el.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
				db.Group.Remove(forDelete);
			}
		}
		private async Task DeleteSubjects(IList<Model.DTO.DTOSubject> selectedObjects, Model.SchoolScheduleEntities db)
		{
			var teachers = await db.Teacher.ToListAsync();

			foreach (var el in selectedObjects)
			{
				var teachersUsesSubject = FindTeachersUsesSubject(ref teachers, el.ModelRef.Id);

				if (teachersUsesSubject.Count() != 0 /*|| lessonsUsesSubject.Count() != 0*/)
					throw new Exception($"Удалите всех уроков и учителей, ссылающихся на предмет \"{el.Name}\"");

				var forDelete = await db.Subject.FirstAsync(x => x.Id == el.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				if (forDelete == null)
					throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
				db.Subject.Remove(forDelete);
			}
		}
		private async Task DeleteStudents(IList<Model.DTO.DTOStudent> selectedObjects, Model.SchoolScheduleEntities db)
		{
			foreach (var el in selectedObjects)
			{
				var forDelete = await db.Student.FirstAsync(x=>x.Id==el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				db.Student.Remove(forDelete);
			}
		}
		private async Task DeleteTeachers(IList<Model.DTO.DTOTeacher> selectedObjects, Model.SchoolScheduleEntities db)
		{
			var schedules = db.Schedule.ToListAsync().Result;
			var phones = db.TeacherPhone.ToListAsync().Result;
			foreach (var el in selectedObjects)
			{
				var forDelete = await db.Teacher.FirstAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				var schedulesUsesTeacher = FindSchedulesUsesTeacher(ref schedules, forDelete.Id);
				var phonesUsesTeacher = FindTeacherPhonesUsesTeacher(ref phones, forDelete.Id);
				if (schedulesUsesTeacher.Any())
					throw new Exception($"Удалите все объекты расписания, в которых записан преподаватель {el.ModelRef}");
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
		private async Task DeleteSchedules(IList<Model.DTO.DTOSchedule> selectedObjects, Model.SchoolScheduleEntities db)
		{
			foreach (var el in selectedObjects)
			{
				var forDelete = await db.Schedule.FirstAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				db.Schedule.Remove(forDelete);
			}
		}
		private async Task DeleteBellSchedules(IEnumerable<Model.DTO.DTOBellSchedule> selectedObjects, Model.SchoolScheduleEntities db)
		{
			foreach (var el in selectedObjects)
			{
				var schedules = await db.Schedule.ToListAsync();
				var schedulesUsesBellSchedule = FindSchedulesUsesBellSchedule(ref schedules, el.Id);

				var forDelete = await db.BellSchedule.FirstAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				if (schedulesUsesBellSchedule.Any())
					throw new Exception($"Отмените использование расписания \"{forDelete.BellScheduleType.Name}\" на {forDelete.LessonNumber} урок");
				db.BellSchedule.Remove(forDelete);
			}
		}
		private async Task DeleteBellScheduleTypes(IList<Model.DTO.DTOBellScheduleType> selectedObjects, Model.SchoolScheduleEntities db)
		{
			var bellSchedules = await db.BellSchedule.ToListAsync();

			foreach (var el in selectedObjects)
			{
				var bellSchedulesUsesBellScheduleType = FindBellSchedulesUsesBellScheduleType(ref bellSchedules, el.ModelRef.Id);
				var schedulesUsesBellScheduleType = (await db.Schedule.ToListAsync()).Where(x => x.BellSchedule.IdBellScheduleType == el.Id);

				if(schedulesUsesBellScheduleType.Count() != 0 )
					throw new Exception($"Отмените расписание звонков \"{el.Name}\" для всех уроков, во всём расписании занятий");

				if (bellSchedulesUsesBellScheduleType.Count() != 0)
				{
					MessageBoxResult messageBoxResult = MessageBox.Show($"Вы действительно хотите удалить все данные расписания \"{el.Name}\"?","Удаление расписания",MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
					if (messageBoxResult == MessageBoxResult.No || messageBoxResult == MessageBoxResult.Cancel)
						return;


					foreach(var bellSchedule in bellSchedulesUsesBellScheduleType)
						db.BellSchedule.Remove(bellSchedule);
				}

				var forDelete = await db.BellScheduleType.FirstAsync(x => x.Id == el.ModelRef.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
				if (forDelete == null)
					throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера");
				db.BellScheduleType.Remove(forDelete);
			}
		}
		private async Task DeleteLessonSubsitutionSchedules(IList<Model.DTO.DTOLessonSubsitutionSchedule> selectedObjects, Model.SchoolScheduleEntities db)
		{
			foreach (var el in selectedObjects)
			{
				var forDelete = await db.LessonSubsitutionSchedule.FirstAsync(x => x.Id == el.Id) ?? throw new Exception("Не удалось найти объект для удаления. Возможно, объект уже был удалён. Попробуйте обновить данные с сервера"); ;
				db.LessonSubsitutionSchedule.Remove(forDelete);
			}
		}
	}
}