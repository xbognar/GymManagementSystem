using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.DTOs
{
	public class InactiveChipDTO
	{

		public int ChipID { get; set; }
		
		public string? OwnerFullName { get; set; }
		
		public string? ChipInfo { get; set; }

	}
}
