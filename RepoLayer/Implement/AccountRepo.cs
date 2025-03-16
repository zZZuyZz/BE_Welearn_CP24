using DataLayer.DbContext;
using DataLayer.DbObject;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implemention
{
    public class AccountRepo : BaseRepo<Account>, IAccountRepo
    {
        private readonly WeLearnContext dbContext;

        public AccountRepo(WeLearnContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public override Task CreateAsync(Account entity)
        {
            return base.CreateAsync(entity);
        }

        public override async Task<Account> GetByIdAsync(int id)
        {
            return await dbContext.Accounts
                 .Include(x => x.GroupMembers).ThenInclude(x => x.Group).ThenInclude(x => x.Discussions).ThenInclude(x => x.AnswerDiscussion)
                 .Include(x => x.GroupMembers).ThenInclude(x => x.Group).ThenInclude(x => x.DocumentFiles)
                 .FirstOrDefaultAsync(x => x.Id == id);
        }



        public async Task<Account> GetProfileByIdAsync(int id)
        {
            return await dbContext.Accounts
                 .Include(e => e.Role)
                 .Include(e => e.GroupMembers).ThenInclude(e => e.Group).ThenInclude(e=>e.GroupMembers)
                 //.Include(e=>e.SupervisionsForStudent).ThenInclude(e=>e.Parent)
                 //.Include(e=>e.SupervisionsForParent).ThenInclude(e=>e.Student)
                 .SingleOrDefaultAsync(e => e.Id == id);
        } 
        public async Task<Account> GetByUsernameAsync(string username)
        {
            Account account =await dbContext.Accounts
                .Include(a => a.Role)
                .Include(e => e.GroupMembers).ThenInclude(e => e.Group).ThenInclude(e => e.GroupMembers)
                .SingleOrDefaultAsync(a => a.Username == username);
            return account;
        }
        public async Task<Account> GetByEmailAsync(string email)
        {
            Account account = await dbContext.Accounts
                .Include(a => a.Role)
                .Include(e => e.GroupMembers).ThenInclude(e => e.Group).ThenInclude(e => e.GroupMembers)
                .SingleOrDefaultAsync(a => a.Email == email);
            return account;
        }

        public async Task<Account> GetByUsernameOrEmailAndPasswordAsync(string usernameOrEmail, string password)
        {
            return await dbContext.Accounts
                .Include(a=>a.Role)
                .Include(e => e.GroupMembers).ThenInclude(e => e.Group).ThenInclude(e => e.GroupMembers)
                .SingleOrDefaultAsync(a => (a.Username == usernameOrEmail || a.Email == usernameOrEmail.ToLower()) && a.Password == password);
        }

        public override IQueryable<Account> GetList()
        {
            return base.GetList();
        }


        public override Task RemoveAsync(int id)
        {
            return base.RemoveAsync(id);
        }

        public override Task UpdateAsync(Account entity)
        {
            return base.UpdateAsync(entity);
        }
        //SignalR
        //////////////////////////////////////////////////////////////////////
        public async Task<Account> GetUserByUsernameSignalrAsync(string username)
        {
            return await dbContext.Accounts.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<Account> GetMemberSignalrAsync(string username)
        {

            return  await dbContext.Accounts.SingleOrDefaultAsync(x => x.Username == username);
            //.ProjectTo<MemberSignalrDto>(mapper.ConfigurationProvider);
            //add CreateMap<AppUser, MemberDto>(); in AutoMapperProfiles
            //.SingleOrDefaultAsync();
        }

    }
}
