using System;
using System.Collections.Generic;

namespace RealAgencyModels
{
    public partial class Сooperation
    {
        public Сooperation()
        {
            Announcements = new HashSet<Announcement>();
        }

        public int Id { get; set; }
        public int Bidpartnerid { get; set; }
        public int Biduserid { get; set; }

        public virtual Bid Bid { get; set; } = null!;

        public virtual ICollection<Announcement> Announcements { get; set; }
    }
}
