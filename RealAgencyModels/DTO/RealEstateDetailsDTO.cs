using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
     public class RealEstateDetailsDTO
    {
        public int? RealEstateId { get; set; }
        public string? Address { get; set; } 
        public string? Rooms { get; set; } 
        public string? Type { get; set; } 
        public string? Square { get; set; }
        public string? Floor { get; set; }
        public string? Bathroom { get; set; } 
        public string? Repair { get; set; } 
        public string?  Furniture { get; set; } 
        public string? TransactionType { get; set; } 
        public decimal? Price { get; set; }
        public List<string> Photos { get; set; } = new();

        // Announcement data
        public string AnnouncementTitle { get; set; } = null!;
        public string AnnouncementDescription { get; set; } = null!;
        public int AnnouncementId { get; set; }
        // AreaInfo data
        public string? AreaName { get; set; } = null!;
        public string? AreaDescription { get; set; } = null!;
    }
}
