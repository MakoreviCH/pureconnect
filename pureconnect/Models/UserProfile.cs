namespace pureconnect.Models
{
	public class UserProfile
	{
        public string ID { get; set; }

        public string Username { get; set; }

        public string Intro { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public int Count_Requests { get; set; }

        public int Count_Friends { get; set; }

        public string Profile_Image { get; set; }

        public string Background_Image { get; set; }
    }
}
