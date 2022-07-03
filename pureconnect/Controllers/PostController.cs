using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;

namespace pureconnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private IConfiguration Configuration;
        public PostController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet("byId")]
        public List<Post> GetPost(string id)
        {
            string query = "SELECT Posts.User_ID, Posts.ID, Users.Username, Users.Profile_Image, Posts.Text, Posts.Images, Posts.Count_Likes, Posts.Count_Comments FROM Posts INNER JOIN Users ON Users.ID = Posts.User_ID " +
                "WHERE Posts.User_ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = id;
                var reader = command.ExecuteReader();
                List<Post> posts = new List<Post>();
                while (reader.Read())
                {
                    Post p = new Post();
                    p.ID = Convert.ToInt32(reader.GetValue(1).ToString());
                    p.User_ID = reader.GetValue(0).ToString();
                    p.Username = reader.GetValue(2).ToString();
                    p.Profile_Image = reader.GetValue(3).ToString();
                    p.Text = reader.GetValue(4).ToString();
                    p.Images = reader.GetValue(5).ToString();
                    p.Count_Likes = Convert.ToInt32(reader.GetValue(6).ToString());
                    p.Count_Comments = Convert.ToInt32(reader.GetValue(7).ToString());

                    posts.Add(p);
                }

                return posts;
            }

        }
    }
}
