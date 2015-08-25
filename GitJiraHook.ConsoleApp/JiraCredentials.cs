using System.Configuration;

namespace GitJiraHook.ConsoleApp
{
    public class JiraCredentials
    {
        public string Url { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        private JiraCredentials(string url, string userName, string password)
        {
            this.Url = url;
            this.UserName = userName;
            this.Password = password;
        }

        internal static JiraCredentials GetCredentials()
        {
            return new JiraCredentials(
                ConfigurationManager.AppSettings["Url"],
                ConfigurationManager.AppSettings["UserName"],
                ConfigurationManager.AppSettings["Password"]
                );
        }
    }
}
