using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;

namespace pureconnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private IConfiguration Configuration;
        public ItemController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpPost("add")]
        public ActionResult AddItem([FromBody] ItemAdd p)
        {
            string query = "INSERT INTO Items (Author_ID, Item_Name, Description, Images, Type, Price)" +
                " VALUES (@Author_ID, @Item_Name, @Description, @Images, @Type, @Price)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@Author_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@Author_ID"].Value = p.User_Id;

                command.Parameters.Add("@Item_Name", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Item_Name"].Value = p.Item_Name;

                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Description"].Value = p.Description;

                command.Parameters.Add("@Images", System.Data.SqlDbType.NChar);
                command.Parameters["@Images"].Value = p.Images;

                command.Parameters.Add("@Type", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Type"].Value = p.Type;

                command.Parameters.Add("@Price", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Price"].Value = p.Price;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch
                {
                    return new StatusCodeResult(204);
                }

            }
            return new StatusCodeResult(200);

        }

        [HttpGet("filter")]
        public List<Item> GetItem(string user_id, string? type, string? ordering)
        {
            string construct = "";
            string query = "SELECT Items.ID, Users.Username, Items.Item_Name, Items.Images,Items.Type FROM Items INNER JOIN Users ON Items.Author_ID = Users.ID ";
            if (type != null)
            {
                if (type.ToLower() == "stickers")
                {
                    query += "WHERE Items.Type = 'stickers'";
                    construct += "WHERE Items.Type = 'stickers'";
                }
                else if (type.ToLower() == "frames")
                {
                    query += "WHERE Items.Type = 'frames'";
                    construct += "WHERE Items.Type = 'frames'";
                }
                else if (type.ToLower() == "backgrounds")
                {
                    query += "WHERE Items.Type = 'backgrounds'";
                    construct += "WHERE Items.Type = 'backgrounds'";
                }
            }


            if (ordering != null)
            {
                if (ordering.ToLower() == "priceDesc")
                {
                    query += " ORDER BY Items.Price DESC";
                }
                else if (ordering.ToLower() == "priceAsc")
                {
                    query += " ORDER BY Items.Price ASC";
                }
                else if (ordering.ToLower() == "newest")
                {
                    query += " ORDER BY Items.Created_At DESC";
                }
                else if (ordering.ToLower() == "popularity")
                {
                    query = $"SELECT Items.ID, Users.Username, Items.Item_Name, Items.Images,Items.Type, COUNT(User_Items.User_ID) AS Cnt FROM Items INNER JOIN Users ON Items.Author_ID = Users.ID INNER JOIN User_Items ON User_Items.Item_ID = Items.ID {construct} GROUP BY Items.ID, Users.Username, Items.Item_Name, Items.Images,Items.Type ORDER BY Cnt DESC";
                }
            }

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                List<Item> posts = new List<Item>();
                while (reader.Read())
                {
                    Item p = new Item();
                    p.ID = Convert.ToInt32(reader.GetValue(0).ToString());
                    p.User_Name = reader.GetValue(1).ToString();
                    p.Item_Name = reader.GetValue(2).ToString();
                    p.Images = reader.GetValue(3).ToString();
                    p.Type = reader.GetValue(4).ToString();
                    ItemBuy itemBuy = new ItemBuy();
                    itemBuy.Item_ID = p.ID;
                    itemBuy.User_ID = user_id;
                    p.IsBought = IsBought(itemBuy);

                    posts.Add(p);
                }


                return posts;
            }

        }

        [HttpPost("buy")]
        public ActionResult BuyItem([FromBody] ItemBuy p)
        {
            string query = "INSERT INTO User_Items (User_ID, Item_ID)" +
                " VALUES (@User_ID, @Item_ID)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@User_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@User_ID"].Value = p.User_ID;

                command.Parameters.Add("@Item_ID", System.Data.SqlDbType.Int);
                command.Parameters["@Item_ID"].Value = p.Item_ID;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch
                {
                    return new StatusCodeResult(204);
                }

            }
            return new StatusCodeResult(200);
        }

        [HttpGet("isBought")]
        public bool IsBought([FromBody] ItemBuy i)
        {
            string query = "SELECT User_ID FROM User_Items WHERE User_Id = @User_ID AND Item_ID = @Item_ID";

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.Add("@User_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@User_ID"].Value = i.User_ID;

                command.Parameters.Add("@Item_ID", System.Data.SqlDbType.Int);
                command.Parameters["@Item_ID"].Value = i.Item_ID;

                var reader = command.ExecuteReader();
                reader.Read();

                if (reader.HasRows)
                {
                    return true;
                }

                return false;
            }

        }

        [HttpGet("search")]
        public List<Item> GetSearchItem(string user_id, string? Search)
        {
            string query = $"SELECT Items.ID, Users.Username, Items.Item_Name, Items.Images,Items.Type FROM Items INNER JOIN Users ON Items.Author_ID = Users.ID WHERE Items.Item_Name LIKE '%'+ @Search +'%' ";

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.Add("@Search", System.Data.SqlDbType.NChar);
                command.Parameters["@Search"].Value = Search;

                var reader = command.ExecuteReader();
                List<Item> posts = new List<Item>();
                while (reader.Read())
                {
                    Item p = new Item();
                    p.ID = Convert.ToInt32(reader.GetValue(0).ToString());
                    p.User_Name = reader.GetValue(1).ToString();
                    p.Item_Name = reader.GetValue(2).ToString();
                    p.Images = reader.GetValue(3).ToString();
                    p.Type = reader.GetValue(4).ToString();
                    ItemBuy itemBuy = new ItemBuy();
                    itemBuy.Item_ID = p.ID;
                    itemBuy.User_ID = user_id;
                    p.IsBought = IsBought(itemBuy);

                    posts.Add(p);
                }


                return posts;
            }

        }

        [HttpGet("byID")]
        public ItemFull GetItemFull(string user_id, int item_id)
        {
            string query = $"SELECT Items.ID, Users.Username, Items.Item_Name, Items.Images,Items.Type, Items.Description FROM Items INNER JOIN Users ON Items.Author_ID = Users.ID WHERE Items.ID = @ID ";

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = item_id;

                var reader = command.ExecuteReader();
                
                reader.Read();
                ItemFull p = new ItemFull();
                p.ID = Convert.ToInt32(reader.GetValue(0).ToString());
                p.User_Name = reader.GetValue(1).ToString();
                p.Item_Name = reader.GetValue(2).ToString();
                p.Images = reader.GetValue(3).ToString();
                p.Type = reader.GetValue(4).ToString();
                p.Description = reader.GetValue(5).ToString();
                ItemBuy itemBuy = new ItemBuy();
                itemBuy.Item_ID = p.ID;
                itemBuy.User_ID = user_id;
                p.IsBought = IsBought(itemBuy);

                

                return p;
            }

        }

    }
}