using DataLayer.DbContext;
using RepoLayer.EfUtil;
using RepoLayer.Interface;

namespace RepoLayer.Implement
{
    public class DbRepository : IDbRepository
    {
        private readonly WeLearnContext dbContext;

        public DbRepository(WeLearnContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Nuke()
        {
            //dbContext.Reports.Truncate();
            ////dbContext.DocumentFiles.Truncate();
            //dbContext.AnswerDiscussions.Truncate();
            ////dbContext.Discussions.Truncate();
            //dbContext.Chats.Truncate();
            //dbContext.ReviewDetails.Truncate();
            ////dbContext.Reviews.Truncate();
            //dbContext.Connections.Truncate();
            //dbContext.Requests.Truncate();
            //dbContext.Invites.Truncate();
            //dbContext.ScheduleSubjects.Truncate();
            ////dbContext.Meetings.Truncate();
            ////dbContext.Schedules.Truncate();
            //dbContext.GroupSubjects.Truncate();
            //dbContext.GroupMembers.Truncate();
            ////dbContext.Groups.Truncate();
            ////dbContext.Subjects.Truncate();
            ////dbContext.Accounts.Truncate();
            ////dbContext.Roles.Truncate();
            ///
            DateTime start = DateTime.Now;
            Console.WriteLine("Start Nuke", start);

            // Create new stopwatch
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            // Begin timing
            stopwatch.Start();

            bool tryAgain = true;
            while(tryAgain)
            {
                try
                {

                    //dbContext.Reports.Clear();
                    //dbContext.DocumentFiles.Clear();
                    //dbContext.AnswerDiscussions.Clear();
                    //dbContext.Discussions.Clear();
                    //dbContext.Chats.Clear();
                    //dbContext.ReviewDetails.Clear();
                    //dbContext.Reviews.Clear();
                    //dbContext.Connections.Clear();
                    //dbContext.Requests.Clear();
                    //dbContext.Invites.Clear();
                    //dbContext.ScheduleSubjects.Clear();
                    //dbContext.Meetings.Clear();
                    //dbContext.Schedules.Clear();
                    //dbContext.GroupSubjects.Clear();
                    //dbContext.GroupMembers.Clear();
                    //dbContext.Groups.Clear();
                    //dbContext.Subjects.Clear();
                    //dbContext.Accounts.Clear();
                    //dbContext.Roles.Clear();

                    //way faster
                    dbContext.Reports.Delete();
                    dbContext.DocumentFiles.Delete();
                    dbContext.AnswerDiscussions.Delete();
                    dbContext.Discussions.Delete();
                    dbContext.Chats.Delete();
                    dbContext.ReviewDetails.Delete();
                    dbContext.Reviews.Delete();
                    dbContext.Connections.Delete();
                    dbContext.Requests.Delete();
                    dbContext.Invites.Delete();
                    dbContext.ScheduleSubjects.Delete();
                    dbContext.Meetings.Delete();
                    dbContext.Schedules.Delete();
                    dbContext.GroupSubjects.Delete();
                    dbContext.GroupMembers.Delete();
                    dbContext.Groups.Delete();
                    dbContext.Subjects.Delete();
                    dbContext.Accounts.Delete();
                    dbContext.Roles.Delete();
                    tryAgain = false;
                }
                catch { }

                // Stop timing
                stopwatch.Stop();

                Console.WriteLine("Time taken : {0}", stopwatch.Elapsed);

                DateTime end = DateTime.Now;
                Console.WriteLine("End Nuke", end);
                Console.WriteLine("Time Nuke", (start - end).TotalMilliseconds);

            }
        }
        public async Task Nuke2()
        {
            ///
            DateTime start = DateTime.Now;
            Console.WriteLine("\nStart Nuke 2", start);

            // Create new stopwatch
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            // Begin timing
            stopwatch.Start();

            bool tryAgain = true;
            while (tryAgain)
            {
                try
                {

                    dbContext.Reports.Clear();
                    dbContext.DocumentFiles.Clear();
                    dbContext.AnswerDiscussions.Clear();
                    dbContext.Discussions.Clear();
                    dbContext.Chats.Clear();
                    dbContext.ReviewDetails.Clear();
                    dbContext.Reviews.Clear();
                    dbContext.Connections.Clear();
                    dbContext.Requests.Clear();
                    dbContext.Invites.Clear();
                    dbContext.ScheduleSubjects.Clear();
                    dbContext.Meetings.Clear();
                    dbContext.Schedules.Clear();
                    dbContext.GroupSubjects.Clear();
                    dbContext.GroupMembers.Clear();
                    dbContext.Groups.Clear();
                    dbContext.Subjects.Clear();
                    dbContext.Accounts.Clear();
                    dbContext.Roles.Clear();
                    dbContext.SaveChanges();
                    tryAgain = false;
                }
                catch { }

                // Stop timing
                stopwatch.Stop();

                Console.WriteLine("Time taken : {0}", stopwatch.Elapsed);

                DateTime end = DateTime.Now;
                Console.WriteLine("End Nuke 2", end);
                Console.WriteLine("Time Nuke 2", (start - end).TotalMilliseconds);

            }
        }
    }
}
