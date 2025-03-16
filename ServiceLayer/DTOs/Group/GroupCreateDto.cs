using DataLayer.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class GroupCreateDto : BaseCreateDto
    {
        private string name;

        public string Name
        {
            get { return name.Trim(); }
            set { name = value.Trim(); }
        }
        private string description;
        public string? Description
        {
            get { return description.Trim(); }
            set { description = value.Trim(); }

        }
        public IFormFile? Image { get; set; }
        //public int ClassId { get; set; }
        //public virtual ICollection<SubjectEnum> SubjectIds { get; set; }
        public virtual ICollection<int> SubjectIds { get; set; }

    }
}
