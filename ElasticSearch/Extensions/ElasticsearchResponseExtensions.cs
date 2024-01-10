using AndrejKrizan.DotNet.Strings;

using Elastic.Transport.Products.Elasticsearch;

using Microsoft.Extensions.Logging;

namespace AndrejKrizan.ElasticSearch.Extensions;

public static class ElasticsearchResponseExtensions
{
    public static void Validate(this ElasticsearchResponse response, ILogger logger)
    {
        if (response.ElasticsearchWarnings.Any())
        {
            logger.LogWarning("Elasticsearch response containts warnings:\n{warnings}", response.ElasticsearchWarnings.StringJoin(separator: "\n"));
        }
        if (response.IsValidResponse)
        {
            return;
        }

        ElasticsearchServerError? serverError = response.ElasticsearchServerError;
        Exception? apiException = response.ApiCallDetails.OriginalException;
        if (serverError != null)
        {
            throw new Exception(serverError.ToString(), apiException);
        }
        if (apiException != null)
        {
            throw apiException;
        }
        throw new Exception("Unknown Elasticsearch error.");
    }
}
