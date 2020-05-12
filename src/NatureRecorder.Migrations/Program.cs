using Microsoft.EntityFrameworkCore;
using NatureRecorder.Data;

namespace NatureRecorder.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            NatureRecorderDbContext context = new NatureRecorderDbContextFactory().CreateDbContext(null);
            context.Database.Migrate();
        }
    }
}
