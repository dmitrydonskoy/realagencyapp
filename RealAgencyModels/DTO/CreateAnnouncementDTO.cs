using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.DTO
{
    public class CreateAnnouncementDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public RealstateDTO RealEstate { get; set; }
        public AreaInfoDTO? AreaInfo { get; set; }
    }
}
