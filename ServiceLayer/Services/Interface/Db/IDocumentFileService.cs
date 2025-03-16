using DataLayer.DbObject;
using Microsoft.AspNetCore.Http;
using ServiceLayer.DTOs;

namespace ServiceLayer.Services.Interface.Db;

public interface IDocumentFileService
{
    public Task<List<DocumentFileDto>> GetDocumentFilesByGroupId(int groupId);
    public Task<DocumentFileDto> ApproveRejectFile(int documentId, Boolean check);
    public Task<DocumentFileDto> UploadDocumentFIle(IFormFile fileUpload, int groupId, int accountId, bool isLeader);
}