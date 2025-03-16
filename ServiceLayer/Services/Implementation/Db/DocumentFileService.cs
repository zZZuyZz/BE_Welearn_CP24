using AutoMapper;
using DataLayer.DbObject;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Db;
using System.IO;
using System.Runtime.CompilerServices;

namespace ServiceLayer.Services.Implementation.Db;

public class DocumentFileService : IDocumentFileService
{
    private IRepoWrapper repos;
    private IMapper mapper;
    private IWebHostEnvironment _webHostEnvironment;
    private IConfiguration _config;


    public DocumentFileService(IRepoWrapper repos, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        this.repos = repos;
        this.mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _config = configuration;
    }
    public async Task<List<DocumentFileDto>> GetDocumentFilesByGroupId(int groupId)
    {
        var documentFiles = await repos.DocumentFiles.GetDocumentFilesByGroupId(groupId);
        var mapped = mapper.Map<List<DocumentFileDto>>(documentFiles);
        return mapped;
    }
    public async Task<DocumentFileDto> ApproveRejectFile(int documentId ,Boolean check)
    {
        var file = await repos.DocumentFiles.GetByIdAsync(documentId);
        file.Approved = check;
        file.IsActive = check;
        
        await repos.DocumentFiles.ApproveRejectAsync(file);
        return mapper.Map<DocumentFileDto>(file);
    }

    public async Task<DocumentFileDto> UploadDocumentFIle(IFormFile fileUpload, int groupId, int accountId, bool isLeader)
    {
        var file = new DocumentFile();

        if (fileUpload != null && fileUpload.Length > 0)
        {
            string firebaseBucket = _config["Firebase:StorageBucket"];

            // Initialize FirebaseStorage instance
            var firebaseStorage = new FirebaseStorage(firebaseBucket);

            // Generate a unique file name
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;

            // Get reference to the file in Firebase Storage
            var fileReference = firebaseStorage.Child("DocumentFiles").Child(uniqueFileName);

            // Upload the file to Firebase Storage
            using (var stream = fileUpload.OpenReadStream())
            {
                await fileReference.PutAsync(stream);
            }

            // Get the download URL of the uploaded file
            string downloadUrl = await fileReference.GetDownloadUrlAsync();

            // Update the discussion entity with the download URL

            file.HttpLink = downloadUrl;
            file.GroupId = groupId;
            file.AccountId = accountId;
            file.CreatedDate = DateTime.UtcNow;
            file.IsActive = true;

            if (!isLeader)
            {
                file.Approved = false;
            }
            else
            {
                file.Approved = true;
            }

            await repos.DocumentFiles.CreateAsync(file);
        }

        return mapper.Map<DocumentFileDto>(file);

    }
}
