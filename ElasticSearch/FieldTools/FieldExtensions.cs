using System.Linq.Expressions;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.FieldTools;
public static class FieldExtensions
{
    public static Fields And<TRecord>(this Field source, IEnumerable<Expression<Func<TRecord, object>>> fields)
        where TRecord : class

        => ((Fields)source).And(fields);

    public static Fields And<TOwner, TRecord>(this Field source,
        Expression<Func<TOwner, TRecord>> record,
        IEnumerable<Expression<Func<TRecord, object>>> recordFields
    )
        where TOwner : class
        => ((Fields)source).And(record, recordFields);
}
