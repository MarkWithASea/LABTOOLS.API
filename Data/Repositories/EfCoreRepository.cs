using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data.Repositories
{
    public abstract class EfCoreRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly AppDbContext _context;

        public EfCoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public EfCoreRepository(IHttpContextAccessor httpContextAccessor, AppSettings appSettings)
        {
            string clientConnectionString = appSettings.ConnectionString;
            _context = new AppDbContext(httpContextAccessor, clientConnectionString);
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await Save();
            return entity;
        }
        
        public virtual async Task<TEntity> Delete(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return entity!;
            }

            _context.Set<TEntity>().Remove(entity);
            await Save();

            return entity;
        }

        public virtual async Task<TEntity> Get(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            return entity!;
        }

        public virtual async Task<List<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity> Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await Save();
            return entity;
        }

        public virtual IEnumerable<TEntity> FindAll(Func<TEntity, bool> predicate)
        {
            var entities = _context.Set<TEntity>().Where(predicate);
            return entities;
        }

        public virtual IQueryable<TEntity> GetQuery(string userCognitoId)
        {
            return _context.Set<TEntity>();
        }

        public virtual async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<User> GetUser(string cognitoId)
        {
            var user = await _context.Users
                .Include(user => user.Roles)
                .Where(user => user.CognitoId == cognitoId)!
                .FirstOrDefaultAsync();

            return user!;
        }

        public AppDbContext Context
        {
            get
            {
                return _context;
            }
        }
    }
}