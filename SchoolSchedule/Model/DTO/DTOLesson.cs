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
		public int Id { get => ModelRef.Id; set { _prevId = ModelRef.Id; ModelRef.Id = value; } }
		public int IdSubject { get => ModelRef.IdSubject; set { _prevIdSubject = ModelRef.IdSubject; ModelRef.IdSubject = value; }}
		public int IdGroup { get => ModelRef.IdGroup; set { _prevIdGroup = ModelRef.IdGroup; ModelRef.IdGroup = value; } }
		public int Number{get => ModelRef.Number; set{ _prevNumber = ModelRef.Number; ModelRef.Number= value;}}
		#region Поля предыдущих значений
		int _prevId = 0;
		int _prevIdSubject = 0;
		int _prevIdGroup = 0;
		int _prevNumber = 0;
		#endregion
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
		public DTOLesson(Model.Lesson other) : base(other)
		{
			_prevId = other.Id;
			_prevIdSubject=other.IdSubject;
			_prevIdGroup = other.IdGroup;
			_prevNumber = other.Number;
		}
		public override bool HasReferenceOfNotExistingObject()
		{
			return ModelRef.IdSubject == 0 || ModelRef.IdGroup == 0;
		}
		public override void Restore()
		{
			if(_prevId!=0)
			{
				ModelRef.Id = _prevId;
				_prevId = 0;
			}
			if(_prevIdSubject!=0)
			{
				ModelRef.IdSubject= _prevIdSubject;
				_prevIdSubject = 0;
			}
			if(_prevIdGroup!=0)
			{
				ModelRef.IdGroup= _prevIdGroup;
				_prevIdGroup = 0;
			}
			if(_prevNumber!=0)
			{
				ModelRef.Number = _prevNumber;
				_prevNumber = 0;
			}
		}
	}
}
