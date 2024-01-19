using System.Collections.Immutable;

using AndrejKrizan.DotNet.Collections;
using AndrejKrizan.DotNet.Pagination;
using AndrejKrizan.DotNet.Records;
using AndrejKrizan.DotNet.Strings;
using AndrejKrizan.DotNet.Utilities;
using AndrejKrizan.ElasticSearch.Extensions;
using AndrejKrizan.ElasticSearch.Pagination;
using AndrejKrizan.ElasticSearch.UnitsOfWork;

using Elastic.Clients.Elasticsearch;

using Microsoft.Extensions.Logging;

namespace AndrejKrizan.ElasticSearch.Repositories;

public abstract class RecordRepository<TRecord, TId> : RecordRepository, IRecordRepository<TRecord, TId>
    where TRecord : class
    where TId : notnull
{
    // Fields
    protected readonly ElasticsearchClient Client;
    protected readonly ElasticSearchUnitOfWork UnitOfWork;
    private readonly ILogger Logger;

    // Constructors
    protected RecordRepository(
        ElasticsearchClient client,
        ElasticSearchUnitOfWork unitOfWork,
        ILogger logger
    )
    {
        Client = client;
        UnitOfWork = unitOfWork;
        Logger = logger;
    }

    // Methods
    public async Task InsertOrUpdateAsync(TRecord record, CancellationToken cancellationToken = default)
    {
        IndexResponse response = await Client.IndexAsync(record, cancellationToken);
        response.Validate(Logger);
    }

    public void ScheduleInsertOrUpdate(TRecord record)
        => UnitOfWork.ScheduleAction(async (CancellationToken cancellationToken = default)
            => await InsertOrUpdateAsync(record, cancellationToken)
        );


    public async Task InsertOrUpdateManyAsync(IEnumerable<TRecord> records, CancellationToken cancellationToken = default)
    {
        if (records.Any())
        {
            BulkResponse responses = await Client.IndexManyAsync(records, cancellationToken);
            responses.Validate(Logger);
        }
    }

    public void ScheduleInsertOrUpdateMany(IEnumerable<TRecord> records)
        => UnitOfWork.ScheduleAction(async (CancellationToken cancellationToken = default)
            => await InsertOrUpdateManyAsync(records, cancellationToken)
        );



    public async Task<TRecord?> GetAsync(TId id, CancellationToken cancellationToken = default)
    {
        Id _id = CreateId(id);
        GetResponse<TRecord> response = await Client.GetAsync<TRecord>(_id, cancellationToken);
        response.Validate(Logger);
        return response.Source;
    }

    public async Task<ImmutableDictionary<TId, TRecord>> GetManyAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default)
    {
        if (!ids.Any())
        {
            return ImmutableDictionary<TId, TRecord>.Empty;
        }
        Ids _ids = CreateIds(ids);
        MultiGetResponse<TRecord> response = await Client.MultiGetAsync<TRecord>(
            new MultiGetRequest
            {
                Ids = _ids
            },
            cancellationToken
        );
        response.Validate(Logger);

        List<string> errorMessages = new(response.Docs.Count);
        ImmutableDictionary<TId, TRecord> recordDictionary = response.Docs
            .Select(response
                => response.Match<KeyValuePair<TId, TRecord>?>(
                    result => result.Found
                        ? new KeyValuePair<TId, TRecord>(Utils.ConvertTo<TId>(result.Id), result.Source!)
                        : null,
                    error =>
                    {
                        if (error.Error.Reason != null)
                            errorMessages.Add(error.Error.Reason);
                        return null;
                    }
                )
            )
            .Where(pair => pair.HasValue)
            .Select(pair => pair!.Value)
            .ToImmutableDictionary();
        if (errorMessages.Count > 0)
        {
            throw new Exception("Elastic search response has errors:\n" + errorMessages.StringJoin(separator: Environment.NewLine));
        }
        return recordDictionary;
    }



    public async Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        Id _id = CreateId(id);
        DeleteResponse response = await Client.DeleteAsync<TRecord>(_id, cancellationToken);
        response.Validate(Logger);
    }

    public void ScheduleDelete(TId id)
        => UnitOfWork.ScheduleAction(async (CancellationToken cancellationToken = default)
            => await DeleteAsync(id, cancellationToken)
        );



    // Protected methods
    protected static Id CreateId(TId id)
    {
        string? str = id.ToString();
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentNullException(nameof(id));
        }
        Id _id = new(str);
        return _id;
    }

    protected static Ids CreateIds(IEnumerable<TId> ids)
        => new Ids(ids.Select(CreateId));

    protected async Task<IEnumerable<TRecord>> GetManyAsync(
        Action<SearchRequestDescriptor<TRecord>> configureSearch,
        CancellationToken cancellationToken = default
    )
    {
        SearchResponse<TRecord> response = await Client.SearchAsync(configureSearch, cancellationToken);
        response.Validate(Logger);
        IEnumerable<TRecord> records = response.GetRecords();
        return records;
    }

    protected async Task<ImmutableArray<TRecord>> GetImmutableArrayAsync(
        Action<SearchRequestDescriptor<TRecord>> configureSearch,
        CancellationToken cancellationToken = default
    )
        => (await GetManyAsync(configureSearch, cancellationToken)).ToImmutableArray();

    protected async Task<Page<TRecord>> GetPageAsync(
        Func<SearchRequestDescriptor<TRecord>, SearchRequestDescriptor<TRecord>> configureSearch,
        uint pageSize, uint pageNumber,
        CancellationToken cancellationToken = default
    )
    {
        SearchResponse<TRecord> response = await Client.SearchAsync<TRecord>(search
            => configureSearch(search)
                .ToPage(pageSize, pageNumber),
            cancellationToken
        );
        response.Validate(Logger);
        Page<TRecord> page = new(response.GetRecords(), (ulong)response.Total, pageSize);
        return page;
    }


}

public abstract class RecordRepository
{
    // Constants
    public const string Fuzziness_Automatic = "AUTO";
}
