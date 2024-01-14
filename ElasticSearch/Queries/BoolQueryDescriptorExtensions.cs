using Elastic.Clients.Elasticsearch.QueryDsl;

namespace AndrejKrizan.ElasticSearch.Queries;
public static class BoolQueryDescriptorExtensions
{
    public static BoolQueryDescriptor<TRecord> ConditionalMust<TRecord, T>(this BoolQueryDescriptor<TRecord> boolQuery,
        ICollection<T>? arguments,
        Func<ICollection<T>, Action<QueryDescriptor<TRecord>>> createConfigurator
    )
        => arguments == null || arguments.Count < 1
        ? boolQuery
        : boolQuery.Must(createConfigurator(arguments));

    public static BoolQueryDescriptor<TRecord> ConditionalMust<TRecord>(this BoolQueryDescriptor<TRecord> boolQuery, 
        ICollection<Query>? conditions
    )
        => conditions == null || conditions.Count < 1
        ? boolQuery
        : boolQuery.Must(conditions);

    public static BoolQueryDescriptor<TRecord> ConditionalMust<TRecord, T>(this BoolQueryDescriptor<TRecord> boolQuery,
        ICollection<T>? arguments,
        Func<ICollection<T>, QueryDescriptor<TRecord>> createDescriptor
    )
        => arguments == null || arguments.Count < 1
        ? boolQuery
        : boolQuery.Must(createDescriptor(arguments));
}
