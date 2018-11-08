using System.Web.Mvc;

namespace SupFile2.Controllers
{
    using System;
    using System.Collections.Specialized;
    using MySql.Data.MySqlClient;
    using SupFile2.Models;
    using SupFile2.Utilities;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using DotNetOpenAuth.GoogleOAuth2;
    using Facebook;
    using Microsoft.AspNet.Membership.OpenAuth;
    using SupFile2.Entities;

    [RequireHttps]
    public class LoginController : Controller
    {
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
         public ActionResult Index(LoginModel loginModel)
        {
            if (loginModel.Email == null || loginModel.Password == null)
            {
                //TODO : Afficher une notif ou une alerte ou un truc
                return View();
            }
            string sql = "SELECT * FROM user u WHERE u.mail = @email AND u.password = @password;";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@email", loginModel.Email);
            cmd.Parameters.AddWithValue("@password", this.CreatePasswordHash(loginModel.Password));
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                var userid = reader.GetInt32(0);
                reader.Close();

                MySession.SetUser(new User(userid));

                return RedirectToAction("Index", "Home");
            }
            else
            {
                reader.Close();

                return this.View();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Models.LoginModel loginModel)
        {
            if (loginModel.Email == null || loginModel.Password == null)
            {
                //TODO : Afficher une notif ou une alerte ou un truc
                return View();
            }

            if (!this.CheckIfEmailIsCorrect(loginModel.Email))
            {
                //TODO : Afficher une notif ou une alerte ou un truc
                return View();
            }
            var command = Database.Instance.Connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(u.id) FROM user u WHERE u.mail = '{loginModel.Email}';";
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                if (reader.GetInt32(0) != 0)
                {
                    //TODO : cet user existe deja,  Afficher une notif ou une alerte ou un truc

                    return this.View();
                }
            }
            
            string sql = "INSERT INTO `supfile`.`user` (`mail`, `password`, `stockage`, `stockagemax`) VALUES (@mail, @password, 0, 30000000);";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@mail", loginModel.Email);
            var encryptedPassword = this.CreatePasswordHash(loginModel.Password);
            cmd.Parameters.AddWithValue("@password", encryptedPassword);
            cmd.ExecuteNonQuery();

            return RedirectToAction("Index", "LandingPage");

        }

        public ActionResult RedirectToGoogle()
        {
            string provider = "google";
            string returnUrl = "";
            return new ExternalLoginResult(provider, Url.Action("GoogleLoginCallback", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public ActionResult GoogleLoginCallback(string returnUrl)
        {
            string ProviderName = OpenAuth.GetProviderNameFromCurrentRequest();

            if (ProviderName == null || ProviderName == "")
            {
                NameValueCollection nvs = Request.QueryString;
                if (nvs.Count > 0)
                {
                    if (nvs["state"] != null)
                    {
                        NameValueCollection provideritem = HttpUtility.ParseQueryString(nvs["state"]);
                        if (provideritem["__provider__"] != null)
                        {
                            ProviderName = provideritem["__provider__"];
                        }
                    }
                }
            }

            GoogleOAuth2Client.RewriteRequest();

            var redirectUrl = Url.Action("GoogleLoginCallback", new { ReturnUrl = returnUrl });
            var retUrl = returnUrl;
            var authResult = OpenAuth.VerifyAuthentication(redirectUrl);

            if (!authResult.IsSuccessful)
            {
                return Redirect(Url.Action("Index", "Login"));
            }

            var username = authResult.UserName;
            var providerId = authResult.ProviderUserId;

            string sql = "SELECT * FROM user u WHERE u.mail = @mail AND u.password = @password;";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@mail", username);
            cmd.Parameters.AddWithValue("@password", providerId);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                var userId = reader.GetInt32(0);
                reader.Close();
                MySession.SetUser(new User(userId));
            }
            else
            {
                reader.Close();

                string sql2 = $"INSERT INTO `supfile`.`user` (`mail`, `password`, `stockage`, `stockagemax`) VALUES ('{username}', @password, 0, 30000000);";
                MySqlCommand cmd2 = new MySqlCommand(sql2, Database.Instance.Connection);
                cmd2.Parameters.AddWithValue("@password", providerId);
                var a = Database.Instance.SeePreparedQuery(cmd2);
                cmd2.ExecuteNonQuery();

                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    var userId = reader.GetInt32(0);
                    reader.Close();
                    MySession.SetUser(new User(userId));
                }
                else
                {
                    //Something appened, register failed
                }
            }

            return RedirectToAction("Index", "Home");

        }

        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "198611740780795",
                client_secret = "09f57022bd379e20008cc3bd3c1e6e2e",
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = "198611740780795",
                client_secret = "09f57022bd379e20008cc3bd3c1e6e2e",
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;

            fb.AccessToken = accessToken;
            dynamic me = fb.Get("me?fields=email,id");
            string email = me.email;
            string id = me.id;

            string sql = "SELECT * FROM user u WHERE u.mail = @mail AND u.password = @password;";
            MySqlCommand cmd = new MySqlCommand(sql, Database.Instance.Connection);
            cmd.Parameters.AddWithValue("@mail", email);
            cmd.Parameters.AddWithValue("@password", id);
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                var userId = reader.GetInt32(0);
                reader.Close();
                MySession.SetUser(new User(userId));
            }
            else
            {
                reader.Close();

                string sql2 = $"INSERT INTO `supfile`.`user` (`mail`, `password`, `stockage`, `stockagemax`) VALUES ('{email}', @password, 0, 30000000);";
                MySqlCommand cmd2 = new MySqlCommand(sql2, Database.Instance.Connection);
                cmd2.Parameters.AddWithValue("@password", id);
                var a = Database.Instance.SeePreparedQuery(cmd2);
                cmd2.ExecuteNonQuery();

                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    var userId = reader.GetInt32(0);
                    reader.Close();
                    MySession.SetUser(new User(userId));
                }
                else
                {
                    //Something appened, register failed
                }
            }

            return RedirectToAction("Index", "Home");

        }

        private string CreatePasswordHash(string clearPassword)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hashedPassword = md5.ComputeHash(Encoding.UTF8.GetBytes(clearPassword));

                return Convert.ToBase64String(hashedPassword);
            }

        }

        private bool CheckIfEmailIsCorrect(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OpenAuth.RequestAuthentication(Provider, ReturnUrl);
            }
        }
    }
}