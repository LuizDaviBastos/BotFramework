using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SpiderBot.Data.Interfaces;
using SpiderBot.Data.Repositories;

namespace SpiderBot.Data.Configuration
{
    public static class ConfigurationData
    {
        public static void AddSpiderMongoDataServices(this IServiceCollection services)
        {
            const string connectionString = @"";

            services.AddScoped<MongoClient>(x => new MongoClient(connectionString));
            services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
        }
    }
}
