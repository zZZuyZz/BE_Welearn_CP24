namespace API.Descriptions
{
    public class MeetingDescriptions
    {
        public const string GetPastMeetingForStudentByMonth = "month (yyyy-mm-dd): chỉ cần năm với tháng, day nhập đại";
        public const string MassCreateScheduleMeeting = "ScheduleSRangeStart: chỉ cần date, time ko quan trọng nhưng vẫn phải điền (cho 00:00:00)<br/>" +
                "chủ nhật là 1, thứ 2-7 là 2-7 ";
    }
}
