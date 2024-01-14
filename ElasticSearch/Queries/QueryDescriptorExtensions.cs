using Elastic.Clients.Elasticsearch.QueryDsl;

namespace AndrejKrizan.ElasticSearch.Queries;
public static class QueryDescriptorExtensions
{
    public static QueryDescriptor<TRecord> ConditionalBool<TRecord, T>(this QueryDescriptor<TRecord> query, 
        IReadOnlyCollection<T>? arguments,
        Func<IReadOnlyCollection<T>, Action<BoolQueryDescriptor<TRecord>>> createConfigurator
    )
        => arguments == null || arguments.Count < 1
        ? query
        : query.Bool(createConfigurator(arguments));

    public static QueryDescriptor<TRecord> ConditionalBool<TRecord, T>(this QueryDescriptor<TRecord> query, 
        IReadOnlyCollection<T>? arguments,
        Func<IReadOnlyCollection<T>, BoolQuery> createBoolQuery
    )
        => arguments == null || arguments.Count < 1
        ? query
        : query.Bool(createBoolQuery(arguments));
}
