using API.SwaggerOption.Const;

namespace API.SwaggerOption.Endpoints
{
    public class GroupsEndpoints
    {
        public const string SearchGroup = $"[{Actor.Student}/{Finnished.True}]Search list of groups for student";
        public const string SearchGroupCode = $"[{Actor.Student}/{Finnished.True}]Search list of groups for student with invite code";
        public const string SearchGroupByClass = $"[{Actor.Student}/{Finnished.True}]Search list of groups by classs for student";
        public const string SearchGroupBySubject = $"[{Actor.Student}/{Finnished.True}]Search list of groups by subject for student";
        public const string GetJoinedGroups = $"[{Actor.Student}/{Finnished.True}]Get list of groups student joined";
        public const string GetMemberGroups = $"[{Actor.Student}/{Finnished.True}]Get list of groups student joined as a member";
        public const string GetGroupDetailForMember = $"[{Actor.Student}/{Finnished.True}]Get group detail for a member";
        public const string GetLeadGroups = $"[{Actor.Leader}/{Finnished.True}] Get list of groups where student is leader";
        public const string GetGroupDetailForLeader = $"[{Actor.Leader}/{Finnished.True}/{Auth.True}] Get group detail for leader by Id";
        public const string CreateGroup = $"[{Actor.Leader}/{Finnished.True}] create new group for leader";
        public const string UpdateGroup = $"[{Actor.Leader}/{Finnished.True}] Update group for leader";
        public const string GetGroups = $"[{Actor.Student}/{Finnished.True}]Get list of groups";
        public const string GetGroup = $"[{Actor.Test}/{Finnished.True}] Get group by Id";
        public const string GetNotJoinedGroups = $"[{Actor.Student}/{Finnished.True}]Get list of groups which is not joined";

    }
}
