using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pureconnect.Models;
using System.Data.SqlClient;

namespace pureconnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorePostController : ControllerBase
    {
        private IConfiguration Configuration;
        public StorePostController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

       
        [HttpGet("byID")]
        public StorePost GetStorePost(int id)
        {
            string query = "SELECT ID, Store_ID, Product_Name, Price, Photos, Description FROM Store_Posts " +
                "WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = id;
                var reader = command.ExecuteReader();

                reader.Read();
                
                    StorePost p = new StorePost();
                    p.ID = Convert.ToInt32(reader.GetValue(0).ToString());
                    p.Store_ID = Convert.ToInt32(reader.GetValue(1).ToString());
                    p.Product_Name = reader.GetValue(2).ToString();
                    p.Price = Convert.ToInt32(reader.GetValue(3).ToString());
                    p.Photos = reader.GetValue(4).ToString();
                    p.Description = reader.GetValue(5).ToString();

                 
                

                return p;
            }

        }


        [HttpPut("update")]
        public ActionResult UpdateStorePost([FromBody] StorePostUpdate p)
        {
            string query = "UPDATE Store_Posts SET Product_Name = @Name, Description = @Description, Price = @Price, Photos = @Photos WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = p.ID;

                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Description"].Value = p.Description;

                command.Parameters.Add("@Price", System.Data.SqlDbType.Int);
                command.Parameters["@Price"].Value = p.Price;

                command.Parameters.Add("@Photos", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Photos"].Value = p.Photos;

                command.Parameters.Add("@Name", System.Data.SqlDbType.NChar);
                command.Parameters["@Name"].Value = p.Product_Name;
                
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

        [HttpPost("add")]
        public ActionResult AddStorePost([FromBody] StorePostAdd p)
        {
            string query = "INSERT INTO Store_Posts(Store_ID, Product_Name, Description, Price, Photos) VALUES (@Store_ID, @Product_Name, @Description, @Price, @Photos)";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@Store_ID", System.Data.SqlDbType.Int);
                command.Parameters["@Store_ID"].Value = p.Store_ID;

                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Description"].Value = p.Description;

                command.Parameters.Add("@Price", System.Data.SqlDbType.Int);
                command.Parameters["@Price"].Value = p.Price;

                command.Parameters.Add("@Photos", System.Data.SqlDbType.NVarChar);
                command.Parameters["@Photos"].Value = p.Photos;

                command.Parameters.Add("@Product_Name", System.Data.SqlDbType.NChar);
                command.Parameters["@Product_Name"].Value = p.Product_Name;
                

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

        [HttpDelete("delete")]
        public ActionResult DeletePost(int id)
        {
            string query = "Delete from Store_Posts WHERE ID = @ID";
            string connectionString = Configuration.GetConnectionString("PureDatabase");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                command.Parameters["@ID"].Value = id;

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


    }
}
