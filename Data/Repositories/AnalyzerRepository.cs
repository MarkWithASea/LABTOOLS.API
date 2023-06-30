using LABTOOLS.API.Helpers;
using LABTOOLS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LABTOOLS.API.Data.Repositories
{
    public class AnalyzerRepository : EfCoreRepository<Models.Analyzer>
    {
        public AnalyzerRepository(AppDbContext appDbContext)
            : base(appDbContext)
        { }

        public AnalyzerRepository(IHttpContextAccessor httpContextAccessor, AppSettings appSettings)
            : base(httpContextAccessor, appSettings)
        { }
    }
}