namespace SchoolSchedule.Model
{
	public partial class Lesson
	{
		public override string ToString()
		{
			return $"Урок по предмету \"{Subject?.Name}\" у {Group} класса под номером \"{Number}\"";
		}
		public Lesson(Lesson other) : this()
		{
			this.Id=other.Id ;
			this.IdSubject= other.IdSubject ;
			this.IdGroup = other.IdGroup ;
			this.Number = other.Number ;

			this.Group=other.Group ;
			this.Subject=other.Subject ;

			/*
			foreach (var item in other.Schedule)
				Schedule.Add(item) ;
			*/
		}
	}
}