using ServiceLayer.Services.Interface.Auth;
using ServiceLayer.Services.Interface.Db;
using ServiceLayer.Services.Interface.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IServiceWrapper
    {
        public IAccountService Accounts { get; }
        public IAuthService Auth { get; }
        public IAutoMailService Mails { get; }
        public IGroupService Groups { get; }
        public IGroupMemberService GroupMembers { get; }
        public IMeetingService Meetings { get; }
        public IStatService Stats { get; }
        public ISubjectService Subjects { get; }
        public IDocumentFileService Documents { get; }
        public IReportService Reports { get; }
        public IDiscussionService Discussions { get; }
        public IAnswerDiscussionService AnswersDiscussions { get;}
    }
}
