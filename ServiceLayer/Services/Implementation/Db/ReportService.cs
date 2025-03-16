using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DbObject;
using DataLayer.Enums;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Db;
using ServiceLayer.Utils;

namespace ServiceLayer.Services.Implementation.Db
{
    public class ReportService : IReportService
    {
        private IRepoWrapper repos;
        private IMapper mapper;

        public ReportService(IRepoWrapper repos, IMapper mapper)
        {
            this.repos = repos;
            this.mapper = mapper;
        }

        public IQueryable<T> GetReportList<T>()
        {
            var list = repos.Reports.GetList()
                 .Include(r => r.Sender)
                 .Include(r => r.Account)
                 .Include(r => r.Group)
                 .Include(r => r.File)
                 .Include(r => r.Discussion);
            //if (list == null || !list.Any())
            //{
            //    return null;
            //}
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }
        public IQueryable<T> GetUnresolvedReportList<T>()
        {
            var list = repos.Reports.GetList()
                 .Include(r => r.Sender)
                 .Include(r => r.Account)
                 .Include(r => r.Group)
                 .Include(r => r.File)
                 .Include(r => r.Discussion)
                 .Where(r=>r.State==RequestStateEnum.Waiting);
            //if (list == null || !list.Any())
            //{
            //    return null;
            //}
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }

        public IQueryable<T> GetApprovedReportList<T>()
        {
            var list = repos.Reports.GetList()
                 .Include(r => r.Sender)
                 .Include(r => r.Account)
                 .Include(r => r.Group)
                 .Include(r => r.File)
                 .Include(r => r.Discussion)
                 .Where(r => r.State == RequestStateEnum.Approved);
            //if (list == null || !list.Any())
            //{
            //    return null;
            //}
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }

        public IQueryable<T> GetReportedAccountList<T>()
        {
            var list = repos.Accounts.GetList()
                .Include(a => a.ReportedReports)
                .Where(a => a.ReportedReports.Any(r => r.State == RequestStateEnum.Waiting));
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }

        public IQueryable<T> GetReportedGroupList<T>()
        {
            var list = repos.Groups.GetList()
                .Include(a => a.ReportedReports)
                .Where(a => a.ReportedReports.Any(r => r.State == RequestStateEnum.Waiting));
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }

        public IQueryable<T> GetReportedDiscussionList<T>()
        {
            var list = repos.Discussions.GetList()
                .Include(a => a.ReportedReports)
                .Where(a => a.ReportedReports.Any(r => r.State == RequestStateEnum.Waiting));
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }

        public IQueryable<T> GetReportedFileList<T>()
        {
            var list = repos.DocumentFiles.GetList()
                .Include(a => a.ReportedReports)
                .Where(a => a.ReportedReports.Any(r => r.State == RequestStateEnum.Waiting));
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }

        public async Task<Report> CreateReport(ReportCreateDto dto, int senderId)
        {
            var report = mapper.Map<Report>(dto);
            report.SenderId = senderId;
            report.State = RequestStateEnum.Waiting;

            await repos.Reports.CreateAsync(report);
            return report;
        }

        public async Task<bool> IsReportExist(int reportId)
        {
            return await repos.Requests.IdExistAsync(reportId);
        }

