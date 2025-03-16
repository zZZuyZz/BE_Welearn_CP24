﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DbObject
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(32)]
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public string? ImagePath { get; set; }

        [Column("Dob")]
        public DateTime? DateOfBirth { get; set; }

        public int ReportCounter { get; set; }

        public bool IsBanned { get; set; }

        //Role
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }

        // Group Member
        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new Collection<GroupMember>();
        public virtual ICollection<Connection> Connections { get; set; } = new Collection<Connection>();
        public virtual ICollection<Report> ReportedReports { get; set; } = new Collection<Report>();


        #region Invite
        public virtual ICollection<Invite> JoinInvites { get; set; } = new Collection<Invite>();
        #endregion

        #region Request
        public virtual ICollection<Request> JoinRequests { get; set; } = new Collection<Request>();
        #endregion


    }

}
