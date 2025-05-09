using SchoolSchedule.Model.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditScheduleViewModel : ABaseViewModel
	{
		public bool ObjectIsNew { get; set; } = true;
		public Model.Schedule CurrentModel { get; set; } = new Model.Schedule();
		public List<Model.Schedule> ModelsForUniqueCheck { get; set; }
		#region Свойства для ввода
		public int IdSubject { get => CurrentModel.IdSubject; set { CurrentModel.IdSubject = value; CurrentModel.Subject = Subjects.Where(x => x.Id == value).First(); } }
		public int IdTeacher { get => CurrentModel.IdTeacher; set { CurrentModel.IdTeacher = value; CurrentModel.Teacher = Teachers.Where(x => x.Id == value).First(); } }
		public int IdGroup { get => CurrentModel.IdGroup; set { CurrentModel.IdGroup = value; CurrentModel.Group = Groups.Where(x => x.Id == value).First(); } }
		public int IdBellSchedule { get => CurrentModel.IdBellSchedule; set { CurrentModel.IdBellSchedule= value; CurrentModel.BellSchedule= BellSchedules.Where(x => x.Id == value).First(); } }
		public int DayOfTheWeek
		{
			get => CurrentModel.DayOfTheWeek; set
			{
				CurrentModel.DayOfTheWeek = value;
				if (CurrentModel.DayOfTheWeek <= 0)
					CurrentModel.DayOfTheWeek = 1;
				else if (CurrentModel.DayOfTheWeek > 7)
					CurrentModel.DayOfTheWeek = 7;
			}
		}
		public int ClassRoom
		{
			get => CurrentModel.ClassRoom; set
			{
				CurrentModel.ClassRoom = value;
				if (CurrentModel.ClassRoom <= 0)
					CurrentModel.ClassRoom = 1;
			}
		}
		#endregion
		#region Списки для выбора
		public List<Model.Subject> Subjects { get; set; }
		public List<Model.Group> Groups{ get; set; }
		public List<Model.Teacher> Teachers { get; set; }
		public List<Model.BellSchedule> BellSchedules{ get; set; }
		#endregion
		public EditScheduleViewModel() { }
		public EditScheduleViewModel
		(
			bool objectIsNew,
			Model.Schedule model,
			List<Model.Schedule> modelsForUniqueCheck,
			List<Model.Subject> subjects,
			List<Model.Group> groups,
			List<Model.Teacher> teachers,
			List<Model.BellSchedule> bellSchedules
		) : this()
		{
			if (subjects.Count == 0)
				throw new ArgumentException("В базе данных нет предметов для назначения расписания уроков");
			if (groups.Count == 0)
				throw new ArgumentException("В базе данных нет классов для назначения расписания уроков");
			if (teachers.Count == 0)
				throw new ArgumentException("В базе данных нет учителей для назначения расписания уроков");
			if (bellSchedules.Count == 0)
				throw new ArgumentException("В базе данных нет звонков для назначения расписания уроков");
			ObjectIsNew = objectIsNew;
			CurrentModel = model ?? throw new ArgumentNullException(nameof(model));
			Subjects = new List<Model.Subject>(subjects);
			Groups = new List<Model.Group>(groups);
			Teachers = new List<Model.Teacher>(teachers);
			BellSchedules = new List<Model.BellSchedule>(bellSchedules);

			if (ObjectIsNew)
			{
				CurrentModel.DayOfTheWeek = (int)Model.Additional.DayOfTheWeek.Понедельник;
				CurrentModel.ClassRoom = 10;
				IdSubject = Subjects.First().Id;
				IdGroup = Groups.First().Id;
				IdTeacher = Teachers.First().Id;
				IdBellSchedule = BellSchedules.First().Id;
			}

			ModelsForUniqueCheck=new List<Model.Schedule>(modelsForUniqueCheck);
			if (!ObjectIsNew)
				ModelsForUniqueCheck.Remove(model);
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (ModelsForUniqueCheck.Any
				(
					x =>
					x.IdSubject == CurrentModel.IdSubject &&
					x.IdGroup == CurrentModel.IdGroup &&
					x.IdTeacher == CurrentModel.IdTeacher &&
					x.IdBellSchedule == CurrentModel.IdBellSchedule &&
					x.DayOfTheWeek == CurrentModel.DayOfTheWeek && 
					x.ClassRoom == CurrentModel.ClassRoom
				))
				return new KeyValuePair<bool, string>(false, "Такое расписание уже существует в базе данных");

			if(ModelsForUniqueCheck.Any
				(
					x=>
					x.IdTeacher==CurrentModel.IdTeacher &&
					x.DayOfTheWeek == CurrentModel.DayOfTheWeek &&
					x.BellSchedule.LessonNumber == CurrentModel.BellSchedule.LessonNumber &&
					x.BellSchedule.IdBellScheduleType == CurrentModel.BellSchedule.IdBellScheduleType
				))
				return new KeyValuePair<bool, string>
				(
					false, 
					$"Преподаватель \"{CurrentModel.Teacher}\" уже занят на " +
					$"{CurrentModel.BellSchedule.LessonNumber} уроке в " +
					$"{Additional.DayOfTheWeekConverter.Convert((Model.Additional.DayOfTheWeek)DayOfTheWeek).ToLower()}"
				);
			if(ModelsForUniqueCheck.Any
				(
					x=>
					x.ClassRoom==CurrentModel.ClassRoom &&
					x.DayOfTheWeek==CurrentModel.DayOfTheWeek &&
					x.BellSchedule.LessonNumber==CurrentModel.BellSchedule.LessonNumber &&
					x.BellSchedule.IdBellScheduleType == CurrentModel.BellSchedule.IdBellScheduleType
				))
				return new KeyValuePair<bool, string>
				(
					false, 
					$"Кабинет номер {CurrentModel.ClassRoom} уже занят на " +
					$"{CurrentModel.BellSchedule.LessonNumber} уроке в " +
					$"{Additional.DayOfTheWeekConverter.Convert((Model.Additional.DayOfTheWeek)DayOfTheWeek).ToLower()}"
				);
			if(ModelsForUniqueCheck.Any
				(
					x=>
					x.IdGroup==CurrentModel.IdGroup &&
					x.DayOfTheWeek==CurrentModel.DayOfTheWeek &&
					x.BellSchedule.LessonNumber==CurrentModel.BellSchedule.LessonNumber &&
					x.BellSchedule.IdBellScheduleType == CurrentModel.BellSchedule.IdBellScheduleType
				))
				return new KeyValuePair<bool, string>
				(
					false, 
					$"Класс {CurrentModel.Group} уже занят на " +
					$"{CurrentModel.BellSchedule.LessonNumber} уроке в " +
					$"{Additional.DayOfTheWeekConverter.Convert((Model.Additional.DayOfTheWeek)DayOfTheWeek).ToLower()}"
				);

			CurrentModel.Subject = null;
			CurrentModel.Group = null;
			CurrentModel.Teacher = null;
			CurrentModel.BellSchedule = null;
			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
