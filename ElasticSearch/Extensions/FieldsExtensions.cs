using System.Linq.Expressions;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.Extensions;
internal static class FieldsExtensions
{
    public static Fields And<TRecord>(this Fields source, IEnumerable<Expression<Func<TRecord, object>>> fields)
        where TRecord : class
        => source.And(fields.Select(field => new Field(field)).ToArray());

}
