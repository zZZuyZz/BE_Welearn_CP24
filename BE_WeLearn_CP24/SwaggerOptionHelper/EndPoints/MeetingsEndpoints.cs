using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class MeetingsEndpoints
    {
        public const string GetAllMeetingForGroup = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all meetings of group";
        public const string GetPastMeetingForGroup = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all past meetings of group";
        public const string GetScheduleMeetingForGroup = $"[{Actor.Student}/{Finnished.True}/{Auth.True}] Get all Schedule meetings of group";
        public const string GetLiveMeetingForGroup = $"[{Actor.Student}/{Finnished.True}/{Auth.True}] Get all Live meetings of group";
        public const string GetAllMeetingForStudent = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all past meetings of student";
        public const string GetPastMeetingForStudent = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all past meetings of student";
        public const string GetPastMeetingForStudentByMonth = $"[{Actor.Student}/{Finnished.True}/{Auth.True}] Get all past meetings of group";
        public const string GetScheduleMeetingForStudent = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all past meetings of student";
        public const string GetLiveMeetingForStudent = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all past meetings of student";
        public const string CreateInstantMeeting = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Create a new instant meeting";
        public const string MassCreateScheduleMeeting = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Mass create many schedule meetings within a range of time";
        public const string CreateScheduleMeeting = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Create a new schedule meeting";
        public const string StartSchdeuleMeeting = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Start a schedule meeting";
        public const string UpdateScheduleMeeting = $"[{Actor.Leader}/{Finnished.No_Test}/{Auth.True}] Update a schedule meeting";
        public const string DeleteSchdeuleMeeting = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Remove a schedule meeting";
        public const string GetChildrenLiveMeeting = $"[{Actor.Parent}/{Finnished.True}/{Auth.True}] Remove a schedule meeting";
    }
}
