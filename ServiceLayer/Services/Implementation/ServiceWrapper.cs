using RepoLayer.Interface;
using ServiceLayer.Services.Implementation.Auth;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using ServiceLayer.Services.Implementation.Db;
using ServiceLayer.Services.Implementation.Mail;
using ServiceLayer.Services.Interface;
using ServiceLayer.Services.Interface.Auth;
using ServiceLayer.Services.Interface.Db;
using ServiceLayer.Services.Interface.Mail;

namespace ServiceLayer.Services.Implementation
{
    public class ServiceWrapper : IServiceWrapper
    {
        private readonly IRepoWrapper repos;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment env;


        public ServiceWrapper(IRepoWrapper repos, IConfiguration configuration, IMapper mapper, IWebHostEnvironment env, IConfiguration config)
        {
            this.repos = repos;
            this.configuration = configuration;
            this.mapper = mapper;
            this.env = env;
            accounts = new AccountService(repos, mapper, env, config);
            groups = new GroupService(repos, mapper, env, config);
            subjects = new SubjectService(repos, mapper);
            meetings = new MeetingService(repos, mapper, config);
            groupMembers = new GroupMemberSerivce(repos, mapper);
            documentFiles = new DocumentFileService(repos, mapper, env, config);
            stats = new StatService(repos, mapper);
            mails = new AutoMailService(env, repos, configuration, Accounts, Stats);
            auth = new AuthService(repos, configuration, mapper, mails);
        }

        private IAccountService accounts;
        public IAccountService Accounts
        {
            get
            {
                if (accounts is null)
                {
                    accounts = new AccountService(repos, mapper, env, configuration);
                }
                return accounts;
            }
        }

        private IGroupService groups;
        public IGroupService Groups
        {
            get
            {
                if (groups is null)
                {
                    groups = new GroupService(repos, mapper, env, configuration);
                }
                return groups;
            }
        }

        private ISubjectService subjects;
        public ISubjectService Subjects
        {
            get
            {
                if (subjects is null)
                {
                    subjects = new SubjectService(repos, mapper);
                }
                return subjects;
            }
        }

        private IMeetingService meetings;
        public IMeetingService Meetings
        {
            get
            {
                if (meetings is null)
                {
                    meetings = new MeetingService(repos, mapper, configuration);
                }
                return meetings;
            }
        }

        private IGroupMemberService groupMembers;
        public IGroupMemberService GroupMembers
        {
            get
            {
                if (groupMembers is null)
                {
                    groupMembers = new GroupMemberSerivce(repos, mapper);
                }
                return groupMembers;
            }
        }

        private IStatService stats;
        public IStatService Stats
        {
            get
            {
                if (stats is null)
                {
                    stats = new StatService(repos, mapper);
                }
                return stats;
            }
        }
        private IAutoMailService mails;
        public IAutoMailService Mails
        {
            get
            {
                if (mails is null)
                {
                    mails = new AutoMailService(env, repos, configuration, Accounts, Stats);
                }
                return mails;
            }
        }

        private IAuthService auth;
        public IAuthService Auth
        {
            get
            {
                if (auth is null)
                {
                    auth = new AuthService(repos, configuration, mapper, mails);
                }
                return auth;
            }
        }
        private IDocumentFileService documentFiles;
        public IDocumentFileService Documents
        {
            get
            {
                if (documentFiles is null)
                {
                    documentFiles = new DocumentFileService(repos, mapper, env, configuration);
                }
                return documentFiles;
            }
        }

        private IReportService reports;
        public IReportService Reports
        {
            get
            {
                if (reports is null)
                {
                    reports = new ReportService(repos, mapper);
                }
                return reports;
            }
        }
        private IDiscussionService discussions;
        public IDiscussionService Discussions
        {
            get
            {
                if (discussions is null)
                {
                    discussions = new DiscussionService(repos, mapper, env, configuration);
                }
                return discussions;
            }
        }
        private IAnswerDiscussionService answersDiscussions;
        public IAnswerDiscussionService AnswersDiscussions
        {
            get
            {
                if (answersDiscussions is null)
                {
                    answersDiscussions = new AnswerDiscussionService(repos, mapper, env, configuration);
                }
                return answersDiscussions;
            }
        }
    }
}

