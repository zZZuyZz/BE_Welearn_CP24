using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Account
{
    public class BannedAccountDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }

        public string RoleName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? ImagePath { get; set; }
        public int ReportCounter { get; set; }
        public bool IsBanned { get; set; }

        public virtual ICollection<ReportGetListDto> Reports { get; set; }

    }
}
