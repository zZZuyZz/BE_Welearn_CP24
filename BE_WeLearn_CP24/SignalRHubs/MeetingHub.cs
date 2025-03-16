using API.Extension.ClaimsPrinciple;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DbObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using ServiceLayer.DTOs;

namespace API.SignalRHub
{
    [Authorize]
    public class MeetingHub : Hub
    {
        #region Message
        //public static string UserOnlineInMeetingMsg => "UserOnlineInMeeting";

        //Thông báo tình trạng muteMic của username. Chỉ dùng để thay đổi icon mic trên 
        //màn hình của người khác. Việc truyền mic hay không là do peer trên FE quyết định
        //SendAsync(OnMuteMicroMsg, new { username: String, mute: bool })
        //public static string OnMuteMicroMsg => "OnMuteMicro";

        //Thông báo có Chat Message mới
        //BE SendAsync("NewMessage", MessageSignalrGetDto)
        public static string NewMessageMsg => "NewMessage";

        //Thông báo có người yêu cầu dc vote
        // BE SendAsync(OnStartVoteMsg, ReviewSignalrDTO);
        public static string OnStartVoteMsg => "OnStartVote";

        //Thông báo có người yêu cầu dc vote
        // BE SendAsync("OnEndVote", username);
        public static string OnEndVoteMsg => "OnEndVote";

        //Thông báo Review có thay đổi
        //BE SendAsync(OnStartVoteMsg, ReviewSignalrDTO);
        public static string OnVoteChangeMsg => "OnVoteChange";

        //Thông báo để reload lại list vote của meeting
        //BE SendAsync(OnReloadVoteMsg, List<ReviewSignalrDTO>);
        public static string OnReloadVoteMsg => "OnReloadVote";

        //public static string OnNewVoteResultMsg => "OnNewVoteResult";

        public static string UserJoinMsg => "user-joined";

        public static string GetMessagesMsg => "get-messages";

        public static string GetFocusScreenMsg => "get-focusScreenList";
        public static string GetFocusMsg => "get-focusList";
        public static string GetAvaMsg => "get-showAvaList";

        #endregion

        IMapper mapper;
        IHubContext<GroupHub> groupHub;
        IHubContext<DrawHub> drawHubContext;
        IRepoWrapper repos;

        public MeetingHub(IRepoWrapper repos, IHubContext<GroupHub> groupHubContext, IMapper mapper, IHubContext<DrawHub> drawHubContext)
        {
            this.repos = repos;
            groupHub = groupHubContext;
            this.mapper = mapper;
            this.drawHubContext = drawHubContext;
        }
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("\n\n===========================\nOnConnectedAsync");
            //Step 1: Lấy meeting Id và username
            HttpContext httpContext = Context.GetHttpContext();
            string meetingIdString = httpContext.Request.Query["meetingId"].ToString();
            int meetingIdInt = int.Parse(meetingIdString);

