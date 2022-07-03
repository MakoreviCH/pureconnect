namespace pureconnect.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string User_ID { get; set; }
        public string Username { get; set; }
        public string Profile_Image { get; set; }
        public string? Text { get; set; }
        public string? Images { get; set; }
        public int Count_Likes { get; set; }
        public int Count_Comments { get; set; }
    }
}
