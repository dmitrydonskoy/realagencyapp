using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
    public class AnnouncementDescription
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Type { get; set; } = null!;
        public List<string> Photos { get; set; } = new List<string>();

    }
}
