using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
    public class RealEstatePhotoDTO
    {
        public int Id { get; set; }
        public string Filepath { get; set; } = null!;
        public int Realestateid { get; set; }

    }
}
