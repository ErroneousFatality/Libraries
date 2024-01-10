using AndrejKrizan.ElasticSearch.Repositories;

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace AndrejKrizan.ElasticSearch.Extensions;

public static class MultiMatchQueryDescriptorExtensions
{
    public static MultiMatchQueryDescriptor<TRecord> Fuzzy<TRecord>(this MultiMatchQueryDescriptor<TRecord> query, string level = RecordRepository.Fuzziness_Automatic)
        => query.Fuzziness(new Fuzziness(level));
}
