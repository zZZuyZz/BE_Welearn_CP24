﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class ScheduleMeetingGetDto : BaseGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? ScheduleStart { get; set; }
        public DateTime? ScheduleEnd { get; set; }
        public int GroupId { get; set; }
        public int ScheduleGroupId { get; set; }
        public string GroupName { get; set; }
        public ICollection<SubjectGetDto> Subjects { get; set; }
        public bool CanStart { get; set; }

    }
    public class ScheduleMeetingForMemberGetDto : ScheduleMeetingGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? ScheduleStart { get; set; }
        public DateTime? ScheduleEnd { get; set; }
        public int GroupId { get; set; }
        public int ScheduleGroupId { get; set; }
        public string GroupName { get; set; }
        public ICollection<SubjectGetDto> Subjects { get; set; }
        public bool CanStart { get; set; } 

    }
    public class ScheduleMeetingForLeaderGetDto : ScheduleMeetingGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? ScheduleStart { get; set; }
        public DateTime? ScheduleEnd { get; set; }
        public int GroupId { get; set; }
        public int ScheduleGroupId { get; set; }
        public string GroupName { get; set; }
        public ICollection<SubjectGetDto> Subjects { get; set; }
        public bool CanStart { get; set; } 

    }
}
