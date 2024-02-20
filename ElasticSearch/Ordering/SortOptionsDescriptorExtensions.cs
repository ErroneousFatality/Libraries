using System.Linq.Expressions;

using AndrejKrizan.DotNet.Ordering;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;

namespace AndrejKrizan.ElasticSearch.Ordering;

public static class SortOptionsDescriptorExtensions
{
    public static SortOptionsDescriptor<T> Field<T>(this SortOptionsDescriptor<T> sort,
        Expression<Func<T, object>> selector,
        OrderDirection direction,
        SortMode? mode = null,
        string? missing = "_last",
        FieldType? unmappedType = FieldType.Long
    )
        => sort.Field(selector, direction.ToFieldSort(mode, missing, unmappedType));

    public static SortOptionsDescriptor<T> Score<T>(this SortOptionsDescriptor<T> sort,
        SortOrder? order = null
    )
        => sort.Score(new ScoreSort() { Order = order });

}
