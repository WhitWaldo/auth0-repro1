namespace DaprApiPoc.Extensions;

public static class HttpRequestMessageExtensions
{
    public static void AddQueryParameters(
        this HttpRequestMessage? message,
        Dictionary<string, string>? queryStringParameters)
    {
        ArgumentNullException.ThrowIfNull((object)message, nameof(message));
        if (queryStringParameters == null)
            return;
        UriBuilder uriBuilder = new UriBuilder(message.RequestUri);
        string str = uriBuilder.Query;
        foreach (KeyValuePair<string, string> queryStringParameter in queryStringParameters)
        {
            if (!string.IsNullOrEmpty(str))
                str += "&";
            str = str + Uri.EscapeDataString(queryStringParameter.Key) + "=" + Uri.EscapeDataString(queryStringParameter.Value);
        }
        uriBuilder.Query = str;
        message.RequestUri = uriBuilder.Uri;
    }
}