using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Edit
{
	public class EditLessonSubsitutionScheduleViewModel : ABaseViewModel
	{
		public bool ObjectIsNew {  get; set; }=true;
		public Model.LessonSubsitutionSchedule CurrentModel { get; set; } = new Model.LessonSubsitutionSchedule();
		public List<Model.LessonSubsitutionSchedule> ModelsForUniqueCheck { get; set; }
		#region Свойства для ввода
		public DateTime Date{ get => CurrentModel.Date; set 
			{
				CurrentModel.Date = value;
				if (CurrentModel.Date < DateTime.Today)
					CurrentModel.Date = DateTime.Today;

				CurrentModel.Date= new DateTime(CurrentModel.Date.Year, CurrentModel.Date.Month, CurrentModel.Date.Day);
			}
		}
		public int? IdSubject { get => CurrentModel.IdSubject; set { CurrentModel.IdSubject = value; CurrentModel.Subject = Subjects.Where(x => x.Id == value).First(); } }
		public int? IdTeacher { get => CurrentModel.IdTeacher; set { CurrentModel.IdTeacher = value; CurrentModel.Teacher = Teachers.Where(x => x.Id == value).First(); } }
		public int? IdGroup { get => CurrentModel.IdGroup; set { CurrentModel.IdGroup = value; CurrentModel.Group = Groups.Where(x => x.Id == value).First(); } }
		public int? ClassRoom{ get => CurrentModel.ClassRoom; set 
			{
				CurrentModel.ClassRoom=value;
				if(value != null)
					if(CurrentModel.ClassRoom <= 0)
						CurrentModel.ClassRoom = 1;
			}
		}
		public int LessonNumber{ get => CurrentModel.LessonNumber;set 
			{
				CurrentModel.LessonNumber = value;
				if (value < 1)
					CurrentModel.LessonNumber = 1;
				else if(value>8)
					CurrentModel.LessonNumber = 8;
			}
		}
		#endregion
		#region Списки для выбора
		public List<Model.Subject> Subjects { get; set; }
		public List<Model.Group> Groups { get; set; }
		public List<Model.Teacher> Teachers{ get; set; }
		#endregion

		public EditLessonSubsitutionScheduleViewModel()
		{
		}

		public EditLessonSubsitutionScheduleViewModel
		(
			Model.LessonSubsitutionSchedule model,
			List<Model.LessonSubsitutionSchedule> modelsForUniqueCheck,
			List<Model.Subject> subjects,
			List<Model.Group> groups,
			List<Model.Teacher> teachers,
			bool objectIsNew
		) : this()
		{
			ObjectIsNew = objectIsNew;
			CurrentModel = model ?? throw new ArgumentNullException(nameof(model));
			Subjects = new List<Model.Subject>(subjects);
			Groups = new List<Model.Group>(groups);
			Teachers = new List<Model.Teacher>(teachers);


			if (ObjectIsNew)
			{
				CurrentModel.Date = DateTime.Today;
				LessonNumber = LessonNumber;
			}

			ModelsForUniqueCheck = new List<Model.LessonSubsitutionSchedule>(modelsForUniqueCheck);
		}

		public KeyValuePair<bool, string> CheckInputRules()
		{
			if (CurrentModel.IdSubject==null && CurrentModel.IdGroup==null && CurrentModel.IdTeacher==null && CurrentModel.ClassRoom==null)
				return new KeyValuePair<bool, string>(false,$"Выберете значения хотя бы для одного поля для заполнения данных о замене" );
			if(ObjectIsNew && ModelsForUniqueCheck.Where(x=>x.LessonNumber==CurrentModel.LessonNumber && x.Date==CurrentModel.Date && x.IdSubject==CurrentModel.IdSubject && x.IdGroup==CurrentModel.IdGroup && x.IdTeacher==CurrentModel.IdTeacher).Any())			
				return new KeyValuePair<bool, string>
				(
					false,
					$"Замена на {CurrentModel.LessonNumber} урок на дату {CurrentModel.Date}\n" +
					((CurrentModel.IdSubject == null ? "" :
						($" с предментом \"{CurrentModel.Subject}\"" + (IdGroup == null && IdTeacher == null && ClassRoom == null ? "" : ", "))) +
					(CurrentModel.IdGroup == null ? "" :
						($"с классом \"{CurrentModel.Group}\"" + (IdTeacher == null && ClassRoom == null ? "" : ", "))) +
					(CurrentModel.IdTeacher == null ? "" :
						($"с учителем \"{CurrentModel.Teacher}\"" + (ClassRoom == null ? "" : ", "))) +
					(CurrentModel.ClassRoom == null ? "" :
						$"с кабинетом\"{CurrentModel.ClassRoom}\"") +
					"\nуже существует в базе данных")
				);


			CurrentModel.Subject = null;
			CurrentModel.Group = null;
			CurrentModel.Teacher = null;
			return new KeyValuePair<bool, string>(true, null);
		}
	}
}
