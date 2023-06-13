using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data.Repositories
{
    public class AdminUserRepository : EfCoreRepository<User>
    {
        public AdminUserRepository(AppDbContext context)
            : base(context)
        { }

        public AdminUserRepository(IHttpContextAccessor httpContextAccessor, AppSettings appSettings)
            : base(httpContextAccessor, appSettings)
        { }

        public override async Task<User> Get(int id)
        {
            var user = await Context.Users
                //.Include(u => u.Roles)
                .Where(u => u.Id == id
                    && u.IsDeleted == false)
                .FirstOrDefaultAsync();

            return user!;
        }

        public async Task<User> GetUserToDelete(int id)
        {
            var user = await Context.Users
                .Where(u => u.Id == id
                    && u.IsDisabled == true)
                .FirstOrDefaultAsync();

            return user!;
        }

        public async Task<User> GetDisabledUsers(int id)
        {
            var user = await Context.Users
                //.Include(u => u.Roles)
                .Where(u => u.Id == id
                    && u.IsDisabled == true
                    && u.IsDeleted == false)
                .FirstOrDefaultAsync();

            return user!;
        }

        public override IQueryable<User> GetQuery(string userCognitoId)
        {
            return Context.Users
                //.Include(u => u.Roles)
                .Where(u => u.IsDisabled == false && u.IsDeleted == false);
        }
    }
}
