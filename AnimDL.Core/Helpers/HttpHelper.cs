﻿using Microsoft.AspNetCore.WebUtilities;

namespace AnimDL.Core.Helpers;

internal static class HttpHelper
{
    const string CORS_PROXY = "https://corsproxy.io/";

    internal static async Task<string> PostFormUrlEncoded(this HttpClient client, string url, Dictionary<string, string> postData)
    {
        using var content = new FormUrlEncodedContent(postData);
        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        HttpResponseMessage response = await client.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    internal static async Task<string> GetStringAsync(this HttpClient client, string url, Dictionary<string,string>? parameters = null, Dictionary<string, string>? headers = null)
    {
        var actualUrl = parameters is null
            ? url
            : QueryHelpers.AddQueryString(url, parameters);

        if(headers is not null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, actualUrl);

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            var response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
        else 
        {
            return await client.GetStringAsync(actualUrl);
        }
    }

    internal static async Task<string> CfGetStringAsync(this HttpClient client, string url, Dictionary<string, string>? parameters = null, Dictionary<string,string>? headers = null)
    {
        if(headers is null)
        {
            headers = new();
        }

        headers.Add("referer", url);

        return await client.GetStringAsync(CORS_PROXY + "?" + url, parameters ?? new());
    }
}
