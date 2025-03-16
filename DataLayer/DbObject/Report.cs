using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DataLayer.Enums;

namespace DataLayer.DbObject
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Detail { get; set; }
        public RequestStateEnum State { get; set; }

        //Sender
        [ForeignKey("SenderId")]
        public int SenderId { get; set; }
        public virtual Account Sender { get; set; }

        //Account
        [ForeignKey("AccountId")]
        public int? AccountId { get; set; }
        public virtual Account? Account { get; set; }

        //Group
        [ForeignKey("GroupId")]
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }

        //Group
        [ForeignKey("DiscussionId")]
        public int? DiscussionId { get; set; }
        public virtual Discussion? Discussion { get; set; }

        //Group
        [ForeignKey("FileId")]
        public int? FileId { get; set; }
        public virtual DocumentFile? File { get; set; }
    }
}
