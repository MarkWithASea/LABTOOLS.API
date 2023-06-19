
using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data.Repositories
{
    public class UserRepository : EfCoreRepository<User>
    {
        public UserRepository(AppDbContext appDbContext)
            : base(appDbContext)
        { }

        public UserRepository(IHttpContextAccessor httpContextAccessor, AppSettings appSettings)
            : base(httpContextAccessor, appSettings)
        { }

        public override async Task<User> Get(int id)
        {
            var user = await Context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

                return user!;
        }
        
        public override IQueryable<User> GetQuery(string userCognitoId)
        {
            return Context.Users
                .Include(u => u.Roles)
                .Where(u => u.CognitoId == userCognitoId);
        }
        
        public async Task<User> GetUserToDelete(int id)
        {
            var user = await Context.Users
                .Where(u => u.Id == id
                    && u.IsDisabled)
                .FirstOrDefaultAsync();

            return user!;
        }

        public async Task<User> GetDisabledUsers(int id)
        {
            var user = await Context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == id
                    && u.IsDisabled
                    && !u.IsDeleted)
                .FirstOrDefaultAsync();

            return user!;
        }
    }
}