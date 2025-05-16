using AndrejKrizan.DotNet.Ordering;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;

namespace AndrejKrizan.ElasticSearch.Ordering;

public static class OrderDirectionExtensions
{
    public static SortOrder ToSortOrder(this OrderDirection orderDirection)
        => orderDirection switch
        {
            OrderDirection.Ascending => SortOrder.Asc,
            OrderDirection.Descending => SortOrder.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(orderDirection), "Unknown order direction."),
        };

    public static FieldSort ToFieldSort(this OrderDirection orderDirection,
        Field field,
        SortMode? mode = null,
        string? missing = "_last",
        FieldType? unmappedType = FieldType.Long
    )
        => new()
        {
            Field = field,
            Order = orderDirection.ToSortOrder(),
            Mode = mode,
            UnmappedType = unmappedType,
            Missing = missing ?? (FieldValue?)null,
        };
}
