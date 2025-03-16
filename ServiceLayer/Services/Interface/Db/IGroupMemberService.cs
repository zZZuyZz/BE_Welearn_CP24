﻿using DataLayer.DbObject;
using ServiceLayer.DTOs;

namespace ServiceLayer.Services.Interface.Db
{
    public interface IGroupMemberService
    {
        Task<bool> AnyAsync(int id);
        IQueryable<AccountProfileDto> GetMembersJoinForGroup(int id);
        public Task<GroupMember> GetByIdAsync(int inviteId);
        public IQueryable<JoinRequestForGroupGetDto> GetJoinRequestForGroup(int groupId);
        public IQueryable<JoinInviteForGroupGetDto> GetJoinInviteForGroup(int groupId);
        public IQueryable<JoinRequestForStudentGetDto> GetJoinRequestForStudent(int studentId);
        public IQueryable<JoinInviteForStudentGetDto> GetJoinInviteForStudent(int studentId);
        public Task CreateJoinInvite(GroupMemberInviteCreateDto dto);
        public Task CreateJoinRequest(GroupMemberRequestCreateDto dto);
        /// <summary>
        /// Get group member status object between student and group
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<T> GetGroupMemberOfStudentAndGroupAsync<T>(int studentId, int groupId);
        public Task AcceptOrDeclineInviteAsync(Invite existed, bool isAccepted);
        public Task AcceptOrDeclineRequestAsync(Request existed, bool isAccepted);
        public Task<T> GetInviteOfStudentAndGroupAsync<T>(int accountId, int groupId);
        public Task<Request> GetRequestOfStudentAndGroupAsync(int accountId, int groupId);
        public Task<Request> GetRequestByIdAsync(int requestId);
        public Task<Invite> GetInviteByIdAsync(int inviteId);
        public Task BanUserFromGroupAsync(GroupMember banned);
        public Task<bool> AnyInviteAsync(int studentId, int groupId);
        public Task<bool> AnyRequestAsync(int studentId, int groupId);
        public Task<bool> LeaveGroup(int accountId, int groupId);
    }
}