using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Catalog.API.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            var retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgres database...");

                    using var connection = 
                        new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand()
                    {
                        Connection = connection
                    };

                    command.CommandText = @"CREATE TABLE IF NOT EXISTS 
                                                Coupon(ID SERIAL PRIMARY KEY NOT NULL, 
                                                        ProductName VARCHAR(24) NOT NULL, 
                                                        Description TEXT, 
                                                        Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = @"INSERT INTO coupon(productname, description, amount) 
                                            SELECT 'Iphone X' AS productname, 'Iphone X description' AS description, 150 AS Price 
                                                WHERE 0 = (SELECT count(*) FROM coupon)
                                            UNION SELECT 'Samsung 10' AS productname, 'Samsung 10 description' AS description, 100 AS Price 
                                                WHERE 0 = (SELECT count(*) FROM coupon)";
                    command.ExecuteNonQuery();
                    
                    logger.LogInformation("Migrated postgres database");
                }
                catch (NpgsqlException e)
                {
                    logger.LogError(e, "An error occured while migrating the postgresql database");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }

            return host;
        }
    }
}