using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace UniDesk.Web.HealthChecks
{
    public class DiskSpaceHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DiskSpaceHealthCheck(
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var minimumFreeSpaceMb = _configuration.GetValue<long>(
                "Diagnostics:MinimumFreeDiskSpaceMb",
                100);

            var databaseDirectory = GetDatabaseDirectory();

            Directory.CreateDirectory(databaseDirectory);

            var root = Path.GetPathRoot(databaseDirectory);

            if (string.IsNullOrWhiteSpace(root))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Cannot determine disk root for SQLite database directory."));
            }

            var driveInfo = new DriveInfo(root);
            var availableBytes = driveInfo.AvailableFreeSpace;
            var availableMb = availableBytes / 1024 / 1024;

            var data = new Dictionary<string, object>
            {
                ["DatabaseDirectory"] = databaseDirectory,
                ["DiskRoot"] = root,
                ["AvailableFreeSpaceMb"] = availableMb,
                ["MinimumRequiredFreeSpaceMb"] = minimumFreeSpaceMb
            };

            if (availableMb < minimumFreeSpaceMb)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Low disk space for SQLite database directory. Available: {availableMb} MB.",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"Disk space OK for SQLite database directory. Available: {availableMb} MB.",
                data: data));
        }

        private string GetDatabaseDirectory()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return _environment.ContentRootPath;
            }

            var connectionBuilder = new SqliteConnectionStringBuilder(connectionString);
            var dataSource = connectionBuilder.DataSource;

            if (string.IsNullOrWhiteSpace(dataSource) || dataSource == ":memory:")
            {
                return _environment.ContentRootPath;
            }

            var databasePath = Path.IsPathRooted(dataSource)
                ? dataSource
                : Path.Combine(_environment.ContentRootPath, dataSource);

            return Path.GetDirectoryName(databasePath) ?? _environment.ContentRootPath;
        }
    }
}