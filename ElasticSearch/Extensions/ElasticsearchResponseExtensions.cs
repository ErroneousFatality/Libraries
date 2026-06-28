using AndrejKrizan.DotNet.Strings;

using Elastic.Transport.Products.Elasticsearch;

using Microsoft.Extensions.Logging;

namespace AndrejKrizan.ElasticSearch.Extensions;

public static class ElasticsearchResponseExtensions
{
    extension(ElasticsearchResponse response)
    {
        /// <summary>Throws an exception if the response is not valid, preserving the Elasticsearch server and api call errors if they're present.</summary>
        /// <exception cref="Exception"></exception>
        public void Validate()
        {
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

        /// <summary>
        ///     Throws an exception if the response is not valid, preserving the Elasticsearch server and api call errors if they're present.
        ///     <br/>
        ///     Also logs any Elasticsearch warnings to the logger.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Validate(ILogger logger)
        {
            if (response.ElasticsearchWarnings.Any())
            {
                logger.LogWarning("Elasticsearch response contains warnings:\n{warnings}", response.ElasticsearchWarnings.StringJoin(separator: "\n"));
            }
            response.Validate();
        }
    }
    
}
