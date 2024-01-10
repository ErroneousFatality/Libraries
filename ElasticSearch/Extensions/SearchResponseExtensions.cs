using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.Extensions;
internal static class SearchResponseExtensions
{
    public static IEnumerable<TRecord> GetRecords<TRecord>(this SearchResponse<TRecord> searchResponse)
        => searchResponse.HitsMetadata.Hits.Select(s => s.Source!);
}
