using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSchedule.Model.DTO
{
	public interface IDTO
	{
		bool ExistsInDataBase();
		bool HasReferenceOfNotExistingObject();
		void Restore();
	}
}
