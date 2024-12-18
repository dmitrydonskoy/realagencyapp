namespace RealAgencyClientApp.Models
{
    public class AnnouncementListModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Type { get; set; } = null!;
        public List<string> Photos { get; set; } = new List<string>();
    }
}
