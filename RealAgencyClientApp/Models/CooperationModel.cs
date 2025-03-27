namespace RealAgencyClientApp.Models
{
    public class CooperationModel
    {
        public int Id { get; set; }
        public int BidPartnerId { get; set; } // ID агента
        public int BidUserId { get; set; } // ID клиента
        public string? ClientName { get; set; }
    }
}
