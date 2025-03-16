using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class DiscussionInGroupDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountUsername { get; set; }
        public string AccountFullname { get; set; }
        public string AccountImagePath { get; set; }
        public int GroupId { get; set; }
        public string? Question { get; set; }
        public string? Content { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<AnswerDiscussionDto> AnswerDiscussions { get; set; }
    }
}
