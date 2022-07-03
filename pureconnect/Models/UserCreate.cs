namespace pureconnect.Models
{
	public class UserCreate
	{
		public string ID { get; set; }

		public string First_Name { get; set; }

		public string Last_Name { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

		public string Password_Hash { get; set; }

		public string? Mobile { get; set; }
	}
}
