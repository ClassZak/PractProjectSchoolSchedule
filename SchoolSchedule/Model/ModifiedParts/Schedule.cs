namespace SchoolSchedule.Model
{
	public partial class Schedule
	{
		public Schedule()
		{ }
		public Schedule(Schedule other) : this()
		{
			this.Id= other.Id;
			//this.IdLesson = other.IdLesson;
			//this.IdTeacher= other.IdTeacher;
			//this.StartTime = other.StartTime;
			//this.EndTime = other.EndTime;
			//this.Date = other.Date;

			//this.Lesson = other.Lesson;
			//this.Teacher = other.Teacher;
		}
	}
}