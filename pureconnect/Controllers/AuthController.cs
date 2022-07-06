using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using MimeKit;
using MailKit.Net.Smtp;

namespace pureconnect.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration Configuration;
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		SmtpClient client;
		public AuthController(IConfiguration _configuration)
		{
			Configuration = _configuration;

		}
		[HttpPost("sendcode")]
		public ActionResult SendCode(string adress, string token)
		{
			Random rnd = new();
			StringBuilder mailText = new();
			mailText.Append("To confirm enter this code: ");

			string code = new(Enumerable.Repeat(chars, 10).Select(s => s[rnd.Next(s.Length)]).ToArray());
			mailText.Append("<b>" + code + "</b>");

			string query = "INSERT INTO Auth(Token, Code) VALUES(@Token, @Code)";
			string connectionString = Configuration.GetConnectionString("PureDatabase");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.Add("@Token", System.Data.SqlDbType.NChar);
				command.Parameters["@Token"].Value = token;
				command.Parameters.Add("@Code", System.Data.SqlDbType.NChar);
				command.Parameters["@Code"].Value = code;
				connection.Open();

				try
				{
					command.ExecuteNonQuery();
					return SendEmail(adress, mailText.ToString());
				}
				catch (Exception)
				{
					return new StatusCodeResult(204);
				}
			}

			

		}

		[HttpGet("confirm")]
		public ActionResult ConfirmCode(string token, string code)
		{
			string query = "BEGIN DECLARE @ReturnCode sysname; " +
				"IF Exists(SELECT * FROM Auth WHERE Token = @Token AND Code=@Code) " +
				"BEGIN IF (SELECT DATEDIFF(mi,Auth.Created_At,@DT) FROM Auth WHERE Token = @Token)<30 " +
				"BEGIN SET @ReturnCode = 200; DELETE FROM Auth WHERE Token = @Token; END " +
				"ELSE BEGIN DELETE FROM Auth WHERE Token = @Token; SET @ReturnCode = 205; END END " +
				"ELSE BEGIN SET @ReturnCode = 204; END SELECT @ReturnCode; END ";
			string connectionString = Configuration.GetConnectionString("PureDatabase");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.Add("@Token", System.Data.SqlDbType.NChar);
				command.Parameters["@Token"].Value = token;
				command.Parameters.Add("@Code", System.Data.SqlDbType.NChar);
				command.Parameters["@Code"].Value = code;
				command.Parameters.Add("@DT", System.Data.SqlDbType.DateTime2);
				command.Parameters["@DT"].Value = DateTime.UtcNow;
				connection.Open();
				var reader = command.ExecuteReader();
				reader.Read();
				return new StatusCodeResult(int.Parse(reader.GetValue(0).ToString()));
			}
		}

		[HttpGet("registration")]
		public ActionResult CheckRegistration(string email, string phone, string username)
		{
			string query = "BEGIN DECLARE @ReturnCode sysname; " +
				"IF Exists(SELECT* FROM Users WHERE Mobile = @Mobile OR Email = @Email or Username = @Username) " +
				"BEGIN SET @ReturnCode = 206; END " +
				"ELSE BEGIN SET @ReturnCode = 200; END " +
				"SELECT @ReturnCode; END";
			string connectionString = Configuration.GetConnectionString("PureDatabase");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar);
				command.Parameters["@Email"].Value = email;
				command.Parameters.Add("@Mobile", System.Data.SqlDbType.NVarChar);
				command.Parameters["@Mobile"].Value = phone;
				command.Parameters.Add("@Username", System.Data.SqlDbType.NVarChar);
				command.Parameters["@Username"].Value = username;
				connection.Open();
				var reader = command.ExecuteReader();
				reader.Read();
				return new StatusCodeResult(int.Parse(reader.GetValue(0).ToString()));
			}
		}


		[HttpGet("resendCode")]
		public ActionResult ResendCode(string adress, string token)
		{
			string query = "BEGIN DECLARE @ReturnCode sysname; " +
				"IF Exists(SELECT * FROM Auth WHERE Token = @Token) " +
				"BEGIN IF (SELECT DATEDIFF(mi,Auth.Created_At,@DT) FROM Auth WHERE Token = @Token)>30 " +
				"BEGIN SET @ReturnCode = 206; DELETE FROM Auth WHERE Token = @Token; END " +
				"ELSE BEGIN SET @ReturnCode = 200; END END " +
				"ELSE BEGIN SET @ReturnCode = 204; END SELECT @ReturnCode; END ";
			string connectionString = Configuration.GetConnectionString("PureDatabase");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.Add("@Token", System.Data.SqlDbType.NChar);
				command.Parameters["@Token"].Value = token;
				command.Parameters.Add("@DT", System.Data.SqlDbType.DateTime2);
				command.Parameters["@DT"].Value = DateTime.UtcNow;
				connection.Open();
				var reader = command.ExecuteReader();
				reader.Read();
				
				int result =int.Parse(reader.GetValue(0).ToString());



				StringBuilder mailText = new();
				mailText.Append("To confirm enter this code: ");

				

				string code="";
				if (result == 206 || result==204)
				{
					Random rnd = new();
					code = new(Enumerable.Repeat(chars, 10).Select(s => s[rnd.Next(s.Length)]).ToArray());
					query = "INSERT INTO Auth(Token, Code) VALUES( @Token, @Code);";

				}
				else if (result == 200)
				{
					query = "SELECT Code FROM Auth WHERE Token=@Token";

				}
				connection.Close();
				reader.Close();

				command = new SqlCommand(query, connection);
				command.Parameters.Add("@Token", System.Data.SqlDbType.NChar);
				command.Parameters["@Token"].Value = token;
				command.Parameters.Add("@Code", System.Data.SqlDbType.NChar);
				command.Parameters["@Code"].Value = code;
				connection.Open();
				if (result == 200)
				{
					reader = command.ExecuteReader();
					reader.Read();
					code = reader.GetValue(0).ToString();
				}
					
				else if (result == 204 || result == 206)
				{
					command.ExecuteNonQuery();
				}
					
				mailText.Append("<b>" + code + "</b>");
				connection.Close();
				return SendEmail(adress, mailText.ToString());
			}
		}

		[HttpPut("resetpassword")]
		public ActionResult ResetPassword(string login, string new_pass)
		{
			string query = "BEGIN DECLARE @ReturnCode sysname; " +
							"IF Exists(SELECT * FROM Users WHERE (Mobile = @Login OR Email = @Login)) " +
							"BEGIN UPDATE Users SET Password_Hash = @Password FROM Users WHERE Mobile = @Login OR Email = @Login; SET @ReturnCode=200; SELECT @ReturnCode; END " +
							"ELSE BEGIN SET @ReturnCode = 204;SELECT @ReturnCode;END END ";

			string connectionString = Configuration.GetConnectionString("PureDatabase");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.Add("@Login", System.Data.SqlDbType.NChar);
				command.Parameters["@Login"].Value = login;
				command.Parameters.Add("@Password", System.Data.SqlDbType.NVarChar);
				command.Parameters["@Password"].Value = BCrypt.Net.BCrypt.HashPassword(new_pass, BCrypt.Net.BCrypt.GenerateSalt(12));
				connection.Open();
				var reader = command.ExecuteReader();
				reader.Read();
				return new StatusCodeResult(int.Parse(reader.GetValue(0).ToString()));
			}
		}


		[HttpGet("login")]
		public ActionResult CheckCredetinals(string login, string password)
		{
			string query = "BEGIN DECLARE @ReturnCode sysname; " +
				"IF Exists(SELECT * FROM Users WHERE (Mobile = @Login OR Email = @Login)) " +
				"BEGIN SELECT Password_Hash FROM Users WHERE Mobile = @Login OR Email = @Login; END " +
				"ELSE BEGIN SET @ReturnCode = 'false';SELECT @ReturnCode;END END ";
			string connectionString = Configuration.GetConnectionString("PureDatabase");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.Add("@Login", System.Data.SqlDbType.NChar);
				command.Parameters["@Login"].Value = login;
				connection.Open();
				var reader = command.ExecuteReader();
				reader.Read();

				string result = reader.GetValue(0).ToString();
				if (result == "false")
				{
					return new StatusCodeResult(204);
				}
				else
				{
					if (BCrypt.Net.BCrypt.Verify(password, result))
					{
						return new StatusCodeResult(200);
					}
					else
						return new StatusCodeResult(206);
				}
			}
		}

			private ActionResult SendEmail(string adress, string body)
			{
				MimeMessage email = new();
				email.From.Add(new MailboxAddress("PureConnect App", "pure.connect.app@gmail.com"));
				email.To.Add(MailboxAddress.Parse(adress));
				email.Subject = "Confirm your code";
				email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

				using (client = new())
				{
					client.Connect("smtp.gmail.com", 465, true);
					client.Authenticate("pure.connect.app@gmail.com", "rtcswcrsxqkldijt");
					client.Send(email);
					client.Disconnect(true);

					return new StatusCodeResult(200);
				}
			}
		}
	}
