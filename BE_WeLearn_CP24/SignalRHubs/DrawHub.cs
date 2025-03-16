using API.Extension.ClaimsPrinciple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalRHub
{
    /// <summary>
    /// Use to count number of ppl in rooms
    /// </summary>
    //[Authorize]
    public class DrawHub : Hub
    {
        public static string MeetingEndMsg => "MeetingEnd";
        public override async Task OnConnectedAsync()
        {
            HttpContext httpContext = Context.GetHttpContext();
            string meetingIdString = httpContext.Request.Query["meetingId"].ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, meetingIdString);

            bool isDrawExisted = Drawings.ContainsKey(meetingIdString);
            if (!isDrawExisted)
            {
                Drawings.Add(meetingIdString, new List<Drawing>());
            }
            await Clients.Caller.SendAsync("get-drawings", Drawings[meetingIdString]);
            base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            HttpContext httpContext = Context.GetHttpContext();
            string meetingIdString = httpContext.Request.Query["meetingId"].ToString();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, meetingIdString);
                                                     //if(Clients.Group(meetingIdString).)
            await base.OnDisconnectedAsync(exception);
        }
        public async Task Draw(int prevX, int prevY, int currentX, int currentY, string color, int size)
        {
            HttpContext httpContext = Context.GetHttpContext();
            int meetingId = int.Parse(httpContext.Request.Query["meetingId"].ToString());
            string username = httpContext.Request.Query["username"].ToString();
            Drawings[meetingId.ToString()]
                .Add(new Drawing
                {
                    PrevX = prevX,
                    PrevY = prevY,
                    CurrentX = currentX,
                    CurrentY = currentY,
                    Color = color,
                    Size = size,
                    Username = username
                }
            );

            //await Clients.GroupExcept(meetingId.ToString(), Context.ConnectionId).SendAsync("draw", prevX, prevY, currentX, currentY, color, size, username);
            await Clients.Group(meetingId.ToString()).SendAsync("draw", prevX, prevY, currentX, currentY, color, size, username);
        }

        public static readonly Dictionary<string, List<Drawing>> Drawings = new Dictionary<string, List<Drawing>>();
        public class Drawing
        {
            public int PrevX { get; set; }
            public int PrevY { get; set; }
            public int CurrentX { get; set; }
            public int CurrentY { get; set; }
            public string Color { get; set; }
            public int Size { get; set; }
            public string Username { get; set; } 
        }
    }
}
