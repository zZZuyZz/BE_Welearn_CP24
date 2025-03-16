using AutoMapper;
using DataLayer.DbObject;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Db;
using ServiceLayer.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ServiceLayer.Services.Implementation.Db
{
    public class DiscussionService : IDiscussionService
    {
        private IRepoWrapper _repos;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        public DiscussionService(IRepoWrapper repoWrapper, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _mapper = mapper;
            _repos = repoWrapper;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

        public async Task<DiscussionDto> GetDiscussionById(int discussionId)
        {
            var discussion = await _repos.Discussions.GetByIdAsync(discussionId);
            var mapper = _mapper.Map<DiscussionDto>(discussion);
            return mapper;
        }

        public async Task<List<DiscussionDto>> GetDiscussionsByGroupId(int groupId)
        {
            var discussion = await _repos.Discussions.GetDiscussionsByGroupId(groupId);
            var mapper = _mapper.Map<List<DiscussionDto>>(discussion);
            return mapper;
        }

        public async Task<DiscussionDto> UpdateDiscussion(int discussionId, UploadDiscussionDto discussionDto)
        {
            var discussion = await _repos.Discussions.GetByIdAsync(discussionId);
                      
            if (discussionDto.File != null && discussionDto.File.Length > 0)
            {
                string firebaseBucket = _config["Firebase:StorageBucket"];

                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + discussionDto.File.FileName;

                // Get reference to the file in Firebase Storage
                var fileReference = firebaseStorage.Child("DiscussionFiles").Child(uniqueFileName);

                // Upload the file to Firebase Storage
                using (var stream = discussionDto.File.OpenReadStream())
                {
                    await fileReference.PutAsync(stream);
                }

                // Get the download URL of the uploaded file
                string downloadUrl = await fileReference.GetDownloadUrlAsync();

                // Update the discussion entity with the download URL
                discussion.FilePath = downloadUrl;
            }
            discussion.CreateAt = DateTime.Now;
            discussion.PatchUpdate(discussionDto);

            await _repos.Discussions.UpdateAsync(discussion);
            var mapped = _mapper.Map<DiscussionDto>(discussion);
            return mapped;
        }

        public async Task<string> UploadDiscussionFile(IFormFile file)
        {
            string filePath;

            if (file != null && file.Length > 0)
            {
                string firebaseBucket = _config["Firebase:StorageBucket"];

                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                // Get reference to the file in Firebase Storage
                var fileReference = firebaseStorage.Child("DiscussionFiles").Child(uniqueFileName);

                // Upload the file to Firebase Storage
                using (var stream = file.OpenReadStream())
                {
                    await fileReference.PutAsync(stream);
                }

                // Get the download URL of the uploaded file
                string downloadUrl = await fileReference.GetDownloadUrlAsync();

                // Update the discussion entity with the download URL
                return downloadUrl;
            }
            throw new Exception("No file found");

        }
        public async Task<DiscussionDto> UploadDiscussion(int accountId, int groupId, UploadDiscussionDto discussionDto)
        {
            string filePath;
            Discussion discussion = _mapper.Map<Discussion>(discussionDto);
            discussion.GroupId = groupId;
            discussion.AccountId = accountId;
            discussion.IsActive = true;

            if (discussionDto.File != null && discussionDto.File.Length > 0)
            {
                string firebaseBucket = _config["Firebase:StorageBucket"];

                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + discussionDto.File.FileName;

                // Get reference to the file in Firebase Storage
                var fileReference = firebaseStorage.Child("DiscussionFiles").Child(uniqueFileName);

                // Upload the file to Firebase Storage
                using (var stream = discussionDto.File.OpenReadStream())
                {
                    await fileReference.PutAsync(stream);
                }

                // Get the download URL of the uploaded file
                string downloadUrl = await fileReference.GetDownloadUrlAsync();

                // Update the discussion entity with the download URL
                discussion.FilePath = downloadUrl;
            }
            discussion.CreateAt = DateTime.Now;
            await _repos.Discussions.CreateAsync(discussion);
            var mapped = _mapper.Map<DiscussionDto>(discussion);
            return mapped;

        }
    }
}
