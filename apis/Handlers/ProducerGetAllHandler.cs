using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.Services;
using MediatR;

namespace CoreJsNoise.Handlers
{
    public class ProducerGetAllRequest: IRequest<ShowsResponse>
    {
        public string Query { get; set; }
        public int ProducerId { get; set; }
        public int? Page { get; set; }
    }

    public class ProducerGetAllHandler : IRequestHandler<ProducerGetAllRequest, ShowsResponse>
    {
        private PodcastsCtx _db;
        public ProducerGetAllHandler(PodcastsCtx db) => _db = db;

        public Task<ShowsResponse> Handle(ProducerGetAllRequest request, CancellationToken cancellationToken)
        {
            var pageSize = 20;
            request.Query = request.Query?.ToLowerInvariant();
            var shows = _db.Shows 
                            .Where(x => x.ProducerId == request.ProducerId);
            if (!string.IsNullOrWhiteSpace(request.Query))
                shows = shows.Where(x =>
                    x.Title.ToLower().Contains(request.Query) || x.Description.ToLower().Contains(request.Query));
            var counts = shows.Count();
            var resp = shows
                .OrderByDescending(x=>x.PublishedDate)
                .Skip(pageSize * ((request.Page??1) - 1))
                .Take(pageSize)
                .Select(x => new ShowDto()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Mp3 = x.Mp3,
                    PublishedDate = x.PublishedDate,
                    ProducerId = x.ProducerId,
                    ProducerName = x.Producer.Name
                }).ToList();

            var totalPages = Math.Ceiling((double) counts / pageSize);
            var response = new ShowsResponse
            {
                Shows = resp,
                First = request.Page == 1, 
                Last = totalPages == 0 ||  totalPages == request.Page   
            };

            return Task.FromResult(response);
        }
    }

}