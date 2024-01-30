using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.Models
{
	public class GymAccessRecord
	{
		[Key]
		public int AccessID { get; set; }
		
		public int ChipID { get; set; }
		
		public DateTime AccessTime { get; set; }

	}
}
