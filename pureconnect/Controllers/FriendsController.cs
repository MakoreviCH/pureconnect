using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;
using System.Text;
using System.Data;

namespace pureconnect.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FriendsController : ControllerBase
	{
		private readonly IConfiguration Configuration;
		public FriendsController(IConfiguration _configuration)
		{
			Configuration = _configuration;
		}

		[HttpPost("add")]
        public ActionResult AddFriend(string source_id, string target_id)
        {
            StringBuilder query = new();
            int status = GetFriendStatus(source_id, target_id);
            if (status == 4)
            {
                query.Append("INSERT INTO User_Friends(Source_ID, Target_ID) VALUES(@Source_ID, @Target_ID)");
            }
            else if (status == 2)
            {
                query.Append("UPDATE User_Friends SET Status=1 WHERE Target_ID=@Source_ID AND Source_ID = @Target_ID");
            }

            string connectionString = Configuration.GetConnectionString("PureDatabase");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand(query.ToString(), connection);
                command.Parameters.Add("@Source_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@Source_ID"].Value = source_id;
                command.Parameters.Add("@Target_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@Target_ID"].Value = target_id;
                connection.Open();

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return new StatusCodeResult(204);
                }


            }
            return new StatusCodeResult(200);
        }


		[HttpGet]
        public List<UserList> GetUserFriends(string user_id, bool list_type)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT Users.ID, Users.Username, Users.First_Name, Users.Last_Name, Users.Profile_Image FROM Users WHERE ID IN ");

            if (list_type)   
                query.Append("(SELECT Source_ID FROM User_Friends WHERE Status = 1 AND Target_ID = @ID " +
                              "UNION ALL " +
                              "SELECT Target_ID FROM User_Friends WHERE Status = 1 AND Source_ID = @ID)");
            else
                query.Append("(SELECT Source_ID FROM User_Friends WHERE Status = 0 AND Target_ID = @ID)");

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query.ToString(), connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = user_id;
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
        [HttpGet("status")]
        public int GetFriendStatus(string User_ID, string Profile_ID)
		{
            int result = -1;
            StringBuilder query = new StringBuilder();
            query.Append("BEGIN IF EXISTS(SELECT * FROM User_Friends WHERE Target_ID = @Profile_ID AND Source_ID = @Usr_ID OR Target_ID = @Usr_ID AND Source_ID = @Profile_ID) "+
                            "BEGIN IF EXISTS (SELECT * FROM User_Friends WHERE Target_ID=@Profile_ID AND Source_ID = @Usr_ID) " +
                                "BEGIN SELECT CAST(Status as INT) FROM User_Friends WHERE Target_ID = @Profile_ID AND Source_ID = @Usr_ID END " +
                            "ELSE BEGIN SELECT CAST(Status as INT)+2 FROM User_Friends WHERE Target_ID=@Usr_ID AND Source_ID = @Profile_ID END " +
                         "END ELSE BEGIN SELECT DISTINCT(4) FROM User_Friends; END END");

            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query.ToString(), connection);
                connection.Open();

                command.Parameters.Add("@Profile_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@Profile_ID"].Value = Profile_ID;

                command.Parameters.Add("@Usr_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@Usr_ID"].Value = User_ID;

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result= Int32.Parse(reader.GetValue(0).ToString());
                }

                return result;
            }

        }

    }
}
