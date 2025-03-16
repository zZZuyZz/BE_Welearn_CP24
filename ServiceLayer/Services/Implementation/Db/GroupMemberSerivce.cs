using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DbObject;
using DataLayer.Enums;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Db;

namespace ServiceLayer.Services.Implementation.Db
{
    internal class GroupMemberSerivce : IGroupMemberService
    {
        private IRepoWrapper repos;
        private IMapper mapper;

        public GroupMemberSerivce(IRepoWrapper repos, IMapper mapper)
        {
            this.repos = repos;
            this.mapper = mapper;
        }

        public async Task<bool> AnyAsync(int id)
        {
            return await repos.GroupMembers.GetList().AnyAsync(x => x.Id == id);
        }

        public async Task<bool> AnyInviteAsync(int studentId, int groupId)
        {
            return await repos.Invites.GetList().AnyAsync(x => x.AccountId == studentId && x.GroupId == groupId);
        }
        public async Task<bool> AnyRequestAsync(int studentId, int groupId)
        {
            return await repos.Requests.GetList().AnyAsync(x => x.Id == studentId && x.GroupId == groupId);
        }
        public async Task<GroupMember> GetByIdAsync(int inviteId)
        {
            return await repos.GroupMembers.GetList().FirstOrDefaultAsync(x => x.Id == inviteId);
        }
        public IQueryable<AccountProfileDto> GetMembersJoinForGroup(int groupId)
        {
            IQueryable<Account> list = repos.GroupMembers.GetList()
                .Where(e => e.GroupId == groupId && e.IsActive == true)
                .Include(e => e.Account)
                .Select(e => e.Account);
            return list.ProjectTo<AccountProfileDto>(mapper.ConfigurationProvider);
        }
        //Fix later
        public IQueryable<JoinRequestForGroupGetDto> GetJoinRequestForGroup(int groupId)
        {
            IQueryable<Request> list = repos.Requests.GetList()
                .Where(e => e.GroupId == groupId && e.State == RequestStateEnum.Waiting)
                .Include(e => e.Account)
                .Include(e => e.Group);
            //Console.WriteLine("+_+_+_+_+_+_ " + list.Count());
            return list.ProjectTo<JoinRequestForGroupGetDto>(mapper.ConfigurationProvider);
        }


        public IQueryable<JoinInviteForGroupGetDto> GetJoinInviteForGroup(int groupId)
        {
            IQueryable<Invite> list = repos.Invites.GetList()
                .Where(e => e.GroupId == groupId && e.State == RequestStateEnum.Waiting)
                .Include(e => e.Account)
                .Include(e => e.Group);
            return list.ProjectTo<JoinInviteForGroupGetDto>(mapper.ConfigurationProvider);
        }


        public IQueryable<JoinRequestForStudentGetDto> GetJoinRequestForStudent(int studentId)
        {
            IQueryable<Request> list = repos.Requests.GetList()
                .Where(e => e.AccountId == studentId && e.State == RequestStateEnum.Waiting)
                .Include(e => e.Account)
                .Include(e => e.Group).ThenInclude(e => e.GroupSubjects).ThenInclude(e => e.Subject)
                .Include(e => e.Group).ThenInclude(e => e.GroupMembers);
            IQueryable<JoinRequestForStudentGetDto> mapped = list.ProjectTo<JoinRequestForStudentGetDto>(mapper.ConfigurationProvider);
            return mapped;
        }

        public async Task<T> GetInviteOfStudentAndGroupAsync<T>(int accountId, int groupId)
        {
            Invite invite = await repos.Invites.GetList()
                .Include(e => e.Account)
                .Include(e => e.Group)
                .SingleOrDefaultAsync(e => e.AccountId == accountId
                    && e.GroupId == groupId 
                    && e.State == RequestStateEnum.Waiting);
            var mapped = mapper.Map<T>(invite);
            return mapped;

        }

        public async Task<Request> GetRequestOfStudentAndGroupAsync(int accountId, int groupId)
        {
            Request request = await repos.Requests.GetList()
                .Include(e => e.Account)
                .Include(e => e.Group)
                .SingleOrDefaultAsync(e => e.AccountId == accountId
                    && e.GroupId == groupId && e.State == RequestStateEnum.Waiting);
            return request;
        }

        public async Task<Invite> GetInviteByIdAsync(int inviteId)
        {
            Invite invite = await repos.Invites.GetList()
                .Include(e => e.Account)
                .Include(e => e.Group)
                .SingleOrDefaultAsync(e => e.Id == inviteId);
            return invite;
        }

        public async Task<Request> GetRequestByIdAsync(int requestId)
        {
            Request request = await repos.Requests.GetList()
                .Include(e => e.Account)
                .Include(e => e.Group)
                .SingleOrDefaultAsync(e => e.Id == requestId);
            return request;
        }


        public IQueryable<JoinInviteForStudentGetDto> GetJoinInviteForStudent(int studentId)
        {
            IQueryable<Invite> list = repos.Invites.GetList()
                .Where(e => e.AccountId == studentId && e.State == RequestStateEnum.Waiting)
                .Include(e => e.Account)
                .Include(e => e.Group).ThenInclude(e => e.GroupSubjects).ThenInclude(e => e.Subject)
                .Include(e => e.Group).ThenInclude(e => e.GroupMembers);
            return list.ProjectTo<JoinInviteForStudentGetDto>(mapper.ConfigurationProvider);
        }

        public async Task CreateJoinInvite(GroupMemberInviteCreateDto dto)
        {
            //GroupMember invite = mapper.Map<GroupMember>(dto);
            //await repos.GroupMembers.CreateAsync(invite);
            Invite invite = mapper.Map<Invite>(dto);
            await repos.Invites.CreateAsync(invite);
        }

        public async Task CreateJoinRequest(GroupMemberRequestCreateDto dto)
        {
            //GroupMember request = mapper.Map<GroupMember>(dto);
            //await repos.GroupMembers.CreateAsync(request);
            Request request = mapper.Map<Request>(dto);
            await repos.Requests.CreateAsync(request);
        }

        public async Task<T> GetGroupMemberOfStudentAndGroupAsync<T>(int studentId, int groupId)
        {
            var groupMember = await repos.GroupMembers.GetList()
               .SingleOrDefaultAsync(e => e.AccountId == studentId && e.GroupId == groupId);
            var mapped = mapper.Map<T>(groupMember);
            return mapped;
        }

        public async Task AcceptOrDeclineInviteAsync(Invite existedInvite, bool isAccepted)
        {
            //if (existed.State != GroupMemberState.Inviting)
            //{
            //    throw new Exception("Đây không phải là thư mời");
            //}
            //existed.State = isAccepted ? GroupMemberState.Member : GroupMemberState.Banned;
            //await repos.GroupMembers.UpdateAsync(existed);
            existedInvite.State = isAccepted ? RequestStateEnum.Approved : RequestStateEnum.Decline;
            await repos.Invites.UpdateAsync(existedInvite);
            if (isAccepted)
            {
                GroupMember newMember = new GroupMember
                {
                    AccountId = existedInvite.AccountId,
                    GroupId = existedInvite.GroupId,
                    MemberRole = GroupMemberRole.Member,
                    IsActive = true,
                };
                await repos.GroupMembers.CreateAsync(newMember);
            }
        }

        public async Task AcceptOrDeclineRequestAsync(Request existedRequest, bool isAccepted)
        {
            //if (existed.State != InviteRequestStateEnum.Waiting)
            //{
            //    throw new Exception("Yêu cầu đã được xử lí");
            //}
            existedRequest.State = isAccepted ? RequestStateEnum.Approved : RequestStateEnum.Decline;
            await repos.Requests.UpdateAsync(existedRequest);
            if (isAccepted)
            {
                GroupMember newMember = new GroupMember
                {
                    AccountId = existedRequest.AccountId,
                    GroupId = existedRequest.GroupId,
                    MemberRole = GroupMemberRole.Member,
                    IsActive = true,
                };
                await repos.GroupMembers.CreateAsync(newMember);
            }
        }

        public async Task BanUserFromGroupAsync(GroupMember banned)
        {
            //banned.IsActive = false;
            //await repos.GroupMembers.UpdateAsync(banned);
            await repos.GroupMembers.RemoveAsync(banned.Id);
        }

        public async Task<bool> LeaveGroup(int accountId, int groupId)
        {

            var groupMember = await repos.GroupMembers.GetList()
               .SingleOrDefaultAsync(e => e.AccountId == accountId && e.GroupId == groupId);

            //groupMember.IsActive = false;
            //await repos.GroupMembers.UpdateAsync(groupMember);
            await repos.GroupMembers.RemoveAsync(groupMember.Id);

            return true;
        }
    }
}