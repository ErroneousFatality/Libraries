using AndrejKrizan.DotNet.Ordering;

using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.Extensions;

public static class OrderDirectionExtensions
{
    public static SortOrder ToSortOrder(this OrderDirection orderDirection)
        => orderDirection switch
        {
            OrderDirection.Ascending => SortOrder.Asc,
            OrderDirection.Descending => SortOrder.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(orderDirection), "Unknown order direction."),
        };

    public static FieldSort ToFieldSort(this OrderDirection orderDirection)
        => new() { Order = orderDirection.ToSortOrder(), Missing = "_last" };
}
