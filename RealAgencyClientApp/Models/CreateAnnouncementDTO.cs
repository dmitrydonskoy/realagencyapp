namespace RealAgencyClientApp.Models
{
    public class CreateAnnouncementDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Userid { get; set; }
        public RealEstateDTO RealEstate { get; set; }
        public AreaInfoDTO? AreaInfo { get; set; }
    }
}
