namespace API.SwaggerOption.Descriptions
{
    public class GroupMembersDescriptions
    {
        public const string CreateInvite = "Nhóm trưởng tạo thư mời vào nhóm<br>" +
                "groupId id của nhóm mời vào<br>" +
                "accountId id của học sinh được mời vào";
        public const string AcceptRequest = "Nhóm trưởng chấp nhận request vào nhóm của học sinh";
        public const string DeclineRequest = "Nhóm trưởng từ chối request vào nhóm của học sinh";
        public const string CreateRequest = "Học sinh tạo request vào nhóm mới<br>" +
                "groupId: id của nhóm học sinh muốn vào<br>" +
                "accountId: id của học sinh đang muốn vào";
        public const string AcceptInvite = "Học sinh chấp nhận lời mời vào nhóm";
        public const string DeclineInvite = "Học sinh từ chối lời mời vào nhóm";
        public const string BanMember = "Leader ban member khỏi nhóm<br>" +
                "groupId: id của nhóm<br>" +
                "banAccId: id của member bị ban";
        public const string LeaveGroup = "Leave group";
    }
}
