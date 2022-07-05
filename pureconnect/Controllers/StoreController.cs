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

        [HttpGet("byId")]
        public Store GetStore(int Store_ID, string User_ID)
        {
            string query = "SELECT Stores.ID, Stores.User_ID, Stores.Store_Name, Stores.Description, Stores.Location, Stores.Photo, Stores.Background_Image, " +
                "Stores.Count_Followers, Stores.Count_Sales FROM Stores WHERE ID = @Store_ID";

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@Store_ID", System.Data.SqlDbType.Int);
                command.Parameters["@Store_ID"].Value = Store_ID;

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
                    if (IsFollowed(Store_ID, User_ID))
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
        public bool IsFollowed(int Store_ID, string User_ID)
        {
            string query = "SELECT Stores.ID, Stores.User_ID, Stores.Store_Name, Stores.Description, Stores.Location, Stores.Photo, Stores.Background_Image, Stores.Count_Followers, Stores.Count_Sales FROM Stores INNER JOIN Store_Follows ON Stores.ID = Store_Follows.Store_Id WHERE Store_Follows.User_ID = @User_ID AND Store_Follows.Store_ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = Store_ID;

                command.Parameters.Add("@User_ID", System.Data.SqlDbType.VarChar);
                command.Parameters["@User_ID"].Value = User_ID;

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
        public List<StorePosts> GetPostsByStoreId(int Store_ID)
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
                command.Parameters["@Store_ID"].Value = Store_ID;

                var reader = command.ExecuteReader();
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

        [HttpPost("add")]
        public ActionResult AddStore(StoreAdd s)
        {
            string query = "INSERT INTO Stores(Photo, Store_Name, Description, Location, Background_Image, User_ID, License) VALUES(@Photo, @Store_Name, @Description, @Location, @Background_Image, @User_ID, @License)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@Photo", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Photo"].Value = s.Photo;

                command.Parameters.Add("@Store_Name", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Store_Name"].Value = s.Store_Name;

                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Description"].Value = s.Description;

                command.Parameters.Add("@Location", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Location"].Value = s.Location;

                command.Parameters.Add("@Background_Image", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Background_Image"].Value = s.Background_Photo;

                command.Parameters.Add("@User_ID", System.Data.SqlDbType.NVarChar);
                command.Parameters["@User_ID"].Value = s.User_ID;

                command.Parameters.Add("@License", System.Data.SqlDbType.NVarChar);
                command.Parameters["@License"].Value = s.License;

                try
                {
                    var reader = command.ExecuteNonQuery();
                }
                catch
                {
                    return new StatusCodeResult(204);
                }


                return new StatusCodeResult(200);

            }

        }

        [HttpPut("update")]
        public ActionResult UpdatePost([FromBody] StoreUpdate s)
        {
            string query = "UPDATE Store SET Photo = @Photo, Store_Name = @Store_Name, Description = @Description, Location = @Location, Background_Image = @Background_Image WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@Photo", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Photo"].Value = s.Photo;

                command.Parameters.Add("@Store_Name", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Store_Name"].Value = s.Store_Name;

                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Description"].Value = s.Description;

                command.Parameters.Add("@Location", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Location"].Value = s.Location;

                command.Parameters.Add("@Background_Image", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Background_Image"].Value = s.Background_Image;

                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = s.Id;

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

        [HttpGet("followedStoresByUser")]
        public List<StoreForSearch> GetFollowedStoresByUserId(string User_Id)
        {
            string query = "SELECT Store_Follows.Store_ID, Stores.Store_Name, Stores.Description, Stores.Photo FROM Stores INNER JOIN Store_Follows ON Stores.ID = Store_Follows.Store_ID WHERE Store_Follows.User_ID = @User_ID";

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@User_ID", System.Data.SqlDbType.NVarChar);
                command.Parameters["@User_ID"].Value = User_Id;

                var reader = command.ExecuteReader();
                StoreForSearch storeForSearch = new StoreForSearch();
                List<StoreForSearch> list = new List<StoreForSearch>();
                while (reader.Read())
                {
                    storeForSearch.Id = Convert.ToInt32(reader.GetValue(0));
                    storeForSearch.Store_Name = reader.GetValue(1).ToString();
                    storeForSearch.Description = reader.GetValue(2).ToString();
                    storeForSearch.Photo = reader.GetValue(3).ToString();

                    list.Add(storeForSearch);
                }

                return list;

            }
        }

        [HttpGet("searchByName")]
        public List<SearchStoreToUser> GetStoresByName(string Name, string User_Id)
        {
            string query = "SELECT ID, Store_Name, Description, Photo FROM Stores WHERE Store_Name LIKE @Store_Name + '%'";

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.Add("@Store_Name", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Store_Name"].Value = Name;

                var reader = command.ExecuteReader();
                List<SearchStoreToUser> list = new List<SearchStoreToUser>();
                while (reader.Read())
                {
                    SearchStoreToUser storeForSearch = new SearchStoreToUser();
                    storeForSearch.Id = Convert.ToInt32(reader.GetValue(0));
                    storeForSearch.Store_Name = reader.GetValue(1).ToString();
                    storeForSearch.Description = reader.GetValue(2).ToString();
                    storeForSearch.Photo = reader.GetValue(3).ToString();

                    if (IsFollowed(storeForSearch.Id, User_Id))
                    {
                        storeForSearch.IsFollowerd = true;
                    }
                    else
                    {
                        storeForSearch.IsFollowerd = false;
                    }

                    list.Add(storeForSearch);
                }

                return list;

            }

        }
        [HttpDelete("delete")]
        public ActionResult DeleteStore(int Id)
        {
            string query = "DELETE FROM Stores WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = Id;

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
    }
}
