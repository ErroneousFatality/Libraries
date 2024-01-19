using System.Linq.Expressions;

using AndrejKrizan.DotNet.Lambdas;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.FieldTools;
public static class FieldsExtensions
{
    public static Fields And(this Fields source, IEnumerable<LambdaExpression> fields)
    {
        Field[] array = fields.Select(field => new Field(field)).ToArray();
        Fields _fields = source.And(array);
        return _fields;
    }

    public static Fields And<TOwner, TRecord>(this Fields source, Expression<Func<TOwner, TRecord>> record, IEnumerable<LambdaExpression> recordFields)
        where TOwner : class
    {
        IEnumerable<LambdaExpression> lambdas = record.Explode(recordFields);
        Fields _fields = source.And(lambdas);
        return _fields;
    }
}
