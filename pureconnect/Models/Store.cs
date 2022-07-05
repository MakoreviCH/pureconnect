namespace pureconnect.Models
{
    public class Store
    {
        public int ID { get; set; }
        public string User_ID { get; set; }
        public string Store_Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Photo { get; set; }
        public string Background_Image { get; set; }
        public int Count_Followers { get; set; }
        public int Count_Sales { get; set; }
        public bool Is_Followed { get; set; }
    }
}
