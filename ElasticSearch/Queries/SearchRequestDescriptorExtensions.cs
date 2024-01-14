using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace AndrejKrizan.ElasticSearch.Queries;

public static class SearchRequestDescriptorExtensions
{
    public static SearchRequestDescriptor<TRecord> ConditionalQuery<TRecord, TArgument>(this SearchRequestDescriptor<TRecord> searchRequest,
        ICollection<TArgument>? arguments,
        Func<ICollection<TArgument>, Query?> createQuery
    )
        => arguments == null || arguments.Count < 1
        ? searchRequest
        : searchRequest.Query(createQuery(arguments));

    public static SearchRequestDescriptor<TRecord> ConditionalQuery<TRecord, TArgument>(this SearchRequestDescriptor<TRecord> searchRequest,
        ICollection<TArgument>? arguments,
        Func<ICollection<TArgument>, Action<QueryDescriptor<TRecord>>> createConfigure
    )
        => arguments == null || arguments.Count < 1
        ? searchRequest
        : searchRequest.Query(createConfigure(arguments));

    public static SearchRequestDescriptor<TRecord> ConditionalQuery<TRecord, TArgument>(this SearchRequestDescriptor<TRecord> searchRequest,
        ICollection<TArgument>? arguments,
        Func<ICollection<TArgument>, QueryDescriptor<TRecord>> createDescriptor
    )
    => arguments == null || arguments.Count < 1
    ? searchRequest
    : searchRequest.Query(createDescriptor(arguments));
}
