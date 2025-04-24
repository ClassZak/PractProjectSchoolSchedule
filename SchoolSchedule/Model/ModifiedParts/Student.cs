namespace SchoolSchedule.Model
{
	public partial class Student
	{
		public Student() { }
		public Student(Student other) : this()
		{ 
			this.Id=other.Id;
			this.Name=other.Name;
			this.Surname=other.Surname;
			this.Patronymic=other.Patronymic;
			this.IdGroup=other.IdGroup;
			this.Email=other.Email;

			this.Group=other.Group;
		}
	}
}