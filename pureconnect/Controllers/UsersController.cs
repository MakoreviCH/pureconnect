using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using BCrypt.Net;

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
        [HttpGet("byId")]
        public List<User> GetUser(string id)
        {
            string query = "SELECT * FROM Users WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

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
        [HttpPost]
        public ActionResult CreateUser([FromBody]User user)
        {
            int requestResult;
            string query = "INSERT INTO Users(ID, First_Name, Last_Name, Username, Password_hash, Registered_At, Last_Login, Intro, Description, Location, Mobile, Profile_Image) " +
                "Values(@ID, @First_Name, @Last_Name, @Username, @Password_hash, @Registered_At, @Last_Login, @Intro, @Description, @Location, @Mobile, @Profile_Image)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");
            var jsonResult = new StringBuilder();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlParameterCollection sqlParameter = command.Parameters;
                sqlParameter.AddRange(new SqlParameter[]{

                    new SqlParameter() { ParameterName = "@ID", SqlDbType = SqlDbType.NChar, Value = user.ID },
                    new SqlParameter() { ParameterName = "@First_name", SqlDbType = SqlDbType.NVarChar, Value = user.First_Name },
                    new SqlParameter() { ParameterName = "@Last_name", SqlDbType = SqlDbType.NVarChar, Value =  user.Last_Name},
                    new SqlParameter() { ParameterName = "@Username", SqlDbType = SqlDbType.NVarChar, Value = user.Username },
                    new SqlParameter() { ParameterName = "@Password_hash", SqlDbType = SqlDbType.NVarChar, Value = BCrypt.Net.BCrypt.HashPassword(user.Password_Hash, BCrypt.Net.BCrypt.GenerateSalt(12))},
                    new SqlParameter() { ParameterName = "@Registered_At", SqlDbType = SqlDbType.DateTime2, Value = user.Registered_At },
                    new SqlParameter() { ParameterName = "@Last_Login", SqlDbType = SqlDbType.DateTime2, Value = user.Last_Login },
                    new SqlParameter() { ParameterName = "@Intro", SqlDbType = SqlDbType.NVarChar, Value = user.Intro },
                    new SqlParameter() { ParameterName = "@Description", SqlDbType = SqlDbType.NVarChar, Value = user.Description },
                    new SqlParameter() { ParameterName = "@Mobile", SqlDbType = SqlDbType.NVarChar, Value = user.Mobile },
                    new SqlParameter() { ParameterName = "@Profile_Image", SqlDbType = SqlDbType.NChar, Value = user.Username },
                    new SqlParameter() { ParameterName = "@Location", SqlDbType = SqlDbType.NVarChar, Value = user.Location }


                });

				try
				{
                    requestResult = command.ExecuteNonQuery();
                }
				catch (Exception)
				{
                    return new StatusCodeResult(204);
                }
                

                
            }
            return new StatusCodeResult(200);


        }



    }
}
