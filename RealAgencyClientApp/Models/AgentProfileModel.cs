namespace RealAgencyClientApp.Models
{
    public class AgentProfileModel
    {
        public int Userid { get; set; }
        public int Id { get; set; }
        public string Experience { get; set; } = null!;
        public string Transactions { get; set; } = null!;
        public string Percent { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
