using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Group
{
    public class BannedGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public int MemberCount { get; set; }
        public int BanCounter { get; set; }
        public bool IsBanned { get; set; }
        public virtual ICollection<ReportGetListDto> Reports { get; set; }
    }
}
