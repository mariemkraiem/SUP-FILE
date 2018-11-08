namespace SupFile2
{
    using System.Collections.Generic;
    using DotNetOpenAuth.GoogleOAuth2;
    using Microsoft.AspNet.Membership.OpenAuth;

    public class AuthConfig
    {
        public static void RegisterAuth()
        {
            GoogleOAuth2Client clientGoog = new GoogleOAuth2Client("59712095190-m5pc9c8mr6bfg11mojafq7b4tmqbu1sq.apps.googleusercontent.com", "a05MowpyYo4K3Pkn5DvmhqE-");
            IDictionary<string, string> extraData = new Dictionary<string, string>();
            OpenAuth.AuthenticationClients.Add("google", () => clientGoog, extraData);
        }
    }
}