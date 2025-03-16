using DataLayer.DbObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interface
{
    public interface IAnswerDiscussionRepository : IBaseRepo<AnswerDiscussion>
    {
        public Task<List<AnswerDiscussion>> GetAnswerDiscussionsByDiscussionId(int discussionId);

    }
}
