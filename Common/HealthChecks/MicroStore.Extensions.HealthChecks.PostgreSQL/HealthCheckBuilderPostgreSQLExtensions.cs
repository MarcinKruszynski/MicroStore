using Microsoft.Extensions.HealthChecks;
using Npgsql;
using System;
using System.Data;

namespace MicroStore.Extensions.HealthChecks
{
    public static class HealthCheckBuilderPostgreSQLExtensions
    {
        public static HealthCheckBuilder AddSqlCheck(this HealthCheckBuilder builder, string name, string connectionString)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return AddNpgsqlCheck(builder, name, connectionString, builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddNpgsqlCheck(this HealthCheckBuilder builder, string name, string connectionString, TimeSpan cacheDuration)
        {
            builder.AddCheck($"NpgsqlCheck({name})", async () =>
            {
                try
                {                    
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = "SELECT 1";
                            var result = (int)await command.ExecuteScalarAsync().ConfigureAwait(false);
                            if (result == 1)
                            {
                                return HealthCheckResult.Healthy($"NpgsqlCheck({name}): Healthy");
                            }

                            return HealthCheckResult.Unhealthy($"NpgsqlCheck({name}): Unhealthy");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"NpgsqlCheck({name}): Exception during check: {ex.GetType().FullName}");
                }
            }, cacheDuration);

            return builder;
        }
    }
}
