using Microsoft.EntityFrameworkCore;

namespace Stock.Service.Models.Contexts
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions options) : base(options)
        {
        }

        
    }
}
