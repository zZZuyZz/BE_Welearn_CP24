using DataLayer.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RepoLayer.Interface
{
    public interface IRepoWrapper
    {
        public IAccountRepo Accounts { get; }
        public IMeetingRepository Meetings { get; }
        public IGroupRepository Groups { get; }
        public IGroupMemberReposity GroupMembers { get; }
        public IInviteReposity Invites { get; }
        public IRequestReposity Requests { get; }
        public ISubjectRepository Subjects { get; }
        public IScheduleRepository Schedules { get; }
        public IConnectionRepository Connections { get; }
        public IChatRepository Chats { get; }
        public IReviewRepository Reviews { get; }
        public IReviewDetailRepository ReviewDetails { get; }
        public IDocumentFileReposity DocumentFiles { get; }
        public IReportRepository Reports { get; }
        public IDiscussionRepository Discussions { get; }
        public IAnswerDiscussionRepository AnswerDiscussions { get; }
        public IDbRepository Db { get; }
    }
}
