using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DbObject;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Db;

namespace ServiceLayer.Services.Implementation.Db
{
    public class StatService : IStatService
    {
        private IRepoWrapper repos;
        private readonly IMapper mapper;

        public StatService(IRepoWrapper repos, IMapper mapper)
        {
            this.repos = repos;
            this.mapper = mapper;
        }

        public async Task<StatGetDto> GetStatForAccountInMonth(int accountId, DateTime month)
        {
            DateTime start = new DateTime(month.Year, month.Month, 1, 0, 0, 0).Date;
            DateTime end = start.AddMonths(1);

            Account student = await repos.Accounts.GetByIdAsync(accountId);

            //Nếu tháng này thì chỉ lấy past meeting
            IQueryable<Meeting> allMeetingsOfJoinedGroups = month.Month == DateTime.Now.Month
                ? repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Reviews).ThenInclude(r => r.Details)
                .Where(e => (e.ScheduleStart >= start && e.ScheduleStart.Value.Date < end || e.Start >= start && e.Start.Value.Date < end)
                    && e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == accountId)
                    //lấy past meeting
                    && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                : repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Reviews).ThenInclude(r => r.Details)
                .Where(c => c.ScheduleStart >= start && c.ScheduleStart.Value.Date < end
                    && c.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == accountId));
            //int totalMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
            //    ? 0 : allMeetingsOfJoinedGroups.Count();
            int totalMeetingsCount = allMeetingsOfJoinedGroups.Count();
            IQueryable<Meeting> atendedMeetings = allMeetingsOfJoinedGroups
                .Where(e => e.Connections.Any(c => c.AccountId == accountId));
            //int atendedMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
            //    ? 0 : allMeetingsOfJoinedGroups
            //    .Where(e => e.Connections.Any(c => c.AccountId == studentId)).Count();
            int atendedMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
                ? 0 : atendedMeetings.Count();
            long totalMeetingTime = atendedMeetings.Count() == 0 ? 0
                : atendedMeetings.SelectMany(m => m.Connections)
                    .Select(e => e.End.Value - e.Start).Select(ts => ts.Ticks).ToList().Sum();
            TimeSpan timeSpan = new TimeSpan(totalMeetingTime);
            IQueryable<ReviewDetail> reviewDetails = atendedMeetings
                .SelectMany(m => m.Reviews)
                .SelectMany(r => r.Details);
            var averageVoteResult = !reviewDetails.Any() ? 0
                : await reviewDetails.Select(e => (int)e.Result).AverageAsync();
            //var totalMeetingTime = allMeetizngsOfJoinedGroups.SelectMany(m => m.Connections);//.Select(e=>e.End.Value-e.Start).Select(ts=>ts.Ticks).Sum(); 

            IQueryable<Discussion> discussions = repos.Discussions.GetList()
                .Include(x => x.AnswerDiscussion)
                .Where(x => x.AccountId == accountId
                        && (x.CreateAt >= start && x.CreateAt < end));

            IQueryable<AnswerDiscussion> answerdiscussions = repos.AnswerDiscussions.GetList()
                .Include(x => x.Discussion)
                .Where(x => x.AccountId == accountId
                        && (x.CreateAt >= start && x.CreateAt < end));

            return new StatGetDto
            {
                StudentFullname = student.FullName,
                StudentUsername = student.Username,
                Month = start,
                //TotalMeetings = allMeetingsOfJoinedGroups.ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider),
                TotalMeetingsCount = totalMeetingsCount,
                AtendedMeetings = atendedMeetings.ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider),
                AtendedMeetingsCount = atendedMeetingsCount,
                MissedMeetingsCount = totalMeetingsCount - atendedMeetingsCount,
                TotalMeetingTme = totalMeetingTime == 0 ? "Chưa tham gia buổi học nào"
                    : $"{timeSpan.Hours} giờ {timeSpan.Minutes} phút {timeSpan.Seconds} giây",
                AverageVoteResult = averageVoteResult,
                TotalDiscussionsCount = discussions.Count(),
                TotalAnswerDiscussionsCount = answerdiscussions.Count()
            };
        }

        public async Task<StatGetDto> GetStatForGroupInMonth(int groupId, DateTime month)
        {
            DateTime start = new DateTime(month.Year, month.Month, 1, 0, 0, 0).Date;
            DateTime end = start.AddMonths(1);

            //Nếu tháng này thì chỉ lấy past meeting
            IQueryable<Meeting> allMeetingsOfJoinedGroups = month.Month == DateTime.Now.Month
                ? repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Reviews).ThenInclude(r => r.Details)
                .Where(e => (e.ScheduleStart >= start && e.ScheduleStart.Value.Date < end || e.Start >= start && e.Start.Value.Date < end)
                    && e.Schedule.Group.Id == groupId
                    //lấy past meeting
                    && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                : repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Reviews).ThenInclude(r => r.Details)
                .Where(c => c.ScheduleStart >= start && c.ScheduleStart.Value.Date < end
                    && c.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == groupId));
            //int totalMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
            //    ? 0 : allMeetingsOfJoinedGroups.Count();
            int totalMeetingsCount = allMeetingsOfJoinedGroups.Count();
            IQueryable<Meeting> atendedMeetings = allMeetingsOfJoinedGroups
                .Where(e => e.Connections.Any(c => c.AccountId == groupId));
            //int atendedMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
            //    ? 0 : allMeetingsOfJoinedGroups
            //    .Where(e => e.Connections.Any(c => c.AccountId == studentId)).Count();
            int atendedMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
                ? 0 : atendedMeetings.Count();
            long totalMeetingTime = atendedMeetings.Count() == 0 ? 0
                : atendedMeetings.SelectMany(m => m.Connections)
                    .Select(e => e.End.Value - e.Start).Select(ts => ts.Ticks).ToList().Sum();
            TimeSpan timeSpan = new TimeSpan(totalMeetingTime);
            IQueryable<ReviewDetail> reviewDetails = atendedMeetings
                .SelectMany(m => m.Reviews)
                .SelectMany(r => r.Details);
            var averageVoteResult = !reviewDetails.Any() ? 0
                : await reviewDetails.Select(e => (int)e.Result).AverageAsync();
            //var totalMeetingTime = allMeetizngsOfJoinedGroups.SelectMany(m => m.Connections);//.Select(e=>e.End.Value-e.Start).Select(ts=>ts.Ticks).Sum(); 

            IQueryable<Discussion> discussions = repos.Discussions.GetList()
                .Include(x => x.AnswerDiscussion)
                .Where(x => x.GroupId == groupId
                        && (x.CreateAt >= start && x.CreateAt < end));

            IQueryable<AnswerDiscussion> answerdiscussions = repos.AnswerDiscussions.GetList()
                .Include(x => x.Discussion)
                .Where(x => x.Discussion.GroupId == groupId
                        && (x.CreateAt >= start && x.CreateAt < end));

            return new StatGetDto
            {
                Month = start,
                //TotalMeetings = allMeetingsOfJoinedGroups.ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider),
                TotalMeetingsCount = totalMeetingsCount,
                AtendedMeetings = atendedMeetings.ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider),
                AtendedMeetingsCount = atendedMeetingsCount,
                MissedMeetingsCount = totalMeetingsCount - atendedMeetingsCount,
                TotalMeetingTme = totalMeetingTime == 0 ? "Chưa tham gia buổi học nào"
                    : $"{timeSpan.Hours} giờ {timeSpan.Minutes} phút {timeSpan.Seconds} giây",
                AverageVoteResult = averageVoteResult,
                TotalDiscussionsCount = discussions.Count(),
                TotalAnswerDiscussionsCount = answerdiscussions.Count()
            };
        }

        public async Task<IList<StatGetListDto>> GetStatsForStudent(int studentId)
        {
            DateTime month = DateTime.Now;
            List<StatGetListDto> stats = new List<StatGetListDto>();
            List<DateTime> startDates = new List<DateTime>();
            DateTime start1 = new DateTime(month.Year, month.Month, 1, 0, 0, 0).Date;
            startDates.Add(start1);

            Account student = await repos.Accounts.GetByIdAsync(studentId);
            for (int i = 1; i <= 4; i++)
            {
                startDates.Add(start1.AddMonths(-i));
            }

            IQueryable<Meeting> allMeetingsOfStudent = repos.Meetings.GetList()
                    //.Include(m => m.Chats).ThenInclude(c => c.Account)
                    .Include(c => c.Connections)
                    .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                    .Include(m => m.Reviews).ThenInclude(r => r.Details)
                    .Where(e =>
                        //(e.ScheduleStart >= start && e.ScheduleStart.Value.Date < end || e.Start >= start && e.Start.Value.Date < end)
                        e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId)
                        //lấy past meeting
                        && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                    .AsSplitQuery().ToList().AsQueryable();
            IQueryable<Discussion> discussions = repos.Discussions.GetList()
                //.Include(x => x.AnswerDiscussion)
                .Where(x => x.AccountId == studentId).ToList().AsQueryable();
            IQueryable<AnswerDiscussion> answerdiscussions = repos.AnswerDiscussions.GetList()
                //.Include(x => x.Discussion)
                .Where(x => x.AccountId == studentId).ToList().AsQueryable();

            foreach (DateTime start in startDates)
            {
                DateTime end = start.AddMonths(1);
                //Nếu tháng này thì chỉ lấy past meeting
                IQueryable<Meeting> allMeetingsOfJoinedGroups = start.Month == DateTime.Now.Month
                    ? allMeetingsOfStudent
                    .Where(e => (e.ScheduleStart >= start && e.ScheduleStart.Value.Date < end || e.Start >= start && e.Start.Value.Date < end)
                        //lấy past meeting
                        && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                    .AsQueryable()
                    : allMeetingsOfStudent
                    .Where(c => c.ScheduleStart >= start && c.ScheduleStart.Value.Date < end)
                    .AsQueryable();
                int totalMeetingsCount = allMeetingsOfJoinedGroups.Count();
                IQueryable<Meeting> atendedMeetings = allMeetingsOfJoinedGroups
                    .Where(e => e.Connections.Any(c => c.AccountId == studentId));
                int atendedMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
                    ? 0 : atendedMeetings.Count();
                long totalMeetingTime = atendedMeetings.Count() == 0 ? 0
                    : atendedMeetings.SelectMany(m => m.Connections)
                        .Select(e => e.End.Value - e.Start).Select(ts => ts.Ticks).ToList().Sum();
                TimeSpan timeSpan = new TimeSpan(totalMeetingTime);
                IQueryable<ReviewDetail> reviewDetails = atendedMeetings
                    .SelectMany(m => m.Reviews)
                    .SelectMany(r => r.Details);
                var averageVoteResult = !reviewDetails.Any() ? 0
                    : reviewDetails.Select(e => (int)e.Result).AsEnumerable().Average();

                IQueryable<Discussion> filteredDiscussions = discussions
               .Where(x => (x.CreateAt >= start && x.CreateAt < end));

                //IQueryable<AnswerDiscussion> filteredAnswerdiscussions = repos.AnswerDiscussions.GetList()
                //    .Include(x => x.Discussion)
                //    .Where(x => x.AccountId == studentId
                //            && (x.CreateAt >= start && x.CreateAt < end));
                IQueryable<AnswerDiscussion> filteredAnswerdiscussions = answerdiscussions
                    .Where(x => x.CreateAt >= start && x.CreateAt < end);

                StatGetListDto newStat = new StatGetListDto
                {
                    StudentFullname = student.FullName,
                    StudentUsername = student.Username,
                    Month = start,
                    TotalMeetingsCount = totalMeetingsCount,
                    AtendedMeetingsCount = atendedMeetingsCount,
                    MissedMeetingsCount = totalMeetingsCount - atendedMeetingsCount,
                    //TotalMeetingTme = totalMeetingTime == 0 ? "Chưa tham gia buổi học nào"
                    //    : $"{timeSpan.Hours} giờ {timeSpan.Minutes} phút {timeSpan.Seconds} giây",
                    TotalMeetingTme = timeSpan,
                    AverageVoteResult = averageVoteResult,
                    TotalDiscussionCount = filteredDiscussions.Count(),
                    ToTalAnswerDiscussionCount = filteredAnswerdiscussions.Count(),
                };
                stats.Add(newStat);
            }
            return stats;
        }

        public async Task<IList<StatGetListDto>> GetStatsForGroup(int groupId)
        {
            DateTime month = DateTime.Now;
            List<StatGetListDto> stats = new List<StatGetListDto>();
            List<DateTime> startDates = new List<DateTime>();
            DateTime start1 = new DateTime(month.Year, month.Month, 1, 0, 0, 0).Date;
            startDates.Add(start1);
            for (int i = 1; i <= 4; i++)
            {
                startDates.Add(start1.AddMonths(-i));
            }
            IQueryable<Meeting> allMeetingsOfJoinedGroupsAllTime = repos.Meetings.GetList()
                    //.Include(m => m.Chats).ThenInclude(c => c.Account)
                    .Include(c => c.Connections)
                    .Include(a => a.Schedule)//.ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                    .Include(m => m.Reviews).ThenInclude(r => r.Details)
                    .Where(e => (e.Schedule.GroupId == groupId))
                    .AsSplitQuery().ToList().AsQueryable();
            IQueryable<Discussion> discussions = repos.Discussions.GetList()
                //.Include(x => x.AnswerDiscussion)
                .Where(x => x.GroupId == groupId).ToList().AsQueryable();
            IQueryable<AnswerDiscussion> answerdiscussions = repos.AnswerDiscussions.GetList()
                .Include(x => x.Discussion)
                .Where(x => x.Discussion.GroupId == groupId).ToList().AsQueryable();
            foreach (DateTime start in startDates)
            {
                DateTime end = start.AddMonths(1);
                //Nếu tháng này thì chỉ lấy past meeting
                IQueryable<Meeting> allMeetingsOfJoinedGroups = start.Month == DateTime.Now.Month
                    ? allMeetingsOfJoinedGroupsAllTime
                    .Where(e => (e.ScheduleStart >= start && e.ScheduleStart.Value.Date < end || e.Start >= start && e.Start.Value.Date < end)
                        //lấy past meeting
                        && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                    : allMeetingsOfJoinedGroupsAllTime
                    .Where(c => c.ScheduleStart >= start && c.ScheduleStart.Value.Date < end);
                int totalMeetingsCount = allMeetingsOfJoinedGroups.Count();
                IQueryable<Meeting> atendedMeetings = allMeetingsOfJoinedGroups;

                int atendedMeetingsCount = allMeetingsOfJoinedGroups.Count() == 0
                    ? 0 : atendedMeetings.Count();
                long totalMeetingTime = atendedMeetings.Count() == 0 ? 0
                    : atendedMeetings.SelectMany(m => m.Connections)
                        .Select(e => e.End.Value - e.Start).Select(ts => ts.Ticks).ToList().Sum();
                TimeSpan timeSpan = new TimeSpan(totalMeetingTime);
                IQueryable<ReviewDetail> reviewDetails = atendedMeetings
                    .SelectMany(m => m.Reviews)
                    .SelectMany(r => r.Details);
                var averageVoteResult = !reviewDetails.Any() ? 0
                    : reviewDetails.Select(e => (int)e.Result).AsEnumerable().Average();

                IQueryable<Discussion> filteredDiscussions = discussions
                    .Where(x => x.GroupId == groupId
                            && (x.CreateAt >= start && x.CreateAt < end));

                IQueryable<AnswerDiscussion> filteredAnswerdiscussions = answerdiscussions
                    .Where(x => x.Discussion.GroupId == groupId
                            && (x.CreateAt >= start && x.CreateAt < end));

                StatGetListDto newStat = new StatGetListDto
                {
                    Month = start,
                    TotalMeetingsCount = totalMeetingsCount,
                    AtendedMeetingsCount = atendedMeetingsCount,
                    MissedMeetingsCount = totalMeetingsCount - atendedMeetingsCount,
                    //TotalMeetingTme = totalMeetingTime == 0 ? "Chưa tham gia buổi học nào"
                    //    : $"{timeSpan.Hours} giờ {timeSpan.Minutes} phút {timeSpan.Seconds} giây",
                    TotalMeetingTme = timeSpan,
                    AverageVoteResult = averageVoteResult,
                    TotalDiscussionCount = filteredDiscussions.Count(),
                    ToTalAnswerDiscussionCount = filteredAnswerdiscussions.Count(),

                };
                stats.Add(newStat);
            }
            return stats;

        }
    }
}
