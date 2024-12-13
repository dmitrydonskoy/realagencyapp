using System;
using System.Collections.Generic;

namespace RealAgencyModels
{
    public partial class RealEstatePhoto
    {
        public int Id { get; set; }
        public string Filepath { get; set; } = null!;
        public int Realestateid { get; set; }

        public virtual Realestate Realestate { get; set; } = null!;
    }
}
