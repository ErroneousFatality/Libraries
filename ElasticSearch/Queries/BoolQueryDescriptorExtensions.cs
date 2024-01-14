using Elastic.Clients.Elasticsearch.QueryDsl;

namespace AndrejKrizan.ElasticSearch.Queries;
public static class BoolQueryDescriptorExtensions
{
    public static BoolQueryDescriptor<TRecord> ConditionalMust<TRecord, TArgument>(this BoolQueryDescriptor<TRecord> source,
        ICollection<TArgument>? arguments,
        Func<ICollection<TArgument>, Action<QueryDescriptor<TRecord>>> createConfigure
    )
        => arguments == null || arguments.Count < 1
        ? source
        : source.Must(createConfigure(arguments));

    public static BoolQueryDescriptor<TRecord> ConditionalMust<TRecord>(this BoolQueryDescriptor<TRecord> source,
        ICollection<Query>? queries
    )
        => queries == null || queries.Count < 1
        ? source
        : source.Must(queries);

    public static BoolQueryDescriptor<TRecord> ConditionalMust<TRecord, TArgument>(this BoolQueryDescriptor<TRecord> source,
        ICollection<TArgument>? arguments,
        Func<ICollection<TArgument>, QueryDescriptor<TRecord>> createDescriptor
    )
        => arguments == null || arguments.Count < 1
        ? source
        : source.Must(createDescriptor(arguments));
}
