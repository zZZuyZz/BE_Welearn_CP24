﻿using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class ScheduleMeetingMassCreateDto: BaseCreateDto
    {
        public int GroupId { get; set; }

        private string name;
		public string Name
		{
			get { return name.Trim(); }
			set { name = value.Trim(); }
		}
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value.Trim(); }
        }
        private TimeSpan scheduleStartTime;
        public TimeSpan ScheduleStartTime
        {
            get { return scheduleStartTime; }
            set { scheduleStartTime = value; }
        }

        private TimeSpan scheduleEndTime;
        public TimeSpan ScheduleEndTime
        {
            get { return scheduleEndTime; }
            set { scheduleEndTime = value; }
        }

        public ICollection<DayOfWeek> DayOfWeeks { get; set; }

        private DateTime scheduleSRangeStart;
		public DateTime ScheduleRangeStart
		{
            //get { return scheduleSRangeStart.Date; }
            //set { scheduleSRangeStart = value.Date; }
            get { return scheduleSRangeStart; }
            set { scheduleSRangeStart = value; }
        }
        
        private DateTime scheduleRangeEnd;
        public DateTime ScheduleRangeEnd
        {
            //get { return scheduleRangeEnd.Date; }
            //set { scheduleRangeEnd = value.Date; }
            get { return scheduleRangeEnd; }
            set { scheduleRangeEnd = value; }
        }
        public virtual ICollection<SubjectEnum> SubjectIds { get; set; }
    }
}
