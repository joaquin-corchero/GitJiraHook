using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitJiraHook.ConsoleApp.DTO
{
    public class GitChangeSet
    {
        public List<string> JiraTickets { get; private set; }

        public string Author { get; private set; }

        public string AuthorEmail { get; private set; }

        public List<string> FileChanges { get; private set; }

        public string CommitDate { get; private set; }

        public string Branch { get; private set; }

        public string Comment { get; private set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("{panel:title=Git Commit|borderStyle=solid|borderColor=#ccc|titleBGColor=#6699C6|bgColor=#EBF3FC}");
            builder.AppendLine(string.Format("{0} with email {1} commited changes on {2}", this.Author, this.AuthorEmail, this.CommitDate));
            builder.AppendLine();
            builder.AppendLine(string.Format("To Branch {0}", this.Branch));
            builder.AppendLine();
            builder.AppendLine("With comments:");
            builder.AppendLine(string.IsNullOrEmpty(this.Comment) ? "None provided" : this.Comment);
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("Changes:");
            this.FileChanges.ForEach(c => builder.AppendLine(string.Format("- {0}", c)));

            builder.AppendLine();
            builder.AppendLine("Tickets affected by change:");
            this.JiraTickets.ForEach(c => builder.AppendLine(string.Format("- {0}", c)));
            builder.AppendLine("{panel}");

            return builder.ToString();
        }

        private GitChangeSet(List<string> jiraTickets, string author, string authorEmail, List<string> fileChanges, string changeDate, string comment, string branch)
        {
            this.JiraTickets = jiraTickets;
            this.Author = author;
            this.AuthorEmail = authorEmail;
            this.FileChanges = fileChanges;
            this.CommitDate = changeDate;
            this.Comment = comment;
            this.Branch = branch;
        }

        public static GitChangeSet Generate(string branch, string author, string email, string commitDate, string title, string files, string body)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("No title provided for the commit");
                return null;
            }

            List<string> issuesId = null;
            StringBuilder comments = new StringBuilder();

            issuesId = GitChangeSet.GetIssuesAndPopulateCommentsFromTitle(title, comments);

            if (issuesId == null || !issuesId.Any())
            {
                Console.WriteLine(string.Format("No valid title provided for the commit {0}", title));
                return null;
            }

            comments.AppendLine(body);

            return new GitChangeSet(
                issuesId,
                author,
                email,
                files.Split(Environment.NewLine.ToCharArray()).Where(r => !string.IsNullOrWhiteSpace(r)).ToList(),
                commitDate,
                comments.ToString(),
                branch);
        }

        private static List<string> GetIssuesAndPopulateCommentsFromTitle(string title, StringBuilder comments)
        {
            List<string> result;

            if (title.Contains("[") && title.Contains("]"))
                result = GetIssuesAndCommentsFromTitle(title, comments);
            else
                result = GitChangeSet.GetIssuesByCommaSplit(title);

            return result;
        }

        private static List<string> GetIssuesAndCommentsFromTitle(string title, StringBuilder comments)
        {
            string issues = title.Substring(0, title.LastIndexOf("]")).Replace("]", "");
            var result = issues.Split('[')
                .Where(r =>
                    !string.IsNullOrWhiteSpace(r) &&
                    r.Length < 15
                )
                .Select(r => r.Trim().Replace(",", ""))
                .Distinct()
                .ToList();

            string commentOnTitle = title.Substring(title.LastIndexOf("]") + 1);

            if (!string.IsNullOrEmpty(commentOnTitle))
                comments.AppendLine(commentOnTitle.Trim());

            return result;
        }

        private static List<string> GetIssuesByCommaSplit(string title)
        {
            if (!title.Contains("-"))
                return null;

            var result = title.Split(',')
                    .Where(r =>
                        !string.IsNullOrWhiteSpace(r) &&
                        r.Length < 15
                    )
                    .Select(r => r.Trim())
                    .Distinct()
                    .ToList();

            return result;
        }
    }
}
