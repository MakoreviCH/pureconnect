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
		private IConfiguration Configuration;
		public FriendsController(IConfiguration _configuration)
		{
			Configuration = _configuration;
		}


        [HttpGet("byID")]
        public List<UserList> GetUserFriends(string id, bool status)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT Users.ID, Users.Username, Users.First_Name, Users.Last_Name, Users.Profile_Image FROM Users WHERE ID IN ");

            if (status)
                query.Append("(SELECT Source_ID FROM User_Friends WHERE Status = 0 AND Target_ID = @ID)");
            else
                query.Append("(SELECT Source_ID FROM User_Friends WHERE Status = 1 AND Target_ID = @ID " +
                    "UNION ALL " +
                    "SELECT Target_ID FROM User_Friends WHERE Status = 1 AND Source_ID = @ID)");
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query.ToString(), connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = id;
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

        public int GetFriendStatus(string User_ID, string Profile_ID)
		{
            int result = -1;
            StringBuilder query = new StringBuilder();
            query.Append("BEGIN IF EXISTS(SELECT * FROM User_Friends WHERE Target_ID = @Profile_ID AND Source_ID = @Usr_ID OR Target_ID = @Usr_ID AND Source_ID = @Profile_ID) "+
                            "BEGIN IF EXISTS (SELECT * FROM User_Friends WHERE Target_ID=@Profile_ID AND Source_ID = @Usr_ID) " +
                                "BEGIN SELECT Status FROM User_Friends WHERE Target_ID = @Profile_ID AND Source_ID = @Usr_ID END" +
                            "ELSE BEGIN SELECT Status+2 FROM User_Friends WHERE Target_ID=@Usr_ID AND Source_ID = @Profile_ID END"+
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
                    result= reader.GetInt32(0);
                }

                return result;
            }

        }

    }
}
