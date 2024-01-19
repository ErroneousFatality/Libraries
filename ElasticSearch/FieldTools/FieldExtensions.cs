using System.Linq.Expressions;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.FieldTools;
public static class FieldExtensions
{
    public static Fields And(this Field source, IEnumerable<LambdaExpression> fields)
        => ((Fields)source).And(fields);

    public static Fields And<TOwner, TRecord>(this Field source, Expression<Func<TOwner, TRecord>> record, IEnumerable<LambdaExpression> recordFields)
        where TOwner : class
        => ((Fields)source).And(record, recordFields);
}
