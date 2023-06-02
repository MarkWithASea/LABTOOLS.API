using LABTOOLS.API.Models;

namespace LABTOOLS.API.Data
{
    public interface IRepository<T>
        where T : class, IEntity
    {
        Task<List<T>> GetAll();

        Task<T> Get(int id);

        Task<T> Add(T entity);

        Task<T> Update(T entity);

        Task<T> Delete(int id);

        Task<int> Save();

        IQueryable<T> GetQuery(string userCognitoId);

        Task<User> GetUser(string cognitoId);
    }
}