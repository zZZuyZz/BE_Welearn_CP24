using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class UploadDiscussionDto
    {
        public string? Question { get; set; }
        public string? Content { get; set; }
        public IFormFile? File { get; set; }
    }
}
