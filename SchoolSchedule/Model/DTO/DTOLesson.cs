using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public class DTOLesson : ADTO<Model.Lesson>
	{
		#region Свойства DTOLesson
		public int Id
		{
			get=>ModelRef.Id; set=>ModelRef.Id = value; 
		}
		public int IdSubject
		{
			get=>ModelRef.IdSubject; set=>ModelRef.IdSubject= value; 
		}
		public int IdGroup
		{
			get=>ModelRef.IdGroup; set=>ModelRef.IdGroup= value; 
		}
		public int Number
		{
			get => ModelRef.Number; set=>ModelRef.Number= value;
		}

		public string Group
		{
			get
			{
				if (ModelRef.Group != null)
					return $"{ModelRef.Group?.Year}{ModelRef.Group?.Name}";
				else
					return string.Empty;
			}
			set{ }
		}
		public string Subject
		{
			get
			{
				if (ModelRef.Subject!= null)
					return ModelRef.Subject?.Name;
				else
					return string.Empty;
			}
			set{ }
		}
		#endregion

		static int _lastDTOId = 0;
		public DTOLesson()
		{
			DTOId = ++_lastDTOId;
		}
		public DTOLesson(Model.Lesson other) : base(other) { }
		public override bool HasReferenceOfNotExistingObject()
		{
			return ModelRef.IdSubject == 0 || ModelRef.IdGroup == 0;
		}
	}
}
