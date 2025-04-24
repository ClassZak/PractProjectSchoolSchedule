namespace SchoolSchedule.Model
{
	public partial class Subject
	{
		public Subject(Subject other) : this() 
		{
			this.Id = other.Id;
			this.Name = other.Name;

			/*
			foreach(var el in other.Lesson)
				this.Lesson.Add(el);
			foreach(var el in other.Teacher)
				this.Teacher.Add(el);
			*/
		}
		public override string ToString()
		{
			return Name;
		}
	}
}