using GitJiraHook.ConsoleApp.DTO;
using TechTalk.JiraRestClient;

namespace GitJiraHook.ConsoleApp
{
    internal class JiraAntiCorruption
    {
        private JiraClient _client;
        private JiraCredentials _credentials;

        internal JiraClient Client
        {
            get
            {
                if (this._client == null)
                    this._client = new JiraClient(this._credentials.Url, this._credentials.UserName, this._credentials.Password);

                return this._client;
            }
        }

        public JiraAntiCorruption(JiraCredentials credentials)
        {
            this._credentials = credentials;
        }

        internal void AddComment(GitChangeSet changeSet)
        {
            foreach (var ticket in changeSet.JiraTickets)
                this.CreateComment(changeSet, ticket);
        }

        private void CreateComment(GitChangeSet changeSet, string ticket)
        {
            var issueRef = new IssueRef { id = ticket };

            var issue = this.Client.LoadIssue(issueRef);

            if (issue == null)
                return;

            this.Client.CreateComment(issueRef, changeSet.ToString());

        }
    }
}
