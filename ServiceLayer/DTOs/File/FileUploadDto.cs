using DataLayer.Enums;
using Microsoft.AspNetCore.Http;

namespace ServiceLayer.DTOs;

public class FileUploadDto
{
    public IFormFile FileDetails { get; set; }
    public FileTypeEnum FileType { get; set; }
}