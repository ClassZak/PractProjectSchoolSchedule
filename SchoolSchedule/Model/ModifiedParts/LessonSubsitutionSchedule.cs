namespace SchoolSchedule.Model
{
	public partial class LessonSubsitutionSchedule
	{
		public LessonSubsitutionSchedule() { }
		public LessonSubsitutionSchedule(LessonSubsitutionSchedule other) : this()
		{
			Id = other.Id;
			Date = other.Date;
			IdSubject=other.IdSubject;
			IdGroup=other.IdGroup;
			IdTeacher=other.IdTeacher;
			ClassRoom=other.ClassRoom;
			LessonNumber=other.LessonNumber;
		}
	}
}