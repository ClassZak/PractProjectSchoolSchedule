namespace SchoolSchedule.Model
{
	public partial class Teacher
	{
		public Teacher(Teacher other) : this()
		{
			this.Id= other.Id;
			this.Name= other.Name;
			this.Surname= other.Surname;
			this.Patronymic= other.Patronymic;

			/*
			foreach(var el in Schedule)
				this.Schedule.Add(el);
			foreach(var el in TeacherPhone)
				this.TeacherPhone.Add(el);
			foreach(var el in Group)
				this.Group.Add(el);
			foreach(var el in Subject)
				this.Subject.Add(el);
			*/
		}
		public override string ToString()
		{
			return $"{Surname} {Name} {Patronymic}";
		}
	}
}