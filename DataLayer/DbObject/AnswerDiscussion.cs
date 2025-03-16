using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DbObject
{
    public class AnswerDiscussion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Student
        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }

        public int DiscussionId { get; set; }
        public virtual Discussion Discussion { get; set; }

        public string? Content { get; set; }
        public DateTime CreateAt { get; set; }
        public string? FilePath { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
