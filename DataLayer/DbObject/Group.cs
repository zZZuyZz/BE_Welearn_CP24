using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System;

namespace DataLayer.DbObject
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public int BanCounter { get; set; }
        public bool IsBanned { get; set; }

        #region Group Member
        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new Collection<GroupMember>();
        #endregion

        #region Invite
        public virtual ICollection<Invite> JoinInvites { get; set; } = new Collection<Invite>();
        #endregion

        #region Request
        public virtual ICollection<Request> JoinRequests { get; set; } = new Collection<Request>();
        #endregion

        #region Subjects
        public virtual ICollection<GroupSubject> GroupSubjects { get; set; } = new Collection<GroupSubject>();
        #endregion

        #region Schedules
        public virtual ICollection<Schedule> Schedules { get; set; } = new Collection<Schedule>();
        #endregion

        public virtual ICollection<Discussion> Discussions { get; set; } = new Collection<Discussion>();
        public virtual ICollection<Report> ReportedReports { get; set; } = new Collection<Report>();
        public virtual ICollection<DocumentFile> DocumentFiles { get; set; } = new Collection<DocumentFile>();


    }
}