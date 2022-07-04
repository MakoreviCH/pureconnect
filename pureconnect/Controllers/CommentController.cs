using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;

namespace pureconnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private IConfiguration Configuration;
        public CommentController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }


        [HttpGet("byId")]
        public List<Comment> GetComment(int post_ID)
        {
            string query = " SELECT Post_Comments.User_id, Users.Username, Users.Profile_Image, Post_Comments.ID,  Post_Comments.Post_ID, Post_Comments.Message FROM Post_Comments INNER JOIN Users ON Users.Id = Post_Comments.User_ID " +
                "WHERE Post_Comments.Post_id = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = post_ID;
                var reader = command.ExecuteReader();
                List<Comment> Comments = new List<Comment>();
                while (reader.Read())
                {
                    Comment c = new Comment();
                    c.User_id = reader.GetValue(0).ToString();
                    c.Username = reader.GetValue(1).ToString();
                    c.Profile_Image = reader.GetValue(2).ToString();
                    c.ID = Convert.ToInt32(reader.GetValue(3).ToString());
                    c.Post_ID = Convert.ToInt32(reader.GetValue(4).ToString());
                    c.Message = reader.GetValue(5).ToString();
                    

                    Comments.Add(c);
                }
                return Comments;

            }

        }

        [HttpPost("add")]
        public ActionResult AddComment(CommentAdd c)
        {
            string query = "INSERT INTO Post_Comments(User_ID, Post_ID, Message) VALUES(@User_id, @Post_id, @Message)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@User_id", System.Data.SqlDbType.NChar);
                command.Parameters["@User_id"].Value = c.User_id;

                command.Parameters.Add("@Post_id", System.Data.SqlDbType.Int);
                command.Parameters["@Post_id"].Value = c.Post_ID;

                command.Parameters.Add("@Message", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Message"].Value = c.Message;

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


    }
}