            string isTempConnection = httpContext.Request.Query["tempConnection"].ToString();
            if (isTempConnection != null && isTempConnection.Length != 0 && isTempConnection == "ok")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, meetingIdString);
                base.OnConnectedAsync();
                return;
            }

            string username;
            int accountId;
            try
            {
                username = Context.User.GetUsername();
                accountId = Context.User.GetUserId();
            }
            catch
            {
                username = httpContext.Request.Query["username"].ToString();
                string acoountIdString = httpContext.Request.Query["accountId"].ToString();
                accountId = int.Parse(acoountIdString);
            }
            //Step 2: Add ContextConnection vào MeetingHub.Group(meetingId) và add (user, meeting) vào presenceTracker

            await Groups.AddToGroupAsync(Context.ConnectionId, meetingIdString);//khi user click vao room se join vao
                                                                                //await AddConnectionToGroup(meetingIdInt); // luu db DbSet<Connection> de khi disconnect biet

            //Step 3: Tạo Connect để lưu vào DB, ConnectionId
            #region lưu Db Connection
            Meeting meeting = await repos.Meetings.GetMeetingByIdSignalr(meetingIdInt);
            Connection connection = new Connection
            {
                SinganlrId = Context.ConnectionId,
                AccountId = accountId,
                MeetingId = meetingIdInt,
                UserName = username,
                Start = DateTime.Now
            };
            if (meeting != null)
            {
                await repos.Connections.CreateConnectionSignalrAsync(connection);
                if (meeting.Start == null)
                {
                    meeting.Start = DateTime.Now;
                }
                meeting.CountMember = await repos.Connections.CountMemberInMeeting(meetingIdInt);
                await repos.Meetings.UpdateAsync(meeting);
            }
            Console.WriteLine("++==++==++++++++++++++++");
            #endregion

            var usersInMeeting = repos.Connections.GetList()
                .Where(e => e.MeetingId == meetingIdInt && e.End == null)
                .Select(e => e.UserName).ToHashSet();

            Console.WriteLine("2.1     " + new String('+', 50));
            Console.WriteLine("2.1     Hub/ChatSend: UserOnlineInGroupMsg, MemberSignalrDto");

            // Step 6: Thông báo với groupHub.Group(groupId) số người ở trong phòng  
            Console.WriteLine("2.1     " + new String('+', 50));
            Console.WriteLine("2.1     Hub/PresenceSend: CountMemberInGroupMsg, { meetingId, countMember }");
            await groupHub.Clients.Group(meeting.Schedule.GroupId.ToString()).SendAsync(GroupHub.CountMemberInGroupMsg,
                   new { meetingId = meetingIdInt, countMember = meeting.CountMember });

            //Code xử lí db xóa duplicate connection
            IQueryable<Connection> dupCons = repos.Connections.GetList()
                .Where(c => c.SinganlrId != Context.ConnectionId && c.AccountId == Context.User.GetUserId() && c.End == null);
            int dupCount = dupCons.Count();
            foreach (var con in dupCons)
            {
                con.End = DateTime.Now;
                await repos.Connections.UpdateAsync(con);
            }
            await groupHub.Clients.Group(meeting.Schedule.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
            await groupHub.Clients.Group(meeting.Schedule.GroupId.ToString()).SendAsync(GroupHub.OnReloadSelfMeetingMsg);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //step 1: Lấy username 
            string username = Context.User.GetUsername();
            //step 2: Xóa connection trong db và lấy meeting
            Meeting meeting = await RemoveConnectionFromMeeting();

            //step 5: Remove ContextConnectionId khỏi meetingHub.Group(meetingId)   chắc move ra khỏi if
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, meeting.Id.ToString());
            int onlineCount = await repos.Connections.CountMemberInMeeting(meeting.Id);
            await repos.Meetings.UpdateCountMemberSignalr(meeting.Id, onlineCount);
            await groupHub.Clients.All.SendAsync(GroupHub.CountMemberInGroupMsg,
                   new { meetingId = meeting.Id, countMember = onlineCount });

            var usersInMeeting = repos.Connections.GetList()
               .Where(e => e.MeetingId == meeting.Id && e.End == null)
               .Select(e => e.UserName).ToHashSet();

            if(usersInMeeting.Count == 0)
            {
                var usersJoined = repos.Connections.GetList()
                  .Where(e => e.MeetingId == meeting.Id)
                  .Select(e => e.UserName).ToHashSet();
                meeting.CountMember = usersJoined.Count;
                meeting.End = DateTime.Now;
                await repos.Meetings.UpdateAsync(meeting);
            }
            try
            {
                Connection connection = await repos.Connections.GetList().SingleOrDefaultAsync(e => e.SinganlrId == Context.ConnectionId);
                if (connection == null)
                {
                    Console.WriteLine("\n\n+++++++++++++\nEnd connection fail");
                }
                else
                {
                    connection.End = DateTime.Now;
                    await repos.Connections.UpdateAsync(connection);
                    //Hot fix duplicate connection
                    var dupConnections = repos.Connections.GetList()
                    .Where(e => e.AccountId == connection.AccountId && e.MeetingId == connection.MeetingId
                        && e.Start.Date == connection.Start.Date && e.Start.Hour == connection.Start.Hour
                        && e.Start.Minute == connection.Start.Minute);
                    foreach (var dupCon in dupConnections)
                    {
                        dupCon.End= DateTime.Now;
                        await repos.Connections.UpdateAsync(dupCon);
                    }
                }
            }
            catch
            {
                Console.WriteLine("\n\n+++++++++++++\nEnd connection fail");
            }
            await groupHub.Clients.Group(meeting.Schedule.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
            await groupHub.Clients.Group(meeting.Schedule.GroupId.ToString()).SendAsync(GroupHub.OnReloadSelfMeetingMsg);

            //step 9: Disconnect khỏi meetHub
            await base.OnDisconnectedAsync(exception);
        }
        //sẽ dc gọi khi FE gọi meetingHubConnection.invoke('SendMessage', { content: string })
        public async Task SendMessageOld(MessageSignalrCreateDto createMessageDto)
        {
            string userName = Context.User.GetUsername();
            Account sender = await repos.Accounts.GetUserByUsernameSignalrAsync(userName);

            Meeting meeting = await repos.Meetings.GetMeetingForConnectionSignalr(Context.ConnectionId);

            if (meeting != null)
            {
                MessageSignalrGetDto message = new MessageSignalrGetDto
                {
                    SenderUsername = userName,
                    SenderDisplayName = sender.Username,
                    Content = createMessageDto.Content,
                    MessageSent = DateTime.Now
                };
                //Luu message vao db
                Chat newChat = new Chat 
                {
                     Content=message.Content,
                     AccountId=Context.User.GetUserId(),
                     MeetingId=meeting.Id,
                     Time = message.MessageSent
                };
                await repos.Chats.CreateAsync(newChat);
                //code here
                //send meaasge to group
                await Clients.Group(meeting.Id.ToString()).SendAsync(NewMessageMsg, message);
            }
        }

        //sẽ dc gọi khi FE gọi meetingHubConnection.invoke('MuteMicro', mute)
        //public async Task MuteMicro(bool muteMicro)
        //{
        //    Meeting meeting = await repos.Meetings.GetMeetingForConnectionSignalr(Context.ConnectionId);
        //    if (meeting != null)
        //    {
        //        await Clients.Group(meeting.Id.ToString()).SendAsync(OnMuteMicroMsg, new { username = Context.User.GetUsername(), mute = muteMicro });
        //    }
        //    else
        //    {
        //        throw new HubException("group == null");
        //    }
        //}
        //sẽ dc gọi khi có người xin dc vote (review)
        //sẽ dc gọi khi FE gọi meetingHubConnection.invoke('StartVote', meetingId: int)
        public async Task StartVote(int meetingId)
        {
            string reviewee = Context.User.GetUsername();
            await Clients.Group(meetingId.ToString()).SendAsync(OnStartVoteMsg, reviewee);
        }

        //sẽ dc gọi khi có người xin dc vote (review)
        //sẽ dc gọi khi FE gọi meetingHubConnection.invoke('StartVote', reviewee: username)
        public async Task EndVote(int meetingId)
        {
            int revieweeId = Context.User.GetUserId();
            Review newReview = new Review
            {
                MeetingId = meetingId,
                RevieweeId = revieweeId
            };
            await repos.Reviews.CreateAsync(newReview);
            ReviewSignalrDTO mapped = mapper.Map<ReviewSignalrDTO>(newReview);
            mapped.RevieweeUsername = Context.User.GetUsername();
            await Clients.Group(meetingId.ToString()).SendAsync(OnEndVoteMsg, mapped);

            var newMeetReviews = repos.Reviews.GetList()
                            .Where(e => e.MeetingId == meetingId)
                            .Include(e => e.Reviewee)
                            .Include(e => e.Details).ThenInclude(e => e.Reviewer);
            List<ReviewSignalrDTO> mappedNewReviews = newMeetReviews
                .ProjectTo<ReviewSignalrDTO>(mapper.ConfigurationProvider).ToList();
            //await Clients.Group(meetingId.ToString()).SendAsync(OnReloadVoteMsg, mappedNewReviews);

        }

        //sẽ dc gọi khi có người xin dc vote (review)
        //sẽ dc gọi khi FE gọi meetingHubConnection.invoke('VoteForReview', reviewDetail: ReviewDetailSignalrCreateDto)
        public async Task VoteForReview(int meetingId)
        {
            Console.WriteLine("2.   " + meetingId);
            await Clients.Group(meetingId.ToString()).SendAsync(OnVoteChangeMsg, "new");
        }
        private async Task<Meeting> RemoveConnectionFromMeeting()
        {
            Meeting meeting = await repos.Meetings.GetMeetingForConnectionSignalr(Context.ConnectionId);
            if(meeting == null)
            {
                int id = int.Parse(Context.GetHttpContext().Request.Query["meetingId"].ToString());
                meeting = repos.Meetings.GetList()
                    .Include(m=>m.Connections)
                    .SingleOrDefault(e => e.Id == id);
            }
            Connection? connection = repos.Connections.GetList()
                .SingleOrDefault(x => x.SinganlrId == Context.ConnectionId);
            if (connection != null) { 
                await repos.Meetings.EndConnectionSignalr(connection);
            }
            
            //hot fix duplicate connection
            var dupConnections = repos.Connections.GetList()
                .Where(e => e.AccountId == connection.AccountId && e.MeetingId == connection.MeetingId
                    && e.Start.Date == connection.Start.Date && e.Start.Hour == connection.Start.Hour
                    && e.Start.Minute == connection.Start.Minute && e.SinganlrId != connection.SinganlrId);
            foreach(var dupCon in dupConnections)
            {
                dupCon.End = DateTime.Now;
                await repos.Connections.UpdateAsync(dupCon);
            }

            IQueryable<Connection> activeConnections = repos.Meetings.GetActiveConnectionsForMeetingSignalr(meeting.Id);
            if (activeConnections.Count() == 0)
            {
                await repos.Meetings.EndMeetingSignalRAsync(meeting);
            }
            return meeting;
        }

        //public static readonly Dictionary<string, List<string>> Rooms = new Dictionary<string, List<string>>();
        public class IMessage
        {
            public string RoomId { get; set; }
            public string Content { get; set; }
            public string TimeStamp { get; set; }
            public string Username { get; set; }
            public string Author { get; set; }
        }
        //public static readonly Dictionary<string, Dictionary<string, Peer>> Rooms = new Dictionary<string, Dictionary<string, Peer>>();
        public static readonly Dictionary<string, List<IMessage>> Chats = new Dictionary<string, List<IMessage>> ();
        public static readonly Dictionary<string, List<FocusItem>> FocusMap = new Dictionary<string, List<FocusItem>> ();
        public static readonly Dictionary<string, List<ShowAvaInput>> AvaMap = new Dictionary<string, List<ShowAvaInput>> ();
        public class CreateRoomInput
        {
            public string peerId { get; set; }
            public string username { get; set; }

            //public string roomId { get; set; } = "default";
            public string roomId { get; set; } = Guid.NewGuid().ToString();
        }

        public class JoinRoomInput
        {
            public string roomId { get; set; }
            public string username { get; set; }
            public string peerId { get; set; }
        }
        public class Peer
        {
            public string peerId { get; set; }
            public string userName { get; set; }
        }
        public async Task JoinRoom(JoinRoomInput input)
        {

            //var input = JsonConvert.DeserializeObject<JoinRoomInput>(json);
            string roomId = input.roomId;
            string peerId = input.peerId;
            string username = input.username;
            Connection connection = await repos.Connections.GetBySignalrIdAsync(Context.ConnectionId);
            connection.PeerId = peerId;
            repos.Connections.UpdateAsync(connection);

            Console.WriteLine($"\n\n==++==++===+++\n JoinRoom");
            Console.WriteLine(peerId);
            Console.WriteLine(roomId);
            Console.WriteLine(username);
            bool isChatExisted = Chats.ContainsKey(roomId);
            if (!isChatExisted)
            {
                Chats.Add(roomId, new List<IMessage>());
            }
            bool isFocusListExisted = FocusMap.ContainsKey(roomId);
            if (!isFocusListExisted)
            {
                FocusMap.Add(roomId, new List<FocusItem>());
            }
            bool isShowCamListExisted = AvaMap.ContainsKey(roomId);
            if (!isShowCamListExisted)
            {
                AvaMap.Add(roomId, new List<ShowAvaInput>());
            }
            Peer peer = new Peer
            {
                peerId = peerId,
                userName = username,
            };
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            //Help stablize fe
            System.Threading.Thread.Sleep(1000);
            await Clients.Group(roomId).SendAsync(GetAvaMsg, AvaMap[roomId]);
            //await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync(UserJoinMsg, peer);
            await Clients.Group(roomId).SendAsync(UserJoinMsg, peer);
            //await Clients.Group(roomId).SendAsync(UserJoinMsg, peer);
            await Clients.Group(roomId).SendAsync(GetMessagesMsg, Chats[roomId]);
            await SendFocusList(roomId);
            //await Clients.Group(roomId).SendAsync(GetFocusMsg, FocusMap[roomId]);
        }

        public class FocusInput
        {
            public string roomId { get; set; }
            public string peerId { get; set; }
            public string action { get; set; }
        }
        public class FocusItem
        {
            public FocusItem(FocusInput input)
            {
                roomId = input.roomId;
                peerId = input.peerId;
                actions = new List<string> { input.action };
            }
            public string roomId { get; set; }
            public string peerId { get; set; }
            public List<string> actions { get; set; }
        }

        public class ShowAvaInput
        {
            public string roomId { get; set; }
            public string peerId { get; set; }
            public string imagePath { get; set; }
        }

        public async Task StartFocus(FocusInput input)
        {
            string roomId = input.roomId;
            string peerId = input.peerId;
            string action = input.action;
            if (FocusMap.ContainsKey(roomId))
            {
                List<FocusItem> focusList = FocusMap[roomId];
                FocusItem focus = focusList.FirstOrDefault(e => e.peerId == peerId);
                if (focus != null)
                {
                    focusList.Remove(focus);
                    focus.actions.Add(action);
                    focusList.Add(focus);
                }
                else
                {
                    focusList.Add(new FocusItem(input));
                }
            }
            else
            {
                FocusMap.Add(roomId, new List<FocusItem>() { new FocusItem(input) });
            }
            await SendFocusList(input.roomId);
        }

        private async Task SendFocusList(string roomId)
        {
            List<FocusItem> focusList = new List<FocusItem>();
            FocusItem last = FocusMap[roomId].LastOrDefault(); 
            if   (last != null)
            {
                focusList.Add(last);
            }
            await Clients.Groups(roomId).SendAsync(GetFocusScreenMsg, focusList);
            await Clients.Groups(roomId).SendAsync(GetFocusMsg, FocusMap[roomId]);
        }

        public async Task EndFocus(FocusInput input)
        {
            string roomId = input.roomId;
            string peerId = input.peerId;
            string action = input.action;
            if (FocusMap.ContainsKey(roomId))
            {
                List<FocusItem> focusList = FocusMap[roomId];
                FocusItem focus = focusList.FirstOrDefault(e => e.peerId == peerId);
                if (focus != null)
                {
                    focus.actions.Remove(action);
                    if (focus.actions.Count == 0)
                    {
                        focusList.Remove(focus);
                    }
                }
            }
            await SendFocusList(roomId);
            //await Clients.Groups(input.roomId).SendAsync(GetFocusMsg, FocusMap[input.roomId]);
        }
        public async Task StartAva(ShowAvaInput input)
        {
            string roomId = input.roomId;
            string peerId = input.peerId;
            if (AvaMap.ContainsKey(roomId))
            {
                List<ShowAvaInput> showAvaList = AvaMap[roomId];
                ShowAvaInput ava = showAvaList.FirstOrDefault(e => e.peerId == peerId);
                if (ava == null)
                {
                    showAvaList.Add(input);
                }
                else
                {
                    ava.imagePath = input.imagePath;
                }
            }
            else
            {
                AvaMap.Add(roomId, new List<ShowAvaInput>() { input });
            }
            await Clients.Groups(roomId).SendAsync(GetAvaMsg, AvaMap[roomId]);
        }
        public async Task EndAva(ShowAvaInput input)
        {
            string roomId = input.roomId;
            string peerId = input.peerId;
            if (FocusMap.ContainsKey(roomId))
            {
                List<ShowAvaInput> showAvaList = AvaMap[roomId];
                ShowAvaInput ava = showAvaList.FirstOrDefault(e => e.peerId == peerId);
                if (ava != null)
                {
                    showAvaList.Remove(ava);
                }
            }
            await Clients.Groups(roomId).SendAsync(GetAvaMsg, AvaMap[roomId]);
        }
        public class LeaveRoomInput
        {
            public string roomId { get; set; }
            public string peerId { get; set; }
        }
        public async Task LeaveRoom(LeaveRoomInput input)
        {
            Console.WriteLine($"\n\n==++==++===+++\n LeaveRoom");
            Console.WriteLine(input.peerId);
            Console.WriteLine(input.roomId);
            string roomId = input.roomId;
            string peerId = input.peerId;
            string username = Context.User.GetUsername();

            await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync("user-disconnected", peerId, username);

            //code mới xử lí db
            Meeting meeting = await RemoveConnectionFromMeeting();
            var usersInMeeting = repos.Connections.GetList()
               .Where(e => e.MeetingId == meeting.Id && e.End == null)
               .Select(e => e.UserName).ToHashSet();
            //await EndFocus(new FocusInput (){ roomId= roomId, peerId=peerId });
            List<FocusItem> focusList = FocusMap[roomId];
            FocusItem focus = focusList.FirstOrDefault(e => e.peerId == input.peerId);
            focusList.Remove(focus);

            await SendFocusList(input.roomId);
            //await Clients.Groups(input.roomId).SendAsync(GetFocusMsg, FocusMap[input.roomId]);

            List<ShowAvaInput> camList = AvaMap[roomId];
            ShowAvaInput cam = camList.FirstOrDefault(e => e.peerId == input.peerId);
            camList.Remove(cam);
            await Clients.Groups(input.roomId).SendAsync(GetAvaMsg, AvaMap[input.roomId]);

            //await Clients.Group(meeting.Id.ToString()).SendAsync(UserOnlineInMeetingMsg, usersInMeeting);
            if (usersInMeeting.Count == 0)
            {
                var usersJoined = repos.Connections.GetList()
                  .Where(e => e.MeetingId == meeting.Id)
                  .Select(e => e.UserName).ToHashSet();
                meeting.CountMember = usersJoined.Count;
                meeting.End = DateTime.Now;
                await repos.Meetings.UpdateAsync(meeting);
                Chats.Remove(roomId);
                FocusMap.Remove(roomId);
                AvaMap.Remove(roomId);
                DrawHub.Drawings.Remove(roomId);
                await drawHubContext.Clients.Group(roomId).SendAsync(DrawHub.MeetingEndMsg);
            }
            else
            {
                meeting.CountMember = usersInMeeting.Count;
                await repos.Meetings.UpdateAsync(meeting);
            }

            Connection connection = await repos.Connections.GetList().SingleOrDefaultAsync(e => e.SinganlrId == Context.ConnectionId);
            if (connection == null)
            {
                Console.WriteLine("\n\n+++++++++++++\nEnd connection fail");
            }
            else
            {
                connection.End = DateTime.Now;
                await repos.Connections.UpdateAsync(connection);
            }
            await groupHub.Clients.Group(meeting.Schedule.GroupId.ToString()).SendAsync(GroupHub.OnReloadGroupMsg);
            await groupHub.Clients.Group(meeting.Schedule.GroupId.ToString()).SendAsync(GroupHub.OnReloadSelfMeetingMsg);

        }


        public async Task SendMessage(IMessage message)
        {
            HttpContext httpContext = Context.GetHttpContext();
            int meetingId = int.Parse(httpContext.Request.Query["meetingId"].ToString());
            string roomId = message.RoomId;
            Chats[roomId].Add(message);
            Clients.Group(roomId).SendAsync("add-message", message);

            //xử lí db
            Chat newChat = new Chat {
                Content= message.Content,
                MeetingId = meetingId,
                AccountId = Context.User.GetUserId(),
                Time = DateTime.Now,
            };
            await repos.Chats.CreateAsync(newChat);
        }
        public async Task LeaderEndMeeting()
        {
            HttpContext httpContext = Context.GetHttpContext();
            int meetingId = int.Parse(httpContext.Request.Query["meetingId"].ToString());
            await Clients.Group(meetingId.ToString()).SendAsync("LeaderEndMeeting", "Nhóm trưởng đã kết thúc cuộc họp");
        }

    }
}
