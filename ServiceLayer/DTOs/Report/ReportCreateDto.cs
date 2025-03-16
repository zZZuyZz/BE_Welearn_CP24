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
    public class ReportCreateDto
    {
        public string Detail { get; set; }
        private readonly RequestStateEnum State = RequestStateEnum.Waiting;

        public int? AccountId { get; set; }
        public int? GroupId { get; set; }
        public int? DiscussionId { get; set; }
        public int? FileId { get; set; }
    }
}
