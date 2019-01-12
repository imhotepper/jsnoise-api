using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace CoreJsNoise.Handlers
{
    public class ShowsRequest : IRequest<ShowsResponse>
    {
        public int? Page { get; set; }
        public string Query { get; set; }
    }
    
    public class ShowsHandler : IRequestHandler<ShowsRequest, ShowsResponse>
    {
        private PodcastsCtx _db;

        public ShowsHandler(PodcastsCtx db) =>  _db = db;
        

        public Task<ShowsResponse> Handle(ShowsRequest request, CancellationToken cancellationToken) {
            var pageSize = 20;
            request.Query = request.Query?.ToLowerInvariant();
            IQueryable<Show> shows = _db.Shows.Include(x=>x.Producer);
            if (!string.IsNullOrWhiteSpace(request.Query))
                shows = shows.Where(x =>
                    x.Title.ToLower().Contains(request.Query) || x.Description.ToLower().Contains(request.Query));
            var counts = shows.Count();
            var resp = shows.OrderByDescending(x=>x.PublishedDate)
                .Skip(pageSize * ((request.Page ?? 1) - 1))
                .Take(pageSize)
                .Select(x => new ShowDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Mp3 = x.Mp3,
                    PublishedDate = x.PublishedDate,
                    ProducerId = x.ProducerId,
                    ProducerName = x.Producer.Name
                })
                .ToList();

            var totalPages = Math.Ceiling((double) counts / pageSize);
            var response = new ShowsResponse
            {
                Shows = resp,
                First = request.Page  == 1, 
                Last = totalPages == 0 ||  totalPages == request.Page    
            };
            
            return Task.FromResult(response);
        }
    }
    
    
}