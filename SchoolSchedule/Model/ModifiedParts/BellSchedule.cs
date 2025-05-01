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
		public override string ToString()
		{
			return $"{BellScheduleType?.ToString()} {StartTime:hh\\:mm\\:ss} - {EndTime:hh\\:mm\\:ss} ({LessonNumber} урок)";
		}
	}
}