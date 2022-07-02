namespace pureconnect.Models
{
    public class Post
    {
        public int ID { get; set; }

        public char User_ID { get; set; }

        public string Text { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }

        public string Images { get; set; }

        public int Count_Likes { get; set; }

        public int Count_Comments { get; set; }

    }
}
