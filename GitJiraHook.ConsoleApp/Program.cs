using System;
using System.Text;
using GitJiraHook.ConsoleApp.DTO;

namespace GitJiraHook.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string branch, author, email, commitDate, title, files, body;

            if (args.Length == 7)
            {
                branch = args[0];
                author = args[1];
                email = args[2];
                commitDate = args[3];
                title = args[4];
                files = args[5];
                body = args[6];
            }
            else
            {
                branch = "Test-Branch";
                author = "Author";
                email = "email@email.com";
                commitDate = DateTime.Now.ToLongDateString();
                title = "[JIRA-TICKET] this is a test";
                files = "file1.txt, file2.txt";
                body = "Body";
            }

            var jira = new JiraAntiCorruption(JiraCredentials.GetCredentials());

            var changeSet = GitChangeSet.Generate(branch, author, email, commitDate, title, files, body);

            try
            {
                jira.AddComment(changeSet);
            }
            catch (Exception e)
            {
                LogAction(branch, author, email, commitDate, title, files, body, e);
            }
        }

        private static void LogAction(string branch, string author, string email, string commitDate, string title, string files, string body, Exception e)
        {
            StringBuilder error = new StringBuilder();
            if (e != null)
                error.Append(e.Message);

            error.AppendLine(string.Format("Branch: {0}", branch));
            error.AppendLine(string.Format("Author: {0}", author));
            error.AppendLine(string.Format("Email: {0}", email));
            error.AppendLine(string.Format("CommitDate: {0}", commitDate));
            error.AppendLine(string.Format("Title: {0}", title));
            error.AppendLine(string.Format("Body: {0}", body));
            error.AppendLine(string.Format("Files: {0}", files));

            Console.Write(error.ToString());

            using (var writer = new System.IO.StreamWriter(string.Format("{0}{1}log.txt", AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.Ticks)))
            {
                writer.Write(error.ToString());
            }
        }
    }
}
