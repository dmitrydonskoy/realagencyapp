using System;
using System.Collections.Generic;

namespace RealAgencyModels
{
    public partial class Bid
    {
        public Bid()
        {
            Сooperations = new HashSet<Сooperation>();
        }

        public int Partnerid { get; set; }
        public int Userid { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Сooperation> Сooperations { get; set; }
    }
}
