using DataLayer.DbContext;
using DataLayer.DbObject;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DbSeeding
{
    public static class DbInitializerExtension
    {
        public static IApplicationBuilder SeedInMemoryDb(this IApplicationBuilder app, bool isInMemory)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<WeLearnContext>();
                DbInitializer.Initialize(context, isInMemory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return app;
        }
        public static bool SeedInMemoryDb(WeLearnContext context, bool isInMemory=false)
        {
            //try
            //{
                DbInitializer.Initialize(context, isInMemory);
                return true;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    return false;
            //}
        }
        public class DbInitializer
        {
            internal static void Initialize(WeLearnContext context, bool isInMemory)
            {
                ArgumentNullException.ThrowIfNull(context, nameof(context));
                //context.Database.EnsureCreated();
                if (isInMemory)
                {

                    #region seed Role
                    if (!context.Roles.Any())
                    {

                        context.Roles.AddRange(DbSeed.Roles);
                    }
                    #endregion

                    #region seed Account
                    if (!context.Accounts.Any())
                    {

                        context.Accounts.AddRange(DbSeed.Accounts);

                    }
                    #endregion

                    #region seed subject
                    if (!context.Subjects.Any())
                    {
                        context.Subjects.AddRange(DbSeed.Subjects);
                    }
                    #endregion

                    #region seed group
                    if (!context.Groups.Any())
                    {
                        context.Groups.AddRange(DbSeed.Groups);
                    }
                    #endregion

                    #region seed group member
                    if (!context.GroupMembers.Any())
                    {
                        context.GroupMembers.AddRange(DbSeed.GroupMembers);
                    }
                    #endregion

                    #region seed group subject
                    if (!context.GroupSubjects.Any())
                    {
                        context.GroupSubjects.AddRange(DbSeed.GroupSubjects);
                    }
                    #endregion

                    #region seed schedule subject
                    if (!context.ScheduleSubjects.Any())
                    {
                        context.ScheduleSubjects.AddRange(DbSeed.ScheduleSubjects);
                    }
                    #endregion

                    #region seed invite
                    if (!context.Invites.Any())
                    {
                        context.Invites.AddRange(DbSeed.Invites);
                    }
                    #endregion

                    #region seed request
                    if (!context.Requests.Any())
                    {
                        context.Requests.AddRange(DbSeed.Requests);
                    }
                    #endregion

                    #region seed meeting and schedule
                    if (!context.Schedules.Any())
                    {
                        context.Schedules.AddRange(DbSeed.Schedules);
                    }
                    if (!context.Meetings.Any())
                    {
                        context.Meetings.AddRange(DbSeed.Meetings);
                    }
                    #endregion

                    #region seed Connection
                    if (!context.Connections.Any())
                    {
                        context.Connections.AddRange(DbSeed.Connections);
                    }
                    #endregion

                    #region seed Review
                    if (!context.Reviews.Any())
                    {
                        context.Reviews.AddRange(DbSeed.Reviews);
                    }
                    #endregion

                    #region seed ReviewDetail
                    if (!context.ReviewDetails.Any())
                    {
                        context.ReviewDetails.AddRange(DbSeed.ReviewDetails);
                    }
                    #endregion

                    #region seed Chat
                    if (!context.Chats.Any())
                    {
                        context.Chats.AddRange(DbSeed.Chats);
                    }
                    #endregion

                    #region seed Report
                    if (!context.Reports.Any())
                    {
                        context.Reports.AddRange(DbSeed.Reports);
                    }
                    #endregion

                    #region seed Discussion
                    if (!context.Discussions.Any())
                    {
                        context.Discussions.AddRange(DbSeed.Discussions);
                    }
                    #endregion

                    #region seed AnswerDiscussion
                    if (!context.AnswerDiscussions.Any())
                    {
                        context.AnswerDiscussions.AddRange(DbSeed.AnswerDiscussions);
                    }
                    #endregion

                    #region seed DocumentFile
                    if (!context.DocumentFiles.Any())
                    {
                        context.DocumentFiles.AddRange(DbSeed.DocumentFiles);
                    }
                    #endregion
                    context.SaveChanges();

                }
                else
                {
                    //context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    #region seed Role
                    if (!context.Roles.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nRoles");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Roles ON");
                            context.Roles.AddRange(DbSeed.Roles);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Roles OFF");
                            Console.WriteLine("\n\nRoles");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed Account
                    if (!context.Accounts.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nAccounts");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Accounts ON");
                            context.Accounts.AddRange(DbSeed.Accounts);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Accounts OFF");
                            Console.WriteLine("\n\nAccounts");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed subject
                    if (!context.Subjects.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nSubjects");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Subjects ON");
                            context.Subjects.AddRange(DbSeed.Subjects);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Subjects OFF");
                            Console.WriteLine("\n\nSubjects");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed group
                    if (!context.Groups.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nGroups");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Groups ON");
                            context.Groups.AddRange(DbSeed.Groups);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Groups OFF");
                            Console.WriteLine("\n\nGroups");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed group member
                    if (!context.GroupMembers.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nGroupMembers");

                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT GroupMembers ON");
                            context.GroupMembers.AddRange(DbSeed.GroupMembers);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT GroupMembers OFF");
                            Console.WriteLine("\n\nGroupMembers");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed group subject
                    if (!context.GroupSubjects.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nGroupSubjects");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT GroupSubjects ON");
                            context.GroupSubjects.AddRange(DbSeed.GroupSubjects);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT GroupSubjects OFF");
                            Console.WriteLine("\n\nGroupSubjects");
                            transaction.Commit();
                        }
                    }
                    #endregion  

                    #region seed meeting and schedule
                    if (!context.Schedules.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nSchedules");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Schedules ON");
                            context.Schedules.AddRange(DbSeed.Schedules);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Schedules OFF");
                            Console.WriteLine("\n\nSchedules");
                            transaction.Commit();
                        }
                    }
                    if (!context.Meetings.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nMeetings");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Meetings ON");
                            context.Meetings.AddRange(DbSeed.Meetings);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Meetings OFF");
                            Console.WriteLine("\n\nMeetings");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed schedule subject
                    if (!context.ScheduleSubjects.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nScheduleSubjects");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ScheduleSubjects ON");
                            context.ScheduleSubjects.AddRange(DbSeed.ScheduleSubjects);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ScheduleSubjects OFF");
                            Console.WriteLine("\n\nScheduleSubjects");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed invite
                    if (!context.Invites.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nJoinInvites");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT JoinInvites ON");
                            context.Invites.AddRange(DbSeed.Invites);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT JoinInvites OFF");
                            Console.WriteLine("\n\nJoinInvites");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed request
                    if (!context.Requests.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nJoinRequests");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT JoinRequests ON");
                            context.Requests.AddRange(DbSeed.Requests);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT JoinRequests OFF");
                            Console.WriteLine("\n\nJoinRequests");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed Connection
                    if (!context.Connections.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nMeetingParticipations");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT MeetingParticipations ON");
                            context.Connections.AddRange(DbSeed.Connections);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT MeetingParticipations OFF");
                            Console.WriteLine("\n\nMeetingParticipations");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed Review
                    if (!context.Reviews.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nReviews");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Reviews ON");
                            context.Reviews.AddRange(DbSeed.Reviews);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Reviews OFF");
                            Console.WriteLine("\n\nReviews");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed ReviewDetail
                    if (!context.ReviewDetails.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nReviewDetails");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ReviewDetails ON");
                            context.ReviewDetails.AddRange(DbSeed.ReviewDetails);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT ReviewDetails OFF");
                            Console.WriteLine("\n\nReviewDetails");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed Chat
                    if (!context.Chats.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nChats");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Chats ON");
                            context.Chats.AddRange(DbSeed.Chats);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Chats OFF");
                            Console.WriteLine("\n\nChats");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed Discussion
                    if (!context.Discussions.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nDiscussions");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Discussions ON");
                            context.Discussions.AddRange(DbSeed.Discussions);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Discussions OFF");
                            Console.WriteLine("\n\nDiscussions");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed AnswerDiscussion
                    if (!context.AnswerDiscussions.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nAnswerDiscussions");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AnswerDiscussions ON");
                            context.AnswerDiscussions.AddRange(DbSeed.AnswerDiscussions);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AnswerDiscussions OFF");
                            Console.WriteLine("\n\nAnswerDiscussions");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed DocumentFile
                    if (!context.DocumentFiles.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nDocumentFiles");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DocumentFiles ON");
                            context.DocumentFiles.AddRange(DbSeed.DocumentFiles);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT DocumentFiles OFF");
                            Console.WriteLine("\n\nDocumentFiles");
                            transaction.Commit();
                        }
                    }
                    #endregion

                    #region seed Report
                    if (!context.Reports.Any())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            Console.WriteLine("\n\nReports");
                            context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Reports ON");
                            context.Reports.AddRange(DbSeed.Reports);
                            context.SaveChanges();
                            //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Reports OFF");
                            Console.WriteLine("\n\nReports");
                            transaction.Commit();
                        }
                    }
                    #endregion
                }
            }
        }
    }
}
