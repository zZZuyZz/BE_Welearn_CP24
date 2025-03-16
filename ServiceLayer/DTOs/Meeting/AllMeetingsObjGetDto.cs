using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs.Meeting
{
    public class AllMeetingsObjGetDto
    {
        public IQueryable<ScheduleMeetingCreateDto> Schedules { get; set; }
    }
}
