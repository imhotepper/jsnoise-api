using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using CoreJsNoise.Dto;

namespace CoreJsNoise.Services
{
    public class RssReader
    {
        public List<ShowParsedDto> Parse(string rssFeed)
        {
            var itemList = FeedReader.ReadAsync(rssFeed).Result.Items;
            Console.WriteLine("Parsing : " + rssFeed);

            var resp = new List<ShowParsedDto>();
            foreach (var item in itemList)
            {
                if ((!(item.SpecificItem is MediaRssFeedItem) ||
                     !(bool) (item.SpecificItem as MediaRssFeedItem)?.Enclosure?.Url?.Contains(".mp3")) &&
                    (!(item.SpecificItem is Rss20FeedItem) ||
                     !(bool) (item.SpecificItem as Rss20FeedItem)?.Enclosure?.Url?.Contains(".mp3"))) continue;

                var url = (item.SpecificItem as MediaRssFeedItem)?.Enclosure?.Url;
                if (string.IsNullOrWhiteSpace(url))
                    url = (item.SpecificItem as Rss20FeedItem)?.Enclosure?.Url;

                var mp3 = url.Substring(0, 3 + url.IndexOf("mp3"));

                resp.Add(new ShowParsedDto
                {
                    ShowId = item.Id,
                    Title = item.Title,
                    Mp3 = mp3,
                    PublishedDate = item.PublishingDate,
                    Description = item.Description
                });
            }

            return resp
                    .GroupBy(x => new {id = x.ShowId, title = x.Title})
                    .Select(x => x.First()).ToList();
        }
    }
}