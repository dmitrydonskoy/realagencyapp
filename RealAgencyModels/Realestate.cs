using System;
using System.Collections.Generic;

namespace RealAgencyModels
{
    public partial class Realestate
    {
        public Realestate()
        {
            AreaInfos = new HashSet<AreaInfo>();
        }

        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string Rooms { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Square { get; set; } = null!;
        public string? Floor { get; set; }
        public string Bathroom { get; set; } = null!;
        public string Repair { get; set; } = null!;
        public string Furniture { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public int Announcementid { get; set; }

        public virtual Announcement Announcement { get; set; } = null!;
        public virtual ICollection<AreaInfo> AreaInfos { get; set; }
    }
}
