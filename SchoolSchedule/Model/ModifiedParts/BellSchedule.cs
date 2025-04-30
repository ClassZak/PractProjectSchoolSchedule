namespace SchoolSchedule.Model
{
	public partial class BellSchedule
	{
		public BellSchedule(BellSchedule other) : this()
		{
			this.Id=other.Id;
			this.IdBellScheduleType= other.IdBellScheduleType;
			this.LessonNumber=other.LessonNumber;
			this.StartTime=other.StartTime;
			this.EndTime=other.EndTime;
		}
	}
}