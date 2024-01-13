using System.Linq.Expressions;

using AndrejKrizan.DotNet.Lambdas;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.FieldTools;
public static class FieldsExtensions
{
    public static Fields And<TRecord>(this Fields source, IEnumerable<Expression<Func<TRecord, object>>> fields)
        where TRecord : class
    {
        Field[] array = fields.Select(field => new Field(field)).ToArray();
        Fields _fields = source.And(array);
        return _fields;
    } 

    public static Fields And<TOwner, TRecord>(this Fields source,
        Expression<Func<TOwner, TRecord>> record,
        IEnumerable<Expression<Func<TRecord, object>>> recordFields
    )
    where TOwner : class
    {
        IEnumerable<Expression<Func<TOwner, object>>> lambdas = record.Explode(recordFields);
        Fields _fields = source.And(lambdas);
        return _fields;
    }
}
