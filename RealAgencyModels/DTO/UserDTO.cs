using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
	public class UserDTO
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Role { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		
	}
}
