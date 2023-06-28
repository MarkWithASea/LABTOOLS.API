using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;


namespace LABTOOLS.API.Data.Repositories
{
    public class AdminRoleRepository : EfCoreRepository<Role>
    {
        public AdminRoleRepository(AppDbContext context)
            : base(context)
        { }

        public AdminRoleRepository(IHttpContextAccessor httpContextAccessor, AppSettings appSettings)
            : base(httpContextAccessor, appSettings)
        { }

        public override async Task<Role> Get(int id)
        {
            var role = await Context.Roles
                .Include(r => r.Permissions)
                .FirstOrDefaultAsync();

            return role!;
        }

        public override IQueryable<Role> GetQuery(string CognitoGroupName)
        {
            return Context.Roles
                .Include(r => r.Permissions);
        }
    }
}
