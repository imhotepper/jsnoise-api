using System.Threading;
using System.Threading.Tasks;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.Services;
using MediatR;

namespace CoreJsNoise.Handlers
{
    public class ProducerPostRequest: IRequest<Producer>
    {
        public Producer Producer { get; set; }
    }



    public class ProducerPostHandler : IRequestHandler<ProducerPostRequest, Producer>
    {
        private PodcastsCtx _db;
        private FeedUpdaterService _feedUpdater;


        public ProducerPostHandler(PodcastsCtx db, FeedUpdaterService feedUpdater)
        {
            _db = db;
            _feedUpdater = feedUpdater;
        }

        public Task<Producer> Handle(ProducerPostRequest request, CancellationToken cancellationToken)
        {
            _db.Producers.Add(request.Producer);
            _db.SaveChanges();
            _feedUpdater.UpdateShows(request.Producer);

           return  Task.FromResult(request.Producer);
        }
    }

}