using Elastic.Clients.Elasticsearch;

namespace AndrejKrizan.ElasticSearch.Pagination;

public static class SearchRequestDescriptorExtensions
{
    public static SearchRequestDescriptor<T> ToPage<T>(this SearchRequestDescriptor<T> request, uint pageSize, uint pageNumber)
    {
        (int size, int from) = GetValidSizeAndFrom(pageSize, pageNumber);
        SearchRequestDescriptor<T> pageRequest = request.Size(size).From(from);
        return pageRequest;
    }

    public static SearchRequestDescriptor ToPage(this SearchRequestDescriptor request, uint pageSize, uint pageNumber)
    {
        (int size, int from) = GetValidSizeAndFrom(pageSize, pageNumber);
        SearchRequestDescriptor pageRequest = request.Size(size).From(from);
        return pageRequest;
    }

    // Private methods
    private static (int, int) GetValidSizeAndFrom(uint pageSize, uint pageNumber)
    {
        if (pageSize is < 1 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), $"Page size must be a positive integer not larger than int.MaxValue ({int.MaxValue}).");
        }
        if (pageNumber is < 1 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), $"Page number must be a positive integer not larger than int.MaxValue ({int.MaxValue}).");
        }
        ulong pageFrom = (ulong)pageSize * (pageNumber - 1);
        if (pageFrom > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                paramName: null,
                message: $"({nameof(pageNumber)} - 1) * {nameof(pageSize)} cannot be larger than int.MaxValue ({int.MaxValue})."
            );
        }
        return ((int)pageSize, (int)pageFrom);
    }
}
