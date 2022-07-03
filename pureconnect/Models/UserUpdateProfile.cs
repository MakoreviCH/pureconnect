namespace pureconnect.Models
{
	public class UserUpdateProfile
	{
		public string ID { get; set; }

		public string First_Name { get; set; }

		public string Last_Name { get; set; }

		public string Username { get; set; }

		public string? Intro { get; set; }

		public string? Description { get; set; }

		public string? Location { get; set; }

		public string Profile_Image { get; set; }
	}
}
