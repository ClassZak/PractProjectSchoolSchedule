namespace SchoolSchedule.Model
{
	public partial class Group
	{
		public Group(Group other) : this()
		{
			this.Id = other.Id ;
			this.Name = other.Name ;
			this.Year = other.Year;
		}
		public override string ToString()
		{
			return $"{Year}{Name}";
		}
	}
}