        public async Task ResolveReport(int reportId, bool isApproved)
        {
            var report = await repos.Reports.GetByIdAsync(reportId);
            if(isApproved == true)
            {
                report.State = RequestStateEnum.Approved;

                //Account
                if (report.AccountId is not null)
                {
                    var account = await repos.Accounts.GetProfileByIdAsync(report.AccountId.Value);
      
                    account.ReportCounter = ++account.ReportCounter;

                    if (account.ReportCounter > 5)
                    {
                        account.IsBanned = true;

                        if (account.GroupMembers.Any())
                        {
                            foreach (var groupMember in account.GroupMembers)
                            {
                                groupMember.IsActive = false;

                                var group = groupMember.Group;
                                if(groupMember.MemberRole == GroupMemberRole.Leader)
                                {
                                    group.IsBanned = true;
                                    
                                    foreach(var mem in group.GroupMembers)
                                    {
                                        mem.IsActive = false;
                                    }
                                }

                                //var document = group.DocumentFiles.Where(x => x.AccountId == account.Id);
                                var documents = await repos.DocumentFiles.GetDocumentFilesByGroupIdAndAccountId(group.Id, account.Id);
                                if (documents.Any())
                                {
                                    foreach (var file in documents)
                                    {
                                        file.IsActive = false;
                                    }
                                }
                                //var discussions = group.Discussions.Where(x => x.AccountId == account.Id);
                                var discussions = await repos.Discussions.GetDiscussionsByGroupIdAndAccountId(group.Id, account.Id);
                                if (discussions.Any())
                                {
                                    foreach (var discussion in discussions)
                                    {
                                        discussion.IsActive = false;

                                        var answerDiscussions = discussion.AnswerDiscussion;
                                        foreach (var answer in answerDiscussions)
                                        {
                                            answer.IsActive = false;
                                        }
                                    }
                                }
                                await repos.Discussions.UpdateRangeAsync(discussions);
                                await repos.DocumentFiles.UpdateRangeAsync(documents);
                            }
                        }
                    }

                    account.PatchUpdate(account);
                    await repos.Accounts.UpdateAsync(account);
                }
                //Discussion
                else if(report.DiscussionId is not null)
                {
                    var discussion = await repos.Discussions.GetByIdAsync(report.DiscussionId.Value);

                    discussion.IsActive = false;
                    discussion.Account.ReportCounter = ++discussion.Account.ReportCounter;

                    if (discussion.Account.ReportCounter > 5)
                    {
                        discussion.Account.IsBanned = true;
                        if (discussion.Account.GroupMembers.Any())
                        {
                            foreach (var groupMember in discussion.Account.GroupMembers)
                            {
                                groupMember.IsActive = false;

                                var group = groupMember.Group;
                                if (groupMember.MemberRole == GroupMemberRole.Leader)
                                {
                                    if (groupMember.MemberRole == GroupMemberRole.Leader)
                                    {
                                        group.IsBanned = true;

                                        foreach (var mem in group.GroupMembers)
                                        {
                                            mem.IsActive = false;
                                        }
                                    }
                                }
                                var documents = await repos.DocumentFiles.GetDocumentFilesByGroupIdAndAccountId(discussion.GroupId, discussion.AccountId);
                                if (documents.Any())
                                {
                                    foreach (var file in documents)
                                    {
                                        file.IsActive = false;
                                    }
                                }
                                var discussions = await repos.Discussions.GetDiscussionsByGroupIdAndAccountId(discussion.GroupId, discussion.AccountId);
                                if (discussions.Any())
                                {
                                    foreach (var discuss in discussions)
                                    {
                                        discuss.IsActive = false;

                                        var answerDiscussions = discuss.AnswerDiscussion;
                                        foreach (var answer in answerDiscussions)
                                        {
                                            answer.IsActive = false;
                                        }
                                    }
                                }
                                await repos.Discussions.UpdateRangeAsync(discussions);
                                await repos.DocumentFiles.UpdateRangeAsync(documents);
                            }
                        }
                    }
                    await repos.Discussions.UpdateAsync(discussion);
                    //await repos.Accounts.UpdateAsync(account);
                }
                //DocumentFile
                else if (report.FileId is not null)
                {
                    var docfile = await repos.DocumentFiles.GetByIdAsync(report.FileId.Value);
                   
                    docfile.IsActive = false;
                    docfile.Account.ReportCounter = ++docfile.Account.ReportCounter;

                    if (docfile.Account.ReportCounter > 5)
                    {
                        docfile.Account.IsBanned = true;

                        if (docfile.Account.GroupMembers.Any())
                        {
                            foreach (var groupMember in docfile.Account.GroupMembers)
                            {
                                groupMember.IsActive = false;

                                var group = groupMember.Group;
                                if (groupMember.MemberRole == GroupMemberRole.Leader)
                                {
                                    group.IsBanned = true;

                                    foreach (var mem in group.GroupMembers)
                                    {
                                        mem.IsActive = false;
                                    }
                                }
                                var documents = await repos.DocumentFiles.GetDocumentFilesByGroupIdAndAccountId(docfile.GroupId, docfile.AccountId);
                                if (documents.Any())
                                {
                                    foreach(var file in documents)
                                    {
                                        file.IsActive = false;
                                    }
                                }
                                var discussions = await repos.Discussions.GetDiscussionsByGroupIdAndAccountId(docfile.GroupId, docfile.AccountId);
                                if (discussions.Any())
                                {
                                    foreach (var discussion in discussions)
                                    {
                                        discussion.IsActive = false;

                                        var answerDiscussions = discussion.AnswerDiscussion;
                                        foreach(var answer in answerDiscussions)
                                        {
                                            answer.IsActive = false;
                                        }
                                    }
                                }
                                await repos.Discussions.UpdateRangeAsync(discussions);
                                await repos.DocumentFiles.UpdateRangeAsync(documents);
                            }
                        }
                    }

                    await repos.DocumentFiles.UpdateAsync(docfile);
                }
                //Group
                else if(report.GroupId is not null)
                {
                    var group = await repos.Groups.GetByIdAsync(report.GroupId.Value);

                    group.BanCounter = ++group.BanCounter;

                    if (group.BanCounter > 3)
                    {
                        group.IsBanned = true;

                        if (group.DocumentFiles.Any()) 
                        {
                            foreach (var doc in group.DocumentFiles)
                            {
                                doc.IsActive = false;
                            }
                        }
                        if (group.Discussions.Any())
                        {
                            foreach (var doc in group.Discussions)
                            {
                                if (doc.AnswerDiscussion.Any())
                                {
                                    foreach(var answer in doc.AnswerDiscussion)
                                    {
                                        answer.IsActive = false;
                                    }
                                }

                                doc.IsActive = false;
                            }
                        }
                        if (group.GroupMembers.Any())
                        {
                            foreach (var doc in group.GroupMembers)
                            {
                                doc.IsActive = false;
                            }
                        }
                        if (group.Schedules.Any())
                        {
                            foreach (var doc in group.Schedules)
                            {
                                doc.IsActive = false;
                            }
                        }
                        if (group.JoinInvites.Any())
                        {
                            foreach (var doc in group.JoinInvites)
                            {
                                doc.State = RequestStateEnum.Decline;
                            }
                        }
                        if (group.JoinRequests.Any())
                        {
                            foreach (var doc in group.JoinRequests)
                            {
                                doc.State = RequestStateEnum.Decline;
                            }
                        }
                    }
                    group.PatchUpdate(group);
                    await repos.Groups.UpdateAsync(group);
                }

            }
            else
            {
                report.State = RequestStateEnum.Decline;
            }
            await repos.Reports.UpdateAsync(report);
        }

