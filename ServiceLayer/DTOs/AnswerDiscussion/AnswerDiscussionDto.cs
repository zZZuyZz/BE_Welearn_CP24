using DataLayer.DbObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class AnswerDiscussionDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int DiscussionId { get; set; }
        public string? Content { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; } 
    }
}
