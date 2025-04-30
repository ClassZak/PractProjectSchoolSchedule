namespace SchoolSchedule.Model
{
	public partial class Schedule
	{
		public Schedule()
		{ }
		public Schedule(Schedule other) : this()
		{
			this.Id= other.Id;
			this.IdSubject= other.IdSubject;
			this.IdGroup= other.IdGroup;
			this.IdTeacher= other.IdTeacher;
			this.IdBellSchedule= other.IdBellSchedule;
			this.DayOfTheWeek= other.DayOfTheWeek;
			this.ClassRoom= other.ClassRoom;
		}
	}
}