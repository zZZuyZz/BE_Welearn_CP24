using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class LoginInfoDto
    {
        //token = token, Id = logined.Id, Username = logined.Username, Email = logined.Email, Role = logined.Role.Name
        public int Id { get; set; }
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }

        public string RoleName { get; set; }
        public string Role => RoleName;
        //public string? Schhool { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsBanned { get; set; }
        public string Token { get; set; }
        public virtual ICollection<GroupGetListDto> LeadGroups { get; set; }// = new Collection<GroupGetListDto>();
        public virtual ICollection<GroupGetListDto> JoinGroups { get; set; }// = new Collection<GroupGetListDto>();

    }
}
