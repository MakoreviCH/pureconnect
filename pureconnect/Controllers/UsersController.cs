using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;
using System.Text;

namespace pureconnect.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IConfiguration Configuration;
        public UsersController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet]
        [Route("api/[controller]")]
        public List<User> GetUser()
        {
            string query = "SELECT * FROM Users";
            string connectionString = Configuration.GetConnectionString("PureDatabase");
            var jsonResult = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                List<User> users = new List<User>();
                while (reader.Read())
                {
                    User u = new User();
                    u.ID = reader.GetValue(0).ToString();
                    u.First_Name=reader.GetValue(1).ToString();
                    u.Last_Name=reader.GetValue(2).ToString();
                    u.Username=reader.GetValue(3).ToString();
                    u.Password_Hash=reader.GetValue(4).ToString();
                    u.Registered_At=DateTime.Parse( reader.GetValue(5).ToString());
                    u.Last_Login= DateTime.Parse(reader.GetValue(6).ToString());
                    u.Intro=reader.GetValue(7).ToString();
                    u.Description=reader.GetValue(8).ToString();
                    u.Location=reader.GetValue(8).ToString();
                    u.Mobile=reader.GetValue(8).ToString();
                    u.Profile_Image=reader.GetValue(8).ToString();

                    users.Add(u);
                }

                return users;
            }

        }
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public List<User> GetUser(string id)
        {
            string query = "SELECT * FROM Users WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");
            var jsonResult = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID",System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = id;
                var reader = command.ExecuteReader();
                List<User> users = new List<User>();
                while (reader.Read())
                {
                    User u = new User();
                    u.ID = reader.GetValue(0).ToString();
                    u.First_Name = reader.GetValue(1).ToString();
                    u.Last_Name = reader.GetValue(2).ToString();
                    u.Username = reader.GetValue(3).ToString();
                    u.Password_Hash = reader.GetValue(4).ToString();
                    u.Registered_At = DateTime.Parse(reader.GetValue(5).ToString());
                    u.Last_Login = DateTime.Parse(reader.GetValue(6).ToString());
                    u.Intro = reader.GetValue(7).ToString();
                    u.Description = reader.GetValue(8).ToString();
                    u.Location = reader.GetValue(8).ToString();
                    u.Mobile = reader.GetValue(8).ToString();
                    u.Profile_Image = reader.GetValue(8).ToString();

                    users.Add(u);
                }

                return users;
            }


        }
    }
}
