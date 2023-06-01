using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data
{
    public interface IDataContext
    {
        DbSet<User> Users { get; init; }
    }
}