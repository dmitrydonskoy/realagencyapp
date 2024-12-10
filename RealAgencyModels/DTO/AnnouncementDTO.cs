using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
	public class AnnouncementDTO
	{
		public int Id { get; set; }
		public string Type { get; set; } = null!;
		public string Description { get; set; } = null!;
		public int Userid { get; set; }

		

	}
}
