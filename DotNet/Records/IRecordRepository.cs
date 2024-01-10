using System.Collections.Immutable;

namespace AndrejKrizan.DotNet.Records;

public interface IRecordRepository<TRecord, TId>
    where TRecord : class
    where TId : notnull
{
    Task InsertOrUpdateAsync(TRecord record, CancellationToken cancellationToken = default);
    void ScheduleInsertOrUpdate(TRecord record);

    Task InsertOrUpdateManyAsync(IEnumerable<TRecord> records, CancellationToken cancellationToken = default);
    void ScheduleInsertOrUpdateMany(IEnumerable<TRecord> records);


    Task<TRecord?> GetAsync(TId id, CancellationToken cancellationToken = default);

    Task<ImmutableDictionary<TId, TRecord>> GetManyAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);


    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
    void ScheduleDelete(TId id);
}