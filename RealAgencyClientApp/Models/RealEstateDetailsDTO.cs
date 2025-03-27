namespace RealAgencyClientApp.Models
{
    public class RealEstateDetailsDTO
    {
        public int RealEstateId { get; set; }
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
        public List<string> Photos { get; set; } = new();

        // Announcement data
        public string AnnouncementTitle { get; set; } = null!;
        public string AnnouncementDescription { get; set; } = null!;

        // AreaInfo data
        public string AreaDescription { get; set; } = null!;
    }
}
