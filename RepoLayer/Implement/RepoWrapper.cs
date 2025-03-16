using DataLayer.DbContext;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Implement;
using RepoLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implemention
{
    public class RepoWrapper : IRepoWrapper
    {
        private readonly WeLearnContext dbContext;

        public RepoWrapper(WeLearnContext dbContext)
        {
            this.dbContext = dbContext;
            users = new AccountRepo(dbContext);
            meeting = new MeetingRepository(dbContext);
            groups = new GroupRepository(dbContext);
            subjects = new SubjectRepository(dbContext);
            groupMembers = new GroupMemberReposity(dbContext);
            schedules = new ScheduleRepository(dbContext);
            invites = new InviteReposity(dbContext);
            requests = new RequestReposity(dbContext);
            connections = new ConnectionRepository(dbContext);
            chats = new ChatRepository(dbContext);
            reviews = new ReviewRepository(dbContext);
            reviewDetails = new ReviewDetailRepository(dbContext);
            documentFiles = new DocumentFileRepository(dbContext);
            reports = new ReportRepository(dbContext);
            discussions = new DiscussionRepository(dbContext);
            answerDiscussions = new AnswerDiscussionRepository(dbContext);
            db = new DbRepository(dbContext);
        }

        private IAccountRepo users;
        public IAccountRepo Accounts
        {
            get
            {
                if (users is null)
                {
                    users = new AccountRepo(dbContext);
                }
                return users;
            }
        }

        private IMeetingRepository meeting;
        public IMeetingRepository Meetings
        {
            get
            {
                if (meeting is null)
                {
                    meeting = new MeetingRepository(dbContext);
                }
                return meeting;
            }
        }

        private IGroupRepository groups;
        public IGroupRepository Groups
        {
            get
            {
                if (groups is null)
                {
                    groups = new GroupRepository(dbContext);
                }
                return groups;
            }
        }

        private IGroupMemberReposity groupMembers;
        public IGroupMemberReposity GroupMembers
        {
            get
            {
                if (groupMembers is null)
                {
                    groupMembers = new GroupMemberReposity(dbContext);
                }
                return groupMembers;
            }
        }

        private ISubjectRepository subjects;
        public ISubjectRepository Subjects
        {
            get
            {
                if (subjects is null)
                {
                    subjects = new SubjectRepository(dbContext);
                }
                return subjects;
            }
        }

        public IScheduleRepository schedules;
        public IScheduleRepository Schedules
        {
            get
            {
                if (schedules is null)
                {
                    schedules = new ScheduleRepository(dbContext);
                }
                return schedules;
            }

        }

        private IInviteReposity invites;
        public IInviteReposity Invites
        {
            get
            {
                if (invites is null)
                {
                    invites = new InviteReposity(dbContext);
                }
                return invites;
            }

        }

        private IRequestReposity requests;
        public IRequestReposity Requests
        {
            get
            {
                if (requests is null)
                {
                    requests = new RequestReposity(dbContext);
                }
                return requests;
            }

        }

        private IConnectionRepository connections;
        public IConnectionRepository Connections
        {
            get
            {
                if (connections is null)
                {
                    connections = new ConnectionRepository(dbContext);
                }
                return connections;
            }

        }

        private IChatRepository chats;
        public IChatRepository Chats
        {
            get
            {
                if (chats is null)
                {
                    chats = new ChatRepository(dbContext);
                }
                return chats;
            }

        }

        private IReviewRepository reviews;
        public IReviewRepository Reviews
        {
            get
            {
                if (reviews is null)
                {
                    reviews = new ReviewRepository(dbContext);
                }
                return reviews;
            }

        }
        private IReviewDetailRepository reviewDetails;
        public IReviewDetailRepository ReviewDetails
        {
            get
            {
                if (reviewDetails is null)
                {
                    reviewDetails = new ReviewDetailRepository(dbContext);
                }
                return reviewDetails;
            }

        }
        
        private IDocumentFileReposity documentFiles;
        public IDocumentFileReposity DocumentFiles
        {
            get
            {
                if (documentFiles is null)
                {
                    documentFiles = new DocumentFileRepository(dbContext);
                }
                return documentFiles;
            }
        }

        private IReportRepository reports;
        public IReportRepository Reports
        {
            get
            {
                if (reports is null)
                {
                    reports = new ReportRepository(dbContext);
                }
                return reports;
            }

        }
        private IDiscussionRepository discussions;
        public IDiscussionRepository Discussions
        {
            get
            {
                if (discussions is null)
                {
                    discussions = new DiscussionRepository(dbContext);
                }
                return discussions;

            }
        }
        private IAnswerDiscussionRepository answerDiscussions;
        public IAnswerDiscussionRepository AnswerDiscussions
        {
            get
            {
                if (answerDiscussions is null)
                {
                    answerDiscussions = new AnswerDiscussionRepository(dbContext);
                }
                return answerDiscussions;

            }
        }

        private IDbRepository db;
        public IDbRepository Db
        {
            get
            {
                if (db is null)
                {
                    db = new DbRepository(dbContext);
                }
                return db;

            }
        }
    }
}
