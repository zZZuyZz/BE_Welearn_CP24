namespace API.SwaggerOption.Descriptions
{
    public class GroupsDescriptions
    {
        public const string SearchGroup = "Search group theo name, class, môn, id<br>" +
                "newGroup thành true nếu không muốn search những nhóm cũ (nên để)";
        public const string SearchGroupCode = "Search group theo invite code<br>" +
                "sẽ thông báo nếu đã tham gia nhóm";
        public const string SearchGroupByClassa = "Search group theo class<br>" +
               "newGroup thành true nếu không muốn search những nhóm cũ (nên để)";
        public const string SearchGroupBySubject = "Search group theo class<br>" +
               "newGroup thành true nếu không muốn search những nhóm cũ (nên để)";
        public const string GetJoinedGroups = "Lấy list nhóm mà học sinh đã tham gia (role là leader hoặc member)";
        public const string GetMemberGroups = "Lấy list nhóm mà học sinh đã tham gia (role member)";
        public const string GetGroupDetailForMember = "Lấy chi tiết group cho member (ít thông tin hơn)";
        public const string GetLeadGroups = "Lấy list nhóm mà học sinh đã tạo(role leader)";
        public const string GetGroupDetailForLeader = "Lấy chi tiết group cho leader (nhiều thông tin hơn)";
        public const string CreateGroup = "create new group for leader<br>" +
                "Ai tạo nhóm sẽ trở thành trưởng nhóm";
        public const string UpdateGroup = "groupId Id nhóm dc update";
        public const string GetGroups = "Get leadGroupIds of group";
        public const string GetGroup = "Get group by Id";
        public const string GetAllGroups = "Get all groups";
        public const string GetNotJoinedGroups = "Get not joined groups";

    }
}
