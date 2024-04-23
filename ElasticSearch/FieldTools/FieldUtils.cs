using System.Linq.Expressions;

using AndrejKrizan.DotNet.Lambdas;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.FieldTools;
public static class FieldUtils
{
    // Methods
    public static Fields Create(IEnumerable<LambdaExpression> fields)
    {
        if (!fields.Any())
        {
            throw new ArgumentException("Fields must have at least one field.", nameof(fields));
        }
        Fields _fields = new Field(fields.First()).ToFields();
        _fields = _fields.And(fields.Skip(1));
        return _fields;
    }

    public static Fields Create<TOwner, TRecord>(Expression<Func<TOwner, TRecord>> record, IEnumerable<LambdaExpression> recordFields)
        where TOwner : class
    {
        IEnumerable<LambdaExpression> fields = record.Explode(recordFields);
        Fields _fields = Create(fields);
        return _fields;
    }
}
