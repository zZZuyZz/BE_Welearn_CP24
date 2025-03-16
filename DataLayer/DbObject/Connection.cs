using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataLayer.DbObject
{
    [Table("MeetingParticipations")]
    [Index(nameof(SinganlrId), IsUnique = true)]
    public class Connection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SinganlrId { get; set; }
        public string? PeerId { get; set; }
        public DateTime Start { get;set; }
        public DateTime? End { get;set; }

        #region Meeting
        [ForeignKey("MeetingId")]
        public int MeetingId { get; set; }
        public virtual Meeting Meeting { get; set; }
        #endregion


        #region Student
        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        public string UserName { get; set; }

        public virtual Account Account { get; set; }
        #endregion
    }
}