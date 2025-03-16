using DataLayer.DbObject;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class ReportGetListDto
    {
        public int Id { get; set; }
        public string Detail { get; set; }
        public RequestStateEnum State { get; set; }

        //Sender
        public int SenderId { get; set; }
        public virtual StudentGetDto Sender { get; set; }

        //Account
        public int? AccountId { get; set; }
        public virtual AccountProfileDto? Account { get; set; }

        //Group
        public int? GroupId { get; set; }
        //public virtual GroupDetailForLeaderGetDto? Group { get; set; }
        public virtual GroupGetListDto? Group { get; set; }

        //Group
        public int? DiscussionId { get; set; }
        public virtual Discussion? Discussion { get; set; }

        //Group
        public int? FileId { get; set; }
        public virtual DocumentFileDto? File { get; set; }
    }
}
