using Domain.Database;
using Domain.Enums;

namespace WebAPI.Configuration.Db
{
    public class DbSeedConfig : IDbSeedConfig
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration _config { get; }
        private ILogger<DbSeedConfig> _logger;

        public DbSeedConfig(ApplicationDbContext context, IConfiguration config, ILogger<DbSeedConfig> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        public async Task Seed()
        {
            _logger.LogInformation("Seeding Db...");
            await _context.Init();
            _logger.LogInformation("Seeded Db.");
        }
    }
}
