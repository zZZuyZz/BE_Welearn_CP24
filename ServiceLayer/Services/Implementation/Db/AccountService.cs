using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DbObject;
using DataLayer.Enums;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using ServiceLayer.Utils;
using ServiceLayer.DTOs;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using ServiceLayer.Services.Interface.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;
using Firebase.Storage;
using ServiceLayer.Validation.FileUpload;
using APIExtension.Validator;
using Microsoft.Extensions.Configuration;

namespace ServiceLayer.Services.Implementation.Db
{
    public class AccountService : IAccountService
    {
        private IRepoWrapper repos;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;

        public AccountService(IRepoWrapper repos, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.repos = repos;
            this.mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
        }
        public IQueryable<T> GetList<T>()
        {
            var list = repos.Accounts.GetList();
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }
        public IQueryable<T> SearchStudents<T>(string search, int? groupId, int? parentId)
        {
            search = search.ToLower().Trim();
            if (parentId.HasValue)
            {
                var list = repos.Accounts.GetList()
               .Include(e => e.GroupMembers).ThenInclude(e => e.Group)
               .Where(e =>
                    e.RoleId == (int)RoleNameEnum.Student
                    //&& !e.SupervisionsForStudent.Any(e => e.ParentId == parentId)
                    && (EF.Functions.Like(e.Id.ToString(), search + "%")
                    || e.Email.ToLower().Contains(search)
                    || e.Username.ToLower().Contains(search)
                    || e.FullName.ToLower().Contains(search))
               );
                var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
                return mapped;
            }
            else if (groupId.HasValue)
            {
                var list = repos.Accounts.GetList()
                .Include(e => e.GroupMembers).ThenInclude(e => e.Group)
                .Where(e =>
                    e.RoleId == (int)RoleNameEnum.Student
                    && !e.GroupMembers.Any(e => e.GroupId == groupId)
                    && (EF.Functions.Like(e.Id.ToString(), search + "%")
                    || e.Email.ToLower().Contains(search)
                    || e.Username.ToLower().Contains(search)
                    || e.FullName.ToLower().Contains(search))
                );
                var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
                return mapped;
            }
            else
            {
                var list = repos.Accounts.GetList()
                .Include(e => e.GroupMembers).ThenInclude(e => e.Group)
                .Where(e =>
                    e.RoleId == (int)RoleNameEnum.Student
                    && (EF.Functions.Like(e.Id.ToString(), search + "%")
                    //e.Id.ToString().Contains(search)
                    //SqlFunctions.StringConvert((double)e.Id) 
                    || e.Email.ToLower().Contains(search)
                    || e.Username.ToLower().Contains(search)
                    || e.FullName.ToLower().Contains(search))
                );
                var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
                return mapped;
            }
        }


        public async Task<T> GetByIdAsync<T>(int id)
        {
            var user = await repos.Accounts.GetByIdAsync(id);
            var mapped = mapper.Map<T>(user);
            return mapped;
        }

        public async Task<T> GetProfileByIdAsync<T>(int id)
        {
            var user = await repos.Accounts.GetProfileByIdAsync(id);
            var mapped = mapper.Map<T>(user);
            return mapped;
        }

        public async Task<Account> GetAccountByUserNameAsync(string userName)
        {
            Account account = await repos.Accounts.GetByUsernameAsync(userName);
            return account;
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            Account account = await repos.Accounts.GetList()
                .Include(a => a.Role)
                .SingleOrDefaultAsync(e => e.Email == email);
            return account;
        }

        public async Task CreateAsync(Account account, IFormFile? image)
        {
            string filePath;
            if (image != null && image.Length > 0)
            {
                string firebaseBucket = _config["Firebase:StorageBucket"];

                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;

                // Get reference to the file in Firebase Storage
                var fileReference = firebaseStorage.Child("Images").Child(uniqueFileName);

                // Upload the file to Firebase Storage
                using (var stream = image.OpenReadStream())
                {
                    await fileReference.PutAsync(stream);
                }

                // Get the download URL of the uploaded file
                string downloadUrl = await fileReference.GetDownloadUrlAsync();
                account.ImagePath = downloadUrl;
            }

            await repos.Accounts.CreateAsync(account);
        }

        public async Task RemoveAsync(int id)
        {
            await repos.Accounts.RemoveAsync(id);
        }

        public async Task<AccountProfileDto> UpdateAsync(int accountId, AccountUpdateDto dto)
        {
            string filePath;
            Account account = await repos.Accounts.GetByIdAsync(accountId);
            //Image
            if (dto.Image != null && dto.Image.Length > 0)
            {
                if (dto.Image.FileName.HasImageExtension())
                {
                    string firebaseBucket = _config["Firebase:StorageBucket"];

                    // Initialize FirebaseStorage instance
                    var firebaseStorage = new FirebaseStorage(firebaseBucket);

                    // Generate a unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + dto.Image.FileName;

                    // Get reference to the file in Firebase Storage
                    var fileReference = firebaseStorage.Child("Images").Child(uniqueFileName);

                    // Upload the file to Firebase Storage
                    using (var stream = dto.Image.OpenReadStream())
                    {
                        await fileReference.PutAsync(stream);
                    }

                    // Get the download URL of the uploaded file
                    string downloadUrl = await fileReference.GetDownloadUrlAsync();
                    account.ImagePath = downloadUrl;
                }
                else
                {
                    throw new Exception("Not support file type" + nameof(dto.Image.FileName).ToString());
                }
            }
                account.PatchUpdate(dto);
            await repos.Accounts.UpdateAsync(account);
            
            var mapped = mapper.Map<AccountProfileDto>(account);
            return mapped;
        }
        public async Task<AccountProfileDto> UpdatePasswordAsync(int accountId , AccountChangePasswordDto dto)
        {
            var account = await repos.Accounts.GetByIdAsync(accountId);
            account.Password = dto.Password;
            await repos.Accounts.UpdateAsync(account);

            var mapped = mapper.Map<AccountProfileDto>(account);
            return mapped;
        }

        public async Task<bool> ExistAsync(int id)
        {
            return await repos.Accounts.IdExistAsync(id);
        }

        public async Task<bool> ExistUsernameAsync(string username)
        {
            return await repos.Accounts.GetList().AnyAsync(x => x.Username == username);
        }

        public async Task<bool> ExistEmailAsync(string email)
        {
            return await repos.Accounts.GetList().AnyAsync(x => x.Email == email);
        }


    }
}
