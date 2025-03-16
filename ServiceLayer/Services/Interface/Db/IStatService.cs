using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface.Db
{
    public interface IStatService
    {
        public Task<StatGetDto> GetStatForAccountInMonth(int studentId, DateTime month);
        public Task<StatGetDto> GetStatForGroupInMonth(int groupId, DateTime month);
        public Task<IList<StatGetListDto>> GetStatsForStudent(int studentId);
        public Task<IList<StatGetListDto>> GetStatsForGroup(int groupId);
    }
}
