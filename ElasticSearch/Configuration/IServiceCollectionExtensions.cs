using AndrejKrizan.ElasticSearch.UnitsOfWork;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

using Microsoft.Extensions.DependencyInjection;

namespace AndrejKrizan.ElasticSearch.Configuration;

public static class IServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddElasticSearch(
            ElasticSearchSettings settings,
            Func<ElasticsearchClientSettings, ElasticsearchClientSettings> configure
        )
        {
            ElasticsearchClientSettings clientSettings = new ElasticsearchClientSettings(new Uri(settings.Uri))
                .Authentication(new BasicAuthentication(settings.Username, settings.Password))
                .ThrowExceptions();
            clientSettings = configure(clientSettings);
            ElasticsearchClient client = new(clientSettings);
            return services.AddSingleton(client);
        }

        public  IServiceCollection AddElasticSearch<IUnitOfWork, UnitOfWork>(
            ElasticSearchSettings settings,
            Func<ElasticsearchClientSettings, ElasticsearchClientSettings> configure
        )
            where IUnitOfWork : class
            where UnitOfWork : ElasticSearchUnitOfWork, IUnitOfWork
            => services
                .AddElasticSearch(settings, configure)
                .AddScoped<UnitOfWork>().AddTransient<IUnitOfWork, UnitOfWork>(services => services.GetRequiredService<UnitOfWork>());
    }
}
