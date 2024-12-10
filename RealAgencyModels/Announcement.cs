using System;
using System.Collections.Generic;

namespace RealAgencyModels
{
    public partial class Announcement
    {
        public Announcement()
        {
            Realestates = new HashSet<Realestate>();
            Сooperations = new HashSet<Сooperation>();
        }

        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Userid { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Realestate> Realestates { get; set; }

        public virtual ICollection<Сooperation> Сooperations { get; set; }
    }
}
