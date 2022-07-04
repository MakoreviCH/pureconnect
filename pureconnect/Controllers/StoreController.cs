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
        public List<Store> GetStore(int id)
        {
            string query = "SELECT ID, User_ID, Store_Name, Description, Location, Photo, Background_Image, Count_Followers, Count_Sales FROM Stores WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = id;
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
                }
                return null;
                //return store;
            }

        }

    }
}
