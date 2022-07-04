using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;

namespace pureconnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private IConfiguration Configuration;
        public StoreController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet("storeById")]
        public Store GetStore(int storeId, string userId)
        {
            string query = "SELECT Stores.ID, Stores.User_ID, Stores.Store_Name, Stores.Description, Stores.Location, Stores.Photo, Stores.Background_Image, " +
                "Stores.Count_Followers, Stores.Count_Sales FROM Stores WHERE ID = @Store_ID";

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@Store_ID", System.Data.SqlDbType.Int);
                command.Parameters["@Store_ID"].Value = storeId;

                var reader = command.ExecuteReader();
                Store store = new Store();

                while (reader.Read())
                {
                    store.ID = Convert.ToInt32(reader.GetValue(0));
                    store.User_ID = reader.GetValue(1).ToString();
                    store.Store_Name = reader.GetValue(2).ToString();
                    store.Description = reader.GetValue(3).ToString();
                    store.Location = reader.GetValue(4).ToString();
                    store.Photo = reader.GetValue(5).ToString();
                    store.Background_Image = reader.GetValue(6).ToString();
                    store.Count_Followers = Convert.ToInt32(reader.GetValue(7));
                    store.Count_Sales = Convert.ToInt32(reader.GetValue(8));
                    if (IsFollowed(storeId, userId))
                    {
                        store.Is_Followed = true;
                    }
                    else
                    {
                        store.Is_Followed = false;
                    }
                }

                return store;
            }

        }

        [HttpGet("isFollowed")]
        public bool IsFollowed(int storeId, string userId)
        {
            string query = "SELECT Stores.ID, Stores.User_ID, Stores.Store_Name, Stores.Description, Stores.Location, Stores.Photo, Stores.Background_Image, Stores.Count_Followers, Stores.Count_Sales FROM Stores INNER JOIN Store_Follows ON Stores.ID = Store_Follows.Store_Id WHERE Store_Follows.User_ID = @User_ID AND Store_Follows.Store_ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = storeId;

                command.Parameters.Add("@User_ID", System.Data.SqlDbType.VarChar);
                command.Parameters["@User_ID"].Value = userId;

                var reader = command.ExecuteReader();
                
                reader.Read();
                

                if (reader.HasRows)
                {
                    return true;
                }

                return false;
            }
        }

        [HttpGet("postsByStoreId")]
        public List<StorePosts> GetPostsByStoreId(int storeId)
        {
            string query = "SELECT ID, Store_ID, Product_Name, Price, Photos FROM Store_Posts WHERE Store_ID = @Store_ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");
            StorePosts storePosts = new StorePosts();
            List<StorePosts> posts = new List<StorePosts>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@Store_ID", System.Data.SqlDbType.Int);
                command.Parameters["@Store_ID"].Value = storeId;

                var reader = command.ExecuteReader();
                reader.Read();
                while (reader.Read())
                {
                    storePosts.ID = Convert.ToInt32(reader.GetValue(0));
                    storePosts.Store_ID = Convert.ToInt32(reader.GetValue(1));
                    storePosts.ProductName = reader.GetValue(2).ToString();
                    storePosts.Price = Convert.ToInt32(reader.GetValue(3));
                    storePosts.Photos = reader.GetValue(4).ToString();
                    posts.Add(storePosts);
                }
            }
            return posts;
        }

    }
}
