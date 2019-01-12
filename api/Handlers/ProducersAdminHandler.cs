using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using MediatR;

namespace CoreJsNoise.Handlers
{
    public class ProducersForAdminRequest : IRequest<List<ProducerAggregateDto>>{}

    public class ProducersAdminHandler : IRequestHandler<ProducersForAdminRequest, List<ProducerAggregateDto>>
    {
        private readonly PodcastsCtx _db;

        public ProducersAdminHandler(PodcastsCtx db) => _db = db;

        public Task<List<ProducerAggregateDto>> Handle(ProducersForAdminRequest request,
            CancellationToken cancellationToken)
        {
            var resp = (from p in _db.Producers
                from s in _db.Shows
                where p.Id == s.ProducerId
                group p by new {Id = p.Id, Name = p.Name}
                into grp
                select new ProducerAggregateDto
                {
                    Name = grp.Key.Name, 
                    Id = grp.Key.Id, 
                    Count = grp.Count()
                })
                .ToList();

            return Task.FromResult(resp);
        }
    }
}