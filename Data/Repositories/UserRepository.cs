
using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data.Repositories
{
    public class UserRepository : EfCoreRepository<User>
    {
        public UserRepository(AppDbContext context)
            : base(context)
        { }

        public override async Task<User> Get(int id)
        {
            var user = await Context.Users
                //.Include(u => u.Roles)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

                return user!;
        }
        
        public override IQueryable<User> GetQuery(string userCognitoId)
        {
            return Context.Users
                //.Include(u => u.Roles)
                .Where(u => u.CognitoId == userCognitoId);
        }
    }
}