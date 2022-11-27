using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJsNoise.Domain;
using CoreJsNoise.Services;
using MediatR;

namespace CoreJsNoise.Handlers
{
    public class ShowsUpdateRequest : IRequest<int>
    {
    }

    public class ShowsUpdaterUpdateHandler: IRequestHandler<ShowsUpdateRequest,int>
    {
        private PodcastsCtx _db;
        private FeedUpdaterService _feedUpdater;


        public ShowsUpdaterUpdateHandler(PodcastsCtx db, FeedUpdaterService feedUpdater)
        {
            _db = db;
            _feedUpdater = feedUpdater;
        }

        public Task<int> Handle(ShowsUpdateRequest request, CancellationToken cancellationToken)
        {
            var count = _db.Shows.Count();
            _feedUpdater.UpdateAll();
            var result = _db.Shows.Count() - count;
            return Task.FromResult(result);
        }
    }
}