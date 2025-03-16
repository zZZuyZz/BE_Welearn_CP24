using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DbObject;
using DataLayer.Enums;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RepoLayer.Interface;
using ServiceLayer.DTOs;
using ServiceLayer.Services.Interface.Db;
using ServiceLayer.Utils;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ServiceLayer.Services.Implementation.Db
{
    internal class MeetingService : IMeetingService
    {
        private IRepoWrapper repos;
        private IMapper mapper;
        private readonly IConfiguration _config;

        public MeetingService(IRepoWrapper repos, IMapper mapper, IConfiguration config)
        {
            this.repos = repos;
            this.mapper = mapper;
            _config = config;
        }

        public async Task<bool> AnyAsync(int id)
        {
            return await repos.Meetings.GetList().AnyAsync(e => e.Id == id);
        }

        public async Task<T> CreateInstantMeetingAsync<T>(InstantMeetingCreateDto dto)
        {
            Schedule schedule = new Schedule()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Name = dto.Name,
                GroupId = dto.GroupId,
                StartTime = DateTime.Now.TimeOfDay,
                DaysOfWeek = "Chủ Nhật, Thứ Hai,Thứ Ba,Thứ Tư,Thứ Năm,Thứ Sáu,Thứ Bảy"
            };

            Meeting meeting = mapper.Map<Meeting>(dto);
            meeting.Schedule = schedule;
            await repos.Meetings.CreateAsync(meeting);
            //await repos.Schedules.CreateAsync(schedule);
            var mapped = mapper.Map<T>(meeting);
            return mapped;
        }

        public async Task CreateScheduleMeetingAsync(ScheduleMeetingCreateDto dto)
        {
            Schedule schedule = new Schedule()
            {
                StartDate = dto.Date,
                EndDate = dto.Date,
                Name = dto.Name,
                GroupId = dto.GroupId,
                StartTime = dto.ScheduleStartTime,
                EndTime = dto.ScheduleEndTime,
            };
            schedule.ScheduleSubjects = dto.SubjectIds.Select(sId => new ScheduleSubject() { SubjectId = (int)sId }).ToList();
            Meeting meeting = mapper.Map<Meeting>(dto);
            meeting.Schedule = schedule;
            //Meeting meeting = new Meeting { 
            //    Name = dto.Name,
            //    Content = dto.Content,
            //    Schedule = schedule,
            //    ScheduleStart = dto.Date.Add()
            //};
            await repos.Meetings.CreateAsync(meeting);
        }

        public async Task<T> MassCreateScheduleMeetingAsync<T>(ScheduleMeetingMassCreateDto dto)
        {
            DateTime[] dates = Enumerable.Range(0, 1 + dto.ScheduleRangeEnd.Subtract(dto.ScheduleRangeStart).Days)
                .Select(offset => dto.ScheduleRangeStart.AddDays(offset))
                .Where(date => dto.DayOfWeeks.Contains(date.DayOfWeek + 1))
                .ToArray();
            List<Meeting> creatingMeetings = dates.Select(date => new Meeting
            {
                Name = dto.Name + " " + date.ToString("d/M"),
                Content = dto.Content,
                ScheduleStart = date.Add(dto.ScheduleStartTime),
                ScheduleEnd = date.Add(dto.ScheduleEndTime),
            }).ToList();
            string daysOfWeek = daysOfWeekToString(dto.DayOfWeeks);
            Console.WriteLine($"+++===+++===+++===+++===+++===+++===\n{daysOfWeek}");
            Schedule schedule = new Schedule
            {
                GroupId = dto.GroupId,
                Name = dto.Name,
                DaysOfWeek = daysOfWeek,
                StartDate = dto.ScheduleRangeStart,
                EndDate = dto.ScheduleRangeEnd,
                StartTime = dto.ScheduleStartTime,
                EndTime = dto.ScheduleEndTime,
                Meetings = creatingMeetings,
                IsActive = true
            };
            schedule.ScheduleSubjects = dto.SubjectIds.Select(sId => new ScheduleSubject() { SubjectId = (int)sId }).ToList();
            //return await repos.Meetings.MassCreateAsync(creatingMeetings);
            await repos.Schedules.CreateAsync(schedule);
            //return creatingMeetings;
            var mapped = mapper.Map<T>(schedule);
            return mapped;
            string daysOfWeekToString(ICollection<DayOfWeek> daysOfWeek)
            {
                //var sorted = daysOfWeek.Select(dayInt => (int)dayInt == 1 ? 8 : (int)dayInt).AsEnumerable().ToImmutableSortedSet();
                var sorted = daysOfWeek.Cast<int>().ToList();
                sorted.Sort();
                if (sorted[0] == 1)
                {
                    sorted.RemoveAt(0);
                    sorted.Add(8);
                }
                string daysOfWeekString = "" + dayOfWeekConvert(sorted[0]);
                for (int i = 1; i < sorted.Count; i++)
                {
                    daysOfWeekString += $", {dayOfWeekConvert(sorted[i])}";
                }
                return daysOfWeekString;

            }
            string dayOfWeekConvert(int intDay)
            {
                switch (intDay)
                {
                    case 1:
                        return "Chủ Nhật";
                    case 2:
                        return "Thứ Hai";
                    case 3:
                        return "Thứ Ba";
                    case 4:
                        return "Thứ Tư";
                    case 5:
                        return "Thứ Năm";
                    case 6:
                        return "Thứ Sáu";
                    case 7:
                        return "Thứ Bảy";
                    case 8:
                        return "Chủ Nhật";
                }
                return "";
            }
        }

        public async Task<T> GetByIdAsync<T>(int id)
        {
            Meeting m = await repos.Meetings.GetByIdAsync(id);
            return mapper.Map<T>(m);
        }
        public async Task<Object> GetAllMeetingsForGroup(int groupId, int studentId)
        {
            bool isLead = await repos.GroupMembers.GetList()
                .AnyAsync(e => e.AccountId == studentId && e.GroupId == groupId
                && e.IsActive == true && e.MemberRole==GroupMemberRole.Leader);
            var allMeeting = repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(m => m.Reviews).ThenInclude(c => c.Reviewee)
                .Include(m => m.Reviews).ThenInclude(c => c.Details).ThenInclude(d=>d.Reviewer)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => e.Schedule.GroupId == groupId)
                .OrderByDescending(e => e.Start.HasValue).ThenBy(e => e.Start)
                .ThenByDescending(e => e.ScheduleStart.HasValue).ThenBy(e => e.ScheduleStart)
                .ToList();//.AsQueryable();
            int countAll = allMeeting.Count();
            var liveMeetings = (allMeeting.Where(e => e.Start != null && e.End == null));
            var liveMapped = mapper.Map<List<LiveMeetingGetDto>>(liveMeetings);
            var pastMeetings = (allMeeting.Where(e => (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                .OrderByDescending(e => e.Start.HasValue).ThenByDescending(e => e.Start)
                .ThenByDescending(e => e.ScheduleStart.HasValue).ThenByDescending(e => e.ScheduleStart)).ToList();
            var pastMapped = mapper.Map<List<PastMeetingGetDto>>(pastMeetings.ToList());
            //var pastMapped = pastMeetings.ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider);
            int countLive = liveMeetings.Count();
            //int countPast = pastMeetings.Count();
            if (isLead)
            {
                var scheduleMeetings = (allMeeting.Where(e => e.ScheduleStart != null
                    && e.ScheduleStart.Value.Date >= DateTime.Today && e.Start == null));
                var scheduleMapped = mapper.Map<List<ScheduleMeetingForLeaderGetDto>>(scheduleMeetings);
                int countSchedule = scheduleMeetings.Count();
                return new
                {
                    Live = liveMapped,
                    Schedule = scheduleMapped,
                    Past = pastMapped
                };
            }
            else
            {
                var scheduleMeetings = allMeeting.Where(e => e.ScheduleStart != null
                    && e.ScheduleStart.Value.Date >= DateTime.Today && e.Start == null);
                var scheduleMapped = mapper.Map<List<ScheduleMeetingForMemberGetDto>>(scheduleMeetings);
                int countSchedule = scheduleMeetings.Count();
                return new
                {
                    Live = liveMapped,
                    Schedule = scheduleMapped,
                    Past = pastMapped
                };
            }
        }
        public IQueryable<PastMeetingGetDto> GetPastMeetingsForGroup(int groupId)
        {
            return repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
               //.Where(e => e.GroupId == groupId && (e.End != null || (e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today && e.Start == null)))
               .Where(e => e.Schedule.GroupId == groupId
                    && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
               .ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider);
        }

        public IQueryable<LiveMeetingGetDto> GetLiveMeetingsForGroup(int groupId)
        {
            //var liveMeetings = repos.Meetings.GetList()
            //    .Where(e => e.Start != null && e.End == null);
            return repos.Meetings.GetList()
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => e.Schedule.GroupId == groupId && e.Start != null && e.End == null)
                .ProjectTo<LiveMeetingGetDto>(mapper.ConfigurationProvider);
        }

        public IQueryable<T> GetScheduleMeetingsForGroup<T>(int groupId)
        {
            //if()
            var list = repos.Meetings.GetList()
                //.Where(e => e.GroupId == groupId && (e.End != null || (e.ScheduleStart != null && e.ScheduleStart.Value.Date >= DateTime.Today)))
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => e.Schedule.GroupId == groupId
                    && e.ScheduleStart != null
                    && e.ScheduleStart.Value.Date >= DateTime.Today && e.Start == null);
            //.ProjectTo<ScheduleMeetingForMemberGetDto>(mapper.ConfigurationProvider);
            var mapped = list.ProjectTo<T>(mapper.ConfigurationProvider);
            return mapped;
        }
        public async Task<Object> GetAllMeetingsForStudent(int studentId)
        {
            var allMeeting = repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(m => m.Reviews).ThenInclude(c => c.Reviewee)
                .Include(m => m.Reviews).ThenInclude(c => c.Details).ThenInclude(d => d.Reviewer)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Include(m => m.Schedule).ThenInclude(a => a.Group).ThenInclude(g => g.GroupMembers)
                .Where(e => e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId))
                .OrderByDescending(e => e.Start.HasValue).ThenBy(e => e.Start)
                .ThenByDescending(e => e.ScheduleStart.HasValue).ThenBy(e => e.ScheduleStart)
                //.AsSplitQuery()
                .ToList();//.AsQueryable();
            int countAll = allMeeting.Count();
            var liveMeetings = (allMeeting.Where(e => e.Start != null && e.End == null));
            var liveMapped = mapper.Map<List<LiveMeetingGetDto>>(liveMeetings);
            var pastMeetings = (allMeeting.Where(e => (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                .OrderByDescending(e => e.Start.HasValue).ThenByDescending(e => e.Start)
                .ThenByDescending(e => e.ScheduleStart.HasValue).ThenByDescending(e => e.ScheduleStart)).ToList();
            var pastMapped = mapper.Map<List<PastMeetingGetDto>>(pastMeetings);
            int countLive = liveMeetings.Count();
            int countPast = pastMeetings.Count();
                var scheduleMeetings = (allMeeting.Where(e => e.ScheduleStart != null
                    && e.ScheduleStart.Value.Date >= DateTime.Today && e.Start == null));
                var scheduleMapped = mapper.Map<List<ScheduleMeetingForLeaderGetDto>>(scheduleMeetings);
            var leadGroupIds = repos.GroupMembers.GetList()
                .Where(gm => gm.AccountId == studentId && gm.MemberRole == GroupMemberRole.Leader)
                .Select(gm => gm.GroupId);
            foreach (var meeting in scheduleMapped)
            {
                if (!meeting.CanStart && leadGroupIds.Contains(meeting.ScheduleGroupId))
                {
                    meeting.CanStart = meeting.ScheduleStart.Value < DateTime.Now.AddHours(1);
                }
                //meeting.CanStart = true;
            }
            int countSchedule = scheduleMeetings.Count();
                return new
                {
                    Live = liveMapped,
                    Schedule = scheduleMapped,
                    Past = pastMapped
                };
        }
        public IQueryable<PastMeetingGetDto> GetPastMeetingsForStudent(int studentId)
        {
            //Nếu tháng này thì chỉ lấy past meeting
            IQueryable<Meeting> allMeetingsOfJoinedGroups = repos.Meetings.GetList()
                .Include(m => m.Chats).ThenInclude(c => c.Account)
                .Include(c => c.Connections)
                .Include(m => m.Schedule).ThenInclude(a => a.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId)
                    //lấy past meeting
                    && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                .OrderByDescending(e => e.Start.HasValue).ThenBy(e => e.Start)
                .ThenByDescending(e => e.ScheduleStart.HasValue).ThenBy(e => e.ScheduleStart);
            return allMeetingsOfJoinedGroups.ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider);
        }

        public IQueryable<PastMeetingGetDto> GetPastMeetingsForStudentByMonth(int studentId, DateTime month)
        {
            DateTime start = new DateTime(month.Year, month.Month, 1);
            DateTime end = start.AddMonths(1);
            //Nếu tháng này thì chỉ lấy past meeting
            IQueryable<Meeting> allMeetingsOfJoinedGroups = month.Month == DateTime.Now.Month
                ? repos.Meetings.GetList()
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => ((e.ScheduleStart >= start && e.ScheduleStart.Value.Date < end) || (e.Start >= start && e.Start.Value.Date < end))
                    && e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId)
                    //lấy past meeting
                    && (e.End != null || e.ScheduleStart != null && e.ScheduleStart.Value.Date < DateTime.Today))
                .OrderByDescending(e => e.Start.HasValue).ThenBy(e => e.Start)
                .ThenByDescending(e => e.ScheduleStart.HasValue).ThenBy(e => e.ScheduleStart)
                : repos.Meetings.GetList()
                .Include(c => c.Connections)
                .Include(m => m.Schedule).ThenInclude(a => a.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(c => c.Start >= start && c.Start.Value.Date < end
                    && c.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId))
                .OrderByDescending(e => e.Start.HasValue).ThenBy(e => e.Start)
                .ThenByDescending(e => e.ScheduleStart.HasValue).ThenBy(e => e.ScheduleStart); ;
            return allMeetingsOfJoinedGroups.ProjectTo<PastMeetingGetDto>(mapper.ConfigurationProvider);
        }

        public List<ScheduleMeetingForMemberGetDto> GetScheduleMeetingsForStudent(int studentId)
        {
            IQueryable<Meeting> scheduleMeetingsOfJoinedGroups = repos.Meetings.GetList()
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId)
                    //lấy past meeting
                    && (e.ScheduleStart != null && e.ScheduleStart.Value.Date >= DateTime.Today && e.Start == null))
                .OrderBy(e => e.ScheduleStart.Value);

            var mapped = scheduleMeetingsOfJoinedGroups.ProjectTo<ScheduleMeetingForMemberGetDto>(mapper.ConfigurationProvider).ToList();
            var leadGroupIds = repos.GroupMembers.GetList()
                .Where(gm => gm.AccountId == studentId && gm.MemberRole == GroupMemberRole.Leader)
                .Select(gm => gm.GroupId);
            foreach (var meeting in mapped)
            {
                if (!meeting.CanStart && leadGroupIds.Contains(meeting.ScheduleGroupId))
                {
                    meeting.CanStart = meeting.ScheduleStart.Value < DateTime.Now.AddHours(1);
                }
                //meeting.CanStart = true;
            }
            return mapped;
        }

        public IQueryable<ScheduleMeetingForMemberGetDto> GetScheduleMeetingsForStudentByDate(int studentId, DateTime date)
        {
            IQueryable<Meeting> scheduleMeetingsOfJoinedGroups = repos.Meetings.GetList()
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId)
                    //lấy schedule meeting
                    && (e.ScheduleStart != null && e.ScheduleStart.Value.Date >= DateTime.Today && e.Start == null)
                    && e.ScheduleStart.Value.Date == date)
                .OrderBy(e => e.ScheduleStart.Value);
            return scheduleMeetingsOfJoinedGroups.ProjectTo<ScheduleMeetingForMemberGetDto>(mapper.ConfigurationProvider);
        }

        public IQueryable<LiveMeetingGetDto> GetLiveMeetingsForStudent(int studentId)
        {
            IQueryable<Meeting> scheduleMeetingsOfJoinedGroups = repos.Meetings.GetList()
                .Include(c => c.Connections)
                .Include(a => a.Schedule).ThenInclude(m => m.Group).ThenInclude(g => g.GroupMembers)
                .Include(m => m.Schedule).ThenInclude(c => c.ScheduleSubjects).ThenInclude(ss => ss.Subject)
                .Where(e => e.Schedule.Group.GroupMembers.Any(gm => gm.AccountId == studentId)
                    //lấy live meeting
                    && e.Start != null && e.End == null)
                .OrderByDescending(e => e.Start.HasValue).ThenBy(e => e.Start);
            return scheduleMeetingsOfJoinedGroups.ProjectTo<LiveMeetingGetDto>(mapper.ConfigurationProvider);
        }

        public async Task UpdateScheduleMeetingAsync(int meetingId, ScheduleMeetingUpdateDto dto)
        {
            Meeting existed = await repos.Meetings.GetByIdAsync(meetingId);
            Meeting updated = new Meeting
            {
                Id = meetingId,
                Name = dto.Name,
                Content = dto.Content,
                ScheduleStart = dto.Date.Add(dto.ScheduleStartTime),
                ScheduleEnd = dto.Date.Add(dto.ScheduleEndTime),
            };
            existed.PatchUpdate(updated);
            await repos.Meetings.UpdateAsync(existed);
        }

        public async Task StartScheduleMeetingAsync(Meeting meeting)
        {
            await repos.Meetings.UpdateAsync(meeting);
        }

        public async Task DeleteScheduleMeetingAsync(Meeting meeting)
        {
            await repos.Meetings.RemoveAsync(meeting.Id);
        }

        public IQueryable<ScheduleGetDto> GetSchedulesForGroup(int groupId)
        {
            IQueryable<Schedule> schedules = repos.Schedules.GetList()
                .Include(c => c.ScheduleSubjects)
                .Include(e => e.Meetings);

            return schedules.ProjectTo<ScheduleGetDto>(mapper.ConfigurationProvider);
        }

        public async Task<string> UploadCanvas(int meetingId, IFormFile file)
        {
            string filePath;

            if (file != null && file.Length > 0)
            {
                string firebaseBucket = _config["Firebase:StorageBucket"];

                // Initialize FirebaseStorage instance
                var firebaseStorage = new FirebaseStorage(firebaseBucket);

                // Generate a unique file name
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                // Get reference to the file in Firebase Storage
                var fileReference = firebaseStorage.Child("MeetingCanvas").Child(uniqueFileName);

                // Upload the file to Firebase Storage
                using (var stream = file.OpenReadStream())
                {
                    await fileReference.PutAsync(stream);
                }

                // Get the download URL of the uploaded file
                string downloadUrl = await fileReference.GetDownloadUrlAsync();

                Meeting meeting = await repos.Meetings.GetList().SingleOrDefaultAsync(m => m.Id == meetingId);
                meeting.CanvasPath = downloadUrl;
                await repos.Meetings.UpdateAsync(meeting);

                // Update the discussion entity with the download URL
                return downloadUrl;
            }
            throw new Exception("No file found");
        }

        //public IQueryable<ChildrenLiveMeetingGetDto> GetChildrenLiveMeetings(int parentId)
        //{
        //    IQueryable<Account> children = repos.Accounts.GetList()
        //        .Where(a => a.SupervisionsForStudent.Any(su => su.ParentId == parentId && su.State == RequestStateEnum.Approved))
        //        .Include(a => a.Connections).ThenInclude(c => c.Meeting);
        //    Account temp = children.FirstOrDefault();
        //    return children.ProjectTo<ChildrenLiveMeetingGetDto>(mapper.ConfigurationProvider);
        //}
    }
}