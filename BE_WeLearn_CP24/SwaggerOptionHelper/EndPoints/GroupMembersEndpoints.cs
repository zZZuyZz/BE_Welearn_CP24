using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class GroupMembersEndpoints
    {
        public const string GetJoinMembersForGroup = $"[{Actor.Leader_Member}/{Finnished.True}/{Auth.True}] Get all members joining of group";
        public const string GetInviteForGroup = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all invite of group for leader";
        public const string GetRequestForGroup = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get all join request of group for leader";
        public const string CreateInvite = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Create Invite to join group for leader";
        public const string AcceptRequest = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Accept join request for leader";
        public const string DeclineRequest = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Decline join request for leader";
        public const string GetInviteForStudent = $"[{Actor.Student}/{Finnished.True}/{Auth.True}] Get all join invite of student";
        public const string GetRequestForStudent = $"[{Actor.Student}/{Finnished.True}/{Auth.True}] Get all request of student";
        public const string CreateRequest = $"[{Actor.Student}/{Finnished.True}/{Auth.True}] Request to join new group for student";
        public const string AcceptInvite = $"[{Actor.Member}/{Finnished.True}/{Auth.True}] Accpet join invite for student";
        public const string DeclineInvite = $"[{Actor.Member}/{Finnished.True}/{Auth.True}] Decline join invite for student";
        public const string BanMember = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Banned user from group for leader";
        public const string LeaveGroup = $"[{Actor.Member}/{Finnished.True}/{Auth.True}] Leave group";

    }
}
