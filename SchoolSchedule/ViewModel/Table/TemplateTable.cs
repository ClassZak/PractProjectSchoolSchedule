using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.ViewModel.Table
{
	public class TemplateTable<T> : ATable<T> where T : class, new()
	{
	}
}
