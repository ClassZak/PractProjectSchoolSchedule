namespace SchoolSchedule.Model
{
	public partial class Group
	{
		public Group(Group other) : this()
		{
			this.Id = other.Id ;
			this.Name = other.Name ;
			this.Year = other.Year;

			/*
			foreach(var el in other.Lesson)
				this.Lesson.Add(el) ;
			foreach (var el in other.Teacher)
				this.Teacher.Add(el);
			foreach (var el in other.Student)
				this.Student.Add(el);
			*/
		}
		public override string ToString()
		{
			return $"{Year}{Name}";
		}
	}
}
