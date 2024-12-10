using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
	public class ProfileDTO
	{
		public int Userid { get; set; }
		public int Id { get; set; }
		public string Experience { get; set; } = null!;
		public string Transactions { get; set; } = null!;
		public string Percent { get; set; } = null!;
		public string Description { get; set; } = null!;
	}
}
