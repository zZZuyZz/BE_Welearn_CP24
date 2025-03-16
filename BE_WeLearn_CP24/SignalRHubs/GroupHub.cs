using API.Extension.ClaimsPrinciple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface;

namespace API.SignalRHub
{
    /// <summary>
    /// Use to count number of ppl in rooms
    /// </summary>
    //[Authorize]
    public class GroupHub : Hub
    {
        private readonly IRepoWrapper repos;
        private readonly IServiceWrapper services;

        public GroupHub(IServiceWrapper services, IRepoWrapper repos)
        {
            this.services = services;
            this.repos = repos;
        }
        //BE: SendAsync(GroupHub.CountMemberInGroupMsg, new { meetingId: int, countMember: int })
        public static string CountMemberInGroupMsg => "CountMemberInGroup";
        public static string OnLockedUserMsg => "OnLockedUser";
        public static string OnReloadGroupMsg => "OnReloadGroup";
        public static string OnReloadMeetingMsg => "OnReloadMeeting";
        public static string OnReloadDocumentMsg => "OnReloadDocument";
        public static string OnReloadDicussionMsg => "OnReloadDicussion";

        public static string OnReloadSelfInfoMsg => "OnReloadSelfInfo";
        public static string OnReloadSelfMeetingMsg => "OnReloadSelfMeeting";

        public override async Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();
            string groupIdString = httpContext.Request.Query["groupId"].ToString();
            if (groupIdString.ToLower() == "all")
            {
                //string username = Context.User.GetUsername();
                string accIdString = httpContext.Request.Query["accId"].ToString();
                try
                {
                    int accId = int.Parse(accIdString);
                    await Groups.AddToGroupAsync(Context.ConnectionId, "accId" + accIdString);
                    //await Groups.AddToGroupAsync(Context.ConnectionId, "a");
                    IQueryable<string> ids = (await services.Groups.GetJoinGroupsOfStudentAsync<GroupGetListDto>(accId)).Select(g => g.Id.ToString());
                    foreach (var id in ids)
                    {
                        //Groups.AddToGroupAsync(Context.ConnectionId, id);
                        await Groups.AddToGroupAsync(Context.ConnectionId, id);
                    }
                }
                catch (Exception ex) { 
                    Console.WriteLine(ex.Message);    
                }
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, groupIdString);

            //var isOnline = await presenceTracker.UserConnected(new UserConnectionSignalrDto(Context.User.GetUsername(), 0), Context.ConnectionId);
            base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            HttpContext httpContext = Context.GetHttpContext();
            string groupIdString = httpContext.Request.Query["groupId"].ToString();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupIdString);

            //var isOffline = await presenceTracker.UserDisconnected(new UserConnectionSignalrDto(Context.User.GetUsername(), 0), Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        //TestOnly
        public async Task TestReceiveInvoke(string msg)
        {
            Console.WriteLine("+++++++++++==================== " + msg + " group ReceiveInvoke successfull");
            //int meetId = presenceTracker.
            Clients.Caller.SendAsync("OnTestReceiveInvoke", "group invoke dc rồi ae ơi " + msg);
        }
    }
}
