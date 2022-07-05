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

        [HttpGet("byId")]
        public List<Item> GetPost(string type = "", string ordering = "")  
        {
            string query = "SELECT Items.ID, Users.Username, Items.Item_Name, Items.Images, Items.Type FROM Items INNER JOIN Users ON Items.Author_ID = Users.ID";
            
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
                    
                    posts.Add(p);
                }

                return posts;
            }

        }
    }
}