        public IQueryable<T> GetBannedAccounts<T>()
        {
            var list = repos.Accounts.GetList()
                    .Include(a => a.ReportedReports)
                    .Where(a => a.IsBanned == true);
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;

        }
        public IQueryable<T> SearchBannedAccounts<T>(string? search)
        {
            if (search != null)
            {
                var list = repos.Accounts.GetList()
                        .Include(a => a.ReportedReports)
                        .Where(a => a.IsBanned == true && (a.FullName.Contains(search) || a.Username.Contains(search) || a.Email.Contains(search)));
                var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
                return mapped;

            }
            else
            {
                var list = repos.Accounts.GetList()
                        .Include(a => a.ReportedReports)
                        .Where(a => a.IsBanned == true);
                var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
                return mapped;
            }

        }
        public IQueryable<T> GetBannedGroups<T>()
        {
            var list = repos.Groups.GetList()
                    .Include(x => x.ReportedReports)
                    .Where(x => x.IsBanned == true);
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }

        public async Task UnbanAccount(int accountId)
        {
            var account = await repos.Accounts.GetProfileByIdAsync(accountId);

            account.IsBanned = false;
            account.ReportCounter = 0;

            if (account.GroupMembers.Any())
            {
                foreach (var groupMember in account.GroupMembers)
                {
                    groupMember.IsActive = true;
                    var group = groupMember.Group;
                    var bannedGroups = ReportedGroups(group.Id);

                    if (bannedGroups == false)
                    {
                        if (groupMember.MemberRole == GroupMemberRole.Leader)
                        {

                            group.IsBanned = false;

                            foreach (var mem in group.GroupMembers)
                            {
                                mem.IsActive = true;
                            }
                        }

                        var documents = await repos.DocumentFiles.GetDocumentFilesByGroupIdAndAccountId(group.Id, accountId);

                        var bannedDocuments = ReportedDocuments(accountId);

                        var notBannedDocuments = documents.Except(bannedDocuments).ToList();

                        if (notBannedDocuments.Any())
                        {
                            foreach (var file in notBannedDocuments)
                            {
                                file.IsActive = true;
                            }
                        }

                        var discussions = await repos.Discussions.GetDiscussionsByGroupIdAndAccountId(group.Id, accountId);

                        var bannedDiscussions = ReportedDiscussions(accountId);

                        var notBannedDiscussions = discussions.Except(bannedDiscussions).ToList();

                        if (notBannedDiscussions.Any())
                        {
                            foreach (var discussion in notBannedDiscussions)
                            {
                                discussion.IsActive = true;

                                var answerDiscussions = discussion.AnswerDiscussion;
                                foreach (var answer in answerDiscussions)
                                {
                                    answer.IsActive = true;
                                }
                            }
                        }
                        await repos.Discussions.UpdateRangeAsync(notBannedDiscussions);
                        await repos.DocumentFiles.UpdateRangeAsync(notBannedDocuments);
                    }
                }
            }
             account.PatchUpdate(account);
             await repos.Accounts.UpdateAsync(account);
        }

        internal IEnumerable<Discussion> ReportedDiscussions(int accountId)
        {
            var list = repos.Discussions.GetList()
               .Include(a => a.ReportedReports)
               .Where(a => a.ReportedReports.Where(r => r.State == RequestStateEnum.Approved).Count() >= 1 && a.AccountId == accountId);

            return list;
        }

        internal IEnumerable<DocumentFile> ReportedDocuments(int accountId)
        {
            var list = repos.DocumentFiles.GetList()
               .Include(a => a.ReportedReports)
               .Where(a => a.ReportedReports.Where(x => x.State == RequestStateEnum.Approved).Count() >= 1 && a.AccountId == accountId);

            return list;
        }
        internal bool ReportedGroups(int groupId)
        {
            var group = repos.Groups.GetList()
               .Include(a => a.ReportedReports)
               .Where(a => a.ReportedReports.Where(r => r.State == RequestStateEnum.Approved).Count() > 3 && a.Id == groupId).FirstOrDefault();
            if (group != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
