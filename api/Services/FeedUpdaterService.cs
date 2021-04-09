using System;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using Microsoft.EntityFrameworkCore;

namespace CoreJsNoise.Services
{
    public class FeedUpdaterService
    {
        private PodcastsCtx _db;
        private RssReader _rssReader;

        public FeedUpdaterService(PodcastsCtx db, RssReader rssReader)
        {
            _db = db;
            _rssReader = rssReader;
        }

        public async void UpdateShows(Producer producer)
        {
            var items = await GetShowsAsync(producer);
            UpdateShows(producer,  items);
        }

        public async void UpdateAll()
        {
            var producers = _db.Producers.Where(x => !string.IsNullOrWhiteSpace(x.FeedUrl)).AsNoTracking().ToList();

            var toUpdate = new Dictionary<Producer, List<ShowParsedDto>>();

            Parallel.ForEach(producers, async (p) => toUpdate.Add(p, await GetShowsAsync(p)));

            foreach (var keyValuePair in toUpdate)
            {
                UpdateShows(keyValuePair.Key, keyValuePair.Value);
            }
        }


      async Task< List<ShowParsedDto>> GetShowsAsync(Producer producer)
        {
            var items = new List<ShowParsedDto>();
            try
            {
                items = await _rssReader.ParseAsync(producer.FeedUrl);

                Console.WriteLine("---------------------------");
                Console.WriteLine(producer.Name + " - parsed ok.");
                Console.WriteLine("---------------------------");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while updating feed: {producer.Name}: \n\r" + e.Message);
                Console.WriteLine(e);
            }

            return items;
        }


       async void UpdateShows(Producer producer, List<ShowParsedDto> items)
        {
            try
            {
                var itemsToSave = items.Select(x => new Show
                {
                    Title = x.Title,
                    Description = x.Description,
                    Mp3 = x.Mp3,
                    PublishedDate = x.PublishedDate ?? DateTime.Now
                }).ToList();
                itemsToSave.ForEach(s =>
                {
                    s.ProducerId = producer.Id;
                    if (!_db.Shows.Any(x => x.Title == s.Title)) _db.Shows.Add(s);
                });
                await _db.SaveChangesAsync();
                Console.WriteLine("---------------------------");
                Console.WriteLine(producer.Name + "- saved to db!");
                Console.WriteLine("---------------------------");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while updating feed: {producer.Name}: \n\r" + e.Message);
                Console.WriteLine(e);
            }
        }
    }
}