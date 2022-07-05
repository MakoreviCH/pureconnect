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
    }
}
