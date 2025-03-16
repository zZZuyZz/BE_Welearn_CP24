using DataLayer.DbObject;
using Microsoft.AspNetCore.Http;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface.Db
{
    public interface IGroupService
    {
        public IQueryable<T> GetList<T>();
        public Task<T> GetFullByIdAsync<T>(int id);
        /// <summary>
        /// Create a group and add group leader
        /// </summary>
        /// <param name="group"></param>
        /// <param name="creatorId">id of creator account id</param>
        /// <returns></returns>
        public Task CreateAsync(GroupCreateDto dto, int creatorId);
        //public Task UpdateAsync(Group group);
        public Task<GroupDetailForLeaderGetDto> UpdateAsync(int groupId, GroupUpdateDto dto);
        /// <summary>
        /// DO NOT USE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task RemoveAsync(int id);
        /// <summary>
        /// Get all groups student (member) has joined
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> GetMemberGroupsOfStudentAsync<T>(int studentId);
        /// <summary>
        /// Get all groups leader has created
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> GetLeaderGroupsOfStudentAsync<T>(int leaderId);
        /// <summary>
        /// Get all groups student (leader, member) has joined
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> GetJoinGroupsOfStudentAsync<T>(int studentId);
        public Task<List<int>> GetLeaderGroupsIdAsync(int leaderId);
        /// <summary>
        /// Check if a student is a leader of a group
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<bool> IsStudentLeadingGroupAsync(int studentId, int groupId);
        /// <summary>
        /// Check if a student is a member (member only) of a group
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<bool> IsStudentMemberGroupAsync(int studentId, int groupId);
        /// <summary>
        /// Check if a student (member, leader) join a group
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<bool> IsStudentJoiningGroupAsync(int studentId, int groupId);
        /// <summary>
        /// Check if a student is requesting to join a group
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<bool> IsStudentRequestingToGroupAsync(int studentId, int groupId);
        /// <summary>
        /// Check if a student is invited to join a group
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<bool> IsStudentInvitedToGroupAsync(int studentId, int groupId);
        /// <summary>
        /// Check if a student is invited to join a group
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Task<bool> IsStudentBannedToGroupAsync(int studentId, int groupId);
        /// <summary>
        /// search by name, class, subject
        /// </summary>
        /// <param name="search"></param>
        /// <param name="studentId"></param>
        /// <param name="newGroup"></param>
        /// <returns></returns>
        public Task<IQueryable<T>> SearchGroups<T>(string search, int studentId, bool newGroup);
        public Task<IQueryable<T>> SearchGroupsWithCode<T>(string code, int studentId, bool newGroup);
        public Task<IQueryable<T>> SearchGroupsBySubject<T>(string search, int studentId, bool newGroup);
        public IQueryable<T> GetGroupsNotJoined<T>(int accountId);
        public Task<bool> ExistsAsync(int groupId);
    }
}
