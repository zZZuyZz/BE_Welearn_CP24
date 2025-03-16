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
    public interface IDiscussionService
    {
        public Task<DiscussionDto> GetDiscussionById(int discussionid);
        public Task<List<DiscussionDto>> GetDiscussionsByGroupId(int groupId);
        public Task<DiscussionDto> UploadDiscussion(int accountId, int groupId, UploadDiscussionDto discussionDto);
        /// <summary>
        /// Upload file to firebase when writing discusision
        /// </summary>
        /// <param name="file"></param>
        /// <returns>file path</returns>
        public Task<string> UploadDiscussionFile(IFormFile file);
        public Task<DiscussionDto> UpdateDiscussion(int discussionId, UploadDiscussionDto discussionDto);
    }
}
