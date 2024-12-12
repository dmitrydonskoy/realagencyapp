namespace RealAgencyClientApp.Models
{
	public class UserRegisterModel
	{
       public int Id { get; set; } = 0;
        public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Role { get; set; } = "User";
	}
}
