﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
	public class AreaInfoDTO
	{
		public int Id { get; set; }
		public string Description { get; set; } = null!;
		public string Square { get; set; } = null!;
		public string Electricity { get; set; } = null!;
		public string Heating { get; set; } = null!;
		public string WaterSupply { get; set; } = null!;
		public string Gas { get; set; } = null!;
		public string Sewerage { get; set; } = null!;
		public int Realestateid { get; set; }
	}
}