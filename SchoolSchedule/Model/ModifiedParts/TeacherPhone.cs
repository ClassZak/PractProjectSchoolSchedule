namespace SchoolSchedule.Model
{
	public partial class TeacherPhone
	{
		public TeacherPhone() { }
		public TeacherPhone(TeacherPhone other) 
		{
			this.Id=other.Id;
			this.PhoneNumber=other.PhoneNumber;
			this.IdTeacher=other.IdTeacher;

			this.Teacher= other.Teacher;
		}
		public override string ToString()
		{
			return PhoneNumber;
		}
	}
}