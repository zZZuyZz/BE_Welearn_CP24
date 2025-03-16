using AutoMapper;
using DataLayer.DbObject;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit.Cryptography;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Db;
using ServiceLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Implementation.Db
{
    internal class AnswerDiscussionService : IAnswerDiscussionService
    {
        private IRepoWrapper _repos;
        private IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;
        private IConfiguration _config;

        public AnswerDiscussionService(IRepoWrapper repoWrapper, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _mapper = mapper;
            _repos = repoWrapper;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

        public async Task<List<AnswerDiscussion>> GetAnswerDiscussionByDiscussionId(int discussionId)
        {
            return await _repos.AnswerDiscussions.GetAnswerDiscussionsByDiscussionId(discussionId);
        }

        public async Task<AnswerDiscussionDto> UpdateAnswerDiscussion(int answeDiscussionId, UploadAnswerDiscussionDto answerDiscussionDto)
        {
            string firebaseBucket = _config["Firebase:StorageBucket"];

            var answerDiscussion = await _repos.AnswerDiscussions.GetByIdAsync(answeDiscussionId);

            if (answerDiscussionDto.File != null && answerDiscussionDto.File.Length > 0)
            {
                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + answerDiscussionDto.File.FileName;

                // Get reference to the file in Firebase Storage
                var fileReference = firebaseStorage.Child("DiscussionFiles").Child(uniqueFileName);

                // Upload the file to Firebase Storage
                using (var stream = answerDiscussionDto.File.OpenReadStream())
                {
                    await fileReference.PutAsync(stream);
                }

                // Get the download URL of the uploaded file
                string downloadUrl = await fileReference.GetDownloadUrlAsync();

                // Update the discussion entity with the download URL
                answerDiscussion.FilePath = downloadUrl;
            }

            answerDiscussion.PatchUpdate(answerDiscussionDto);

            await _repos.AnswerDiscussions.UpdateAsync(answerDiscussion);

            var mapped = _mapper.Map<AnswerDiscussionDto>(answerDiscussion);
            return mapped;

        }

        public async Task<AnswerDiscussionDto> UploadAnswerDiscussion(int accountId, int discussionId, UploadAnswerDiscussionDto answerDiscussionDto)
        {
            string filePath;
            string firebaseBucket = _config["Firebase:StorageBucket"];

            AnswerDiscussion answerDiscussion = _mapper.Map<AnswerDiscussion>(answerDiscussionDto);
            answerDiscussion.DiscussionId = discussionId;
            answerDiscussion.AccountId = accountId;
            answerDiscussion.IsActive = true;
            answerDiscussion.CreateAt = DateTime.Now;
         
            if (answerDiscussionDto.File != null && answerDiscussionDto.File.Length > 0)
            {
                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + answerDiscussionDto.File.FileName;

                // Get reference to the file in Firebase Storage
                var fileReference = firebaseStorage.Child("DiscussionFiles").Child(uniqueFileName);

                // Upload the file to Firebase Storage
                using (var stream = answerDiscussionDto.File.OpenReadStream())
                {
                    await fileReference.PutAsync(stream);
                }

                // Get the download URL of the uploaded file
                string downloadUrl = await fileReference.GetDownloadUrlAsync();

                // Update the discussion entity with the download URL
                answerDiscussion.FilePath = downloadUrl;
            }

            await _repos.AnswerDiscussions.CreateAsync(answerDiscussion);

            var mapped = _mapper.Map<AnswerDiscussionDto>(answerDiscussion);
            return mapped;
        }

        public async Task<AnswerDiscussionDto> GetAnswerDiscussionById(int answerDiscussionId)
        {
            var discussion = await _repos.AnswerDiscussions.GetByIdAsync(answerDiscussionId);
            var mapper = _mapper.Map<AnswerDiscussionDto>(discussion);
            return mapper;
        }

    }
}
