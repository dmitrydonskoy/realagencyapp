using System;
using System.Collections.Generic;

namespace RealAgencyModels
{
    public partial class User
    {
        public User()
        {
            Announcements = new HashSet<Announcement>();
            Bids = new HashSet<Bid>();
            Profiles = new HashSet<Profile>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }

        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
    }
}
