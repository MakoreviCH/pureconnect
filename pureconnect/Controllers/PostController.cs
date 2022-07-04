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

        [HttpGet("byfriends")]
        public List<Post> GetFriendsPost(string id)
        {
            string query = "SELECT Posts.User_ID, Posts.ID, Users.Username, Users.Profile_Image, Posts.Text, Posts.Images, Posts.Count_Likes, Posts.Count_Comments FROM Posts INNER JOIN Users ON Users.ID = Posts.User_ID " +
                "WHERE Posts.User_ID IN(SELECT User_Friends.Target_ID AS User_id FROM User_Friends WHERE Source_ID = @ID  UNION ALL SELECT User_Friends.Source_ID AS User_id FROM User_Friends WHERE Target_ID = @ID AND Status = 1)";
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




        [HttpPut("update")]
        public ActionResult UpdatePost([FromBody]PostUpdate p)
        {
            string query = "UPDATE Posts SET Text = @Text, Images = @Images WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = p.ID;

                command.Parameters.Add("@Text", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Text"].Value = (object)p.Text ?? DBNull.Value;

                command.Parameters.Add("@Images", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Images"].Value = (object)p.Images ?? DBNull.Value;

                command.ExecuteNonQuery();
            }
            return new StatusCodeResult(200);

        }

        [HttpPost("add")]
        public ActionResult AddPost([FromBody] PostAdd p)
        {
            string query = "INSERT INTO Posts(User_ID, Text, Images) VALUES (@User_ID, @Text, @Images)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@User_ID", System.Data.SqlDbType.NChar);
                command.Parameters["@User_ID"].Value = p.User_ID;

                command.Parameters.Add("@Text", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Text"].Value = (object)p.Text ?? DBNull.Value;

                command.Parameters.Add("@Images", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Images"].Value = (object)p.Images ?? DBNull.Value;


                command.ExecuteNonQuery();

            }
            return new StatusCodeResult(200);

        }

        [HttpDelete("delete")]
        public ActionResult DeletePost(int id)
        {
            string query = "DELETE FROM Posts WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.NChar);
                command.Parameters["@ID"].Value = id;

                command.ExecuteNonQuery();
            }
            return new StatusCodeResult(200);

        }

        [HttpGet("checkLike")]
        public bool CheckLike(string userId, int postId)
        {
            string query = "SELECT COUNT(*) FROM Post_Likes WHERE User_ID = @User_ID AND Post_ID = @Post_ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@User_ID", System.Data.SqlDbType.NChar);
                command.Parameters.Add("@Post_ID", System.Data.SqlDbType.Int);

                command.Parameters["@User_ID"].Value = userId;
                command.Parameters["@Post_ID"].Value = postId;

                var reader = command.ExecuteReader();
                reader.Read();
                if (Convert.ToInt32(reader.GetValue(0)) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [HttpPut("updateLike")]
        public ActionResult UpdateLike(string userId, int postId)
        {
            string queryPost = "";
            string queryLike = "";
            if (CheckLike(userId, postId))
            {
                queryLike = "DELETE FROM Post_Likes WHERE User_ID = @User_ID AND Post_ID = @Post_ID";
            }
            else
            {
                queryLike = "INSERT INTO Post_Likes (User_ID, Post_ID) VALUES (@User_ID, @Post_ID)";
            }
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand commandPost = new SqlCommand(queryPost, connection);
                SqlCommand commandLike = new SqlCommand(queryLike, connection);

                connection.Open();


                commandLike.Parameters.Add("@Post_ID", System.Data.SqlDbType.Int);
                commandLike.Parameters["@Post_ID"].Value = postId;
                commandLike.Parameters.Add("@User_ID", System.Data.SqlDbType.NChar);
                commandLike.Parameters["@User_ID"].Value = userId;

                var readerLike = commandLike.ExecuteNonQuery();
            }

            return new StatusCodeResult(200);
        }
    }

}

