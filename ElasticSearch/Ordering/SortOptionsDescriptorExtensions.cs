using System.Linq.Expressions;

using AndrejKrizan.DotNet.Ordering;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.Ordering;

public static class SortOptionsDescriptorExtensions
{
    public static SortOptionsDescriptor<T> Field<T>(this SortOptionsDescriptor<T> sort,
        Expression<Func<T, object>> selector,
        OrderDirection direction
    )
        => sort.Field(selector, direction.ToFieldSort());

    public static SortOptionsDescriptor<T> Score<T>(this SortOptionsDescriptor<T> sort,
        SortOrder? order = null
    )
        => sort.Score(new ScoreSort() { Order = order });

}
