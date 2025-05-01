namespace SchoolSchedule.Model
{
	partial class BellScheduleType
	{
		public BellScheduleType(BellScheduleType other) : this()
		{
			this.Id= other.Id;
			this.Name = other.Name;
		}
		public override string ToString()
		{
			return $"{Name}";
		}
	}
}