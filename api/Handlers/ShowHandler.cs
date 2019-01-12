using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreJsNoise.Handlers
{
    
    public class ShowRequest : IRequest<ShowDto>
    {
        public int Id { get; set; }
    }
    
    public class ShowHandler : IRequestHandler<ShowRequest, ShowDto>
    {
        private PodcastsCtx _db;

        public ShowHandler(PodcastsCtx db) => _db = db;

        public Task<ShowDto> Handle(ShowRequest request, CancellationToken cancellationToken)
        {
            var show = _db.Shows.Include(x => x.Producer).FirstOrDefault(x => x.Id == request.Id);
            if (show == null) return null;

            var resp = new ShowDto()
            {
                Title = show.Title,
                ProducerId = show.ProducerId,
                ProducerName = show.Producer.Name,
                PublishedDate = show.PublishedDate,
                Mp3 = show.Mp3
            };
            return Task.FromResult(resp);
        }
    }
}