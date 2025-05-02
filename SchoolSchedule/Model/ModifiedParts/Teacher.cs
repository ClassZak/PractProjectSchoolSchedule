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
			this.Gender= other.Gender;
			this.BirthDay =other.BirthDay;
		}
		public override string ToString()
		{
			return $"{Surname} {Name} {Patronymic}";
		}
	}
}