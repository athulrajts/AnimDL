﻿using AnimDL.Core.Helpers;
using AnimDL.Core.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace AnimDL.Core.StreamProviders;

internal class TenshiMoeStreamProvider : BaseStreamProvider
{
    readonly string BASE_URL = Constants.Tenshi;
    private readonly Regex _streamRegex = new("src: '(.*)',[\x00-\x7F]*?size: (\\d+)", RegexOptions.Compiled);
    private readonly ILogger<TenshiMoeStreamProvider> _logger;

    public TenshiMoeStreamProvider(ILogger<TenshiMoeStreamProvider> logger)
    {
        _logger = logger;
    }

    public override async IAsyncEnumerable<VideoStreamsForEpisode> GetStreams(string url)
    {
        var client = await BypassHelper.BypassDDoS(BASE_URL);
        var html = await client.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var count = int.Parse(doc.QuerySelector("span.badge").InnerText);

        foreach (var ep in Enumerable.Range(1, count - 1))
        {
            if (await ExtractUrls(client, $"{url}/{ep}") is VideoStreamsForEpisode streamForEp)
            {
                streamForEp.Episode = ep;
                yield return streamForEp;
            }
        }
    }

    private async Task<VideoStreamsForEpisode?> ExtractUrls(HttpClient client, string url)
    {
        var html = await client.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var embedStream = doc.QuerySelector("iframe")?.Attributes["src"].Value;

        if (string.IsNullOrEmpty(embedStream))
        {
            _logger.LogError("unable to find embed stream");
            return null;
        }

        html = await client.GetStringAsync(embedStream);

        var streamsForEp = new VideoStreamsForEpisode();
        foreach (Match match in _streamRegex.Matches(html))
        {
            var quality = match.Groups[2].Value;

            streamsForEp.Qualities.Add(quality, new VideoStream
            {
                Quality = quality,
                Url = match.Groups[1].Value
            });
        }

        return streamsForEp;
    }
}