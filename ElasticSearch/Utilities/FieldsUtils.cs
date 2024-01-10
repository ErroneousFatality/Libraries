using System.Linq.Expressions;

using AndrejKrizan.DotNet.Lambdas;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.Utilities;
public static class FieldsUtils
{
    // Methods
    public static Fields Create<TOwner, TRecord>(
        Expression<Func<TOwner, TRecord>> record,
        IEnumerable<Expression<Func<TRecord, object>>> recordFields

    )
        where TOwner : class
        where TRecord : class
    {
        IEnumerable<Expression<Func<TOwner, object>>> fields = recordFields.Select(field => record.Extend(field));
        Fields _fields = Create(fields);
        return _fields;
    }

    public static Fields Create<TRecord>(IEnumerable<Expression<Func<TRecord, object>>> fields)
        where TRecord : class
    {
        if (!fields.Any())
            throw new ArgumentException("Fields must have at least one field.", nameof(fields));
        Fields _fields = (Fields)new Field(fields.First());
        foreach (Expression<Func<TRecord, object>> propertyNavigation in fields.Skip(1))
            _fields = _fields.And<TRecord>(propertyNavigation);
        return _fields;
    }
}
