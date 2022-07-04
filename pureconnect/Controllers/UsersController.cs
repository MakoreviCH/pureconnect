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
        [HttpGet("byID")]
        public List<User> GetUsers()
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
                    u.First_Name = reader.GetValue(1).ToString();
                    u.Last_Name = reader.GetValue(2).ToString();
                    u.Username = reader.GetValue(3).ToString();
                    u.Email = reader.GetValue(4).ToString();
                    u.Password_Hash = reader.GetValue(5).ToString();
                    u.Registered_At = DateTime.Parse(reader.GetValue(6).ToString());
                    u.Last_Login = DateTime.Parse(reader.GetValue(7).ToString());
                    u.Intro = reader.GetValue(8).ToString();
                    u.Description = reader.GetValue(9).ToString();
                    u.Location = reader.GetValue(10).ToString();
                    u.Mobile = reader.GetValue(11).ToString();
                    u.Profile_Image = reader.GetValue(12).ToString();
                    u.Count_Requests = Int32.Parse( reader.GetValue(13).ToString());
                    u.Count_Friends = Int32.Parse(reader.GetValue(14).ToString());
                    u.Background_Image = reader.GetValue(15).ToString();

                    users.Add(u);
                }

                return users;
            }

        }
        [HttpGet("byName")]
        public List<UserList> GetUsers(string input)
        {
            string query = "SELECT Users.ID, Users.Username, Users.First_Name, Users.Last_Name, Users.Profile_Image FROM Users " +
				"WHERE Users.Username LIKE @Name+'%' OR Users.First_Name LIKE @Name+'%' OR Users.Last_Name LIKE @Name+'%'";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query.ToString(), connection);
                connection.Open();
                command.Parameters.Add("@Name", System.Data.SqlDbType.NChar);
                command.Parameters["@Name"].Value = input;
                var reader = command.ExecuteReader();
                List<UserList> users = new List<UserList>();
                while (reader.Read())
                {
                    UserList u = new UserList();
                    u.ID = reader.GetValue(0).ToString();
                    u.Username = reader.GetValue(1).ToString();
                    u.First_Name = reader.GetValue(2).ToString();
                    u.Last_Name = reader.GetValue(3).ToString();
                    u.Profile_Image = reader.GetValue(4).ToString();

                    users.Add(u);
                }

                return users;
            }

        }

        

        [HttpGet]
        [Route("profile")]
        public UserProfile GetUserProfile(string id)
        {
            string query = "SELECT ID, Username, Intro, Description, Location, Count_Requests, Count_Friends, Profile_Image, Background_Image FROM users WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = id;
                var reader = command.ExecuteReader();
                UserProfile u = new UserProfile();
                while (reader.Read())
                {
                    u = new UserProfile();
                    u.ID = reader.GetValue(0).ToString();
                    u.Username = reader.GetValue(1).ToString();
                    u.Intro = reader.GetValue(2).ToString();
                    u.Description = reader.GetValue(3).ToString();
                    u.Location = reader.GetValue(4).ToString();
                    u.Count_Requests = Int32.Parse(reader.GetValue(5).ToString());
                    u.Count_Friends = Int32.Parse(reader.GetValue(6).ToString());
                    u.Profile_Image = reader.GetValue(7).ToString();
                    u.Background_Image = reader.GetValue(8).ToString();
                    u.Friend_Status = 

                }

                return u;
            }

        }

        
        [HttpPost("create")]
        public ActionResult CreateUser([FromBody] UserCreate user)
        {
            int requestResult;
            string query = "INSERT INTO Users(ID, First_Name, Last_Name, Username, Email, Password_hash, Mobile) " +
                "Values(@ID, @First_Name, @Last_Name, @Username,@Email, @Password_hash, @Mobile)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");
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
                    new SqlParameter() { ParameterName = "@Email", SqlDbType = SqlDbType.NVarChar, Value = user.Email },
                    new SqlParameter() { ParameterName = "@Password_hash", SqlDbType = SqlDbType.NVarChar, Value = BCrypt.Net.BCrypt.HashPassword(user.Password_Hash, BCrypt.Net.BCrypt.GenerateSalt(12))},
                    new SqlParameter() { ParameterName = "@Mobile", SqlDbType = SqlDbType.NVarChar, Value = user.Mobile },

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

        [HttpPut("update")]
        public ActionResult UpdateUserProfile(string id, [FromBody] UserUpdateProfile user)
        {
            int requestResult;
            string query = "UPDATE Users SET Username=@Username, First_Name=@First_Name, Last_Name = @Last_name, Intro=@Intro, " +
                "Description=@Description, Location = @location,Profile_Image = @Profile_Image WHERE ID =@ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@ID", System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = id;
                connection.Open();
                SqlParameterCollection sqlParameter = command.Parameters;
                sqlParameter.AddRange(new SqlParameter[]{

                    new SqlParameter() { ParameterName = "@First_name", SqlDbType = SqlDbType.NVarChar, Value = user.First_Name },
                    new SqlParameter() { ParameterName = "@Last_name", SqlDbType = SqlDbType.NVarChar, Value =  user.Last_Name},
                    new SqlParameter() { ParameterName = "@Username", SqlDbType = SqlDbType.NVarChar, Value = user.Username},
                    new SqlParameter() { ParameterName = "@Intro", SqlDbType = SqlDbType.NVarChar, Value = (object)user.Intro??DBNull.Value },
                    new SqlParameter() { ParameterName = "@Description", SqlDbType = SqlDbType.NVarChar, Value = (object)user.Description??DBNull.Value },
                    new SqlParameter() { ParameterName = "@Profile_Image", SqlDbType = SqlDbType.NChar, Value = user.Profile_Image },
                    new SqlParameter() { ParameterName = "@Location", SqlDbType = SqlDbType.NVarChar, Value = (object)user.Location??DBNull.Value },

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
        [HttpDelete("delete")]
        public ActionResult DeleteUser(string id)
        {
            int requestResult;
            string query = "DELETE FROM Users WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@ID", System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = id;
                connection.Open();
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
