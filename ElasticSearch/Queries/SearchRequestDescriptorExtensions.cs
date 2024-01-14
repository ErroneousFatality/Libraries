using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace AndrejKrizan.ElasticSearch.Queries;

public static class SearchRequestDescriptorExtensions
{
    public static SearchRequestDescriptor<TRecord> ConditionalQuery<TRecord, T>(this SearchRequestDescriptor<TRecord> searchRequest,
        IReadOnlyCollection<T>? arguments,
        Func<IReadOnlyCollection<T>, Query?> createQuery
    )
        => arguments == null || arguments.Count < 1
        ? searchRequest
        : searchRequest.Query(createQuery(arguments));

    public static SearchRequestDescriptor<TRecord> ConditionalQuery<TRecord, T>(this SearchRequestDescriptor<TRecord> searchRequest,
        IReadOnlyCollection<T>? arguments,
        Func<IReadOnlyCollection<T>, Action<QueryDescriptor<TRecord>>> createConfigurator
    )
        => arguments == null || arguments.Count < 1
        ? searchRequest
        : searchRequest.Query(createConfigurator(arguments));

    public static SearchRequestDescriptor<TRecord> ConditionalQuery<TRecord, T>(this SearchRequestDescriptor<TRecord> searchRequest,
        IReadOnlyCollection<T>? arguments,
        Func<IReadOnlyCollection<T>, QueryDescriptor<TRecord>> createQueryDescriptor
    )
    => arguments == null || arguments.Count < 1
    ? searchRequest
    : searchRequest.Query(createQueryDescriptor(arguments));
}
