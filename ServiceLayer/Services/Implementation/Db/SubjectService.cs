using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.DbObject;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using ServiceLayer.Services.Interface.Db;

namespace ServiceLayer.Services.Implementation.Db
{
    internal class SubjectService : ISubjectService
    {
        private IRepoWrapper repos;
        private IMapper mapper;

        public SubjectService(IRepoWrapper repos, IMapper mapper)
        {
            this.repos = repos;
            this.mapper = mapper;
        }

        public async Task CreateSubjectAsync(string name)
        {
            await repos.Subjects.CreateSubjectAsync(name);
        }

        public IQueryable<T> GetList<T>()
        {
            return repos.Subjects.GetList().ProjectTo<T>(mapper.ConfigurationProvider);
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await repos.Subjects.GetList().AnyAsync(s=>s.Id == id);
        }
    }
}