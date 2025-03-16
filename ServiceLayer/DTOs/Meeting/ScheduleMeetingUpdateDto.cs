using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class ScheduleMeetingUpdateDto  : BaseUpdateDto
    {
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
        private DateTime date;

        public DateTime Date /*{ get; set; }*/
        {
            //get { return date; }
            //set { date = value.Date; }
            get { return date; }
            set { date = value; }
        }

        public TimeSpan ScheduleStartTime { get; set; }
        public TimeSpan ScheduleEndTime { get; set; }
    }
}
