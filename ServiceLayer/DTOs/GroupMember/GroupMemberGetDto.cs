using DataLayer.Enums;

namespace ServiceLayer.DTOs
{
    public class GroupMemberGetDto : BaseGetDto
    {
        public int Id { get; set; }
        public GroupMemberRole State { get; set; }
        public string InviteMessage { get; set; }
        public string RequestMessage { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string ImagePath { get; set; }
        public int AccountId { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        public GroupMemberRole MemberRole { get; set; }
    }
}