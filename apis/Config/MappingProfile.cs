using System.Collections.Generic;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.Handlers;

namespace CoreJsNoise.Config
{
    using AutoMapper;
    public class MappingProfile : Profile {
        public MappingProfile() {
            CreateMap<Show, ShowParsedDto>();
            CreateMap<ShowParsedDto, Show>();
            CreateMap<List<ShowParsedDto>, List<Show>>();
            CreateMap<Producer, ProducersController.ProducerDto>();
            CreateMap<Show, ShowDto>();
            CreateMap<ShowsRequest, ShowsResponse>();


            //CoreJsNoise.Handlers.ShowsRequest,CoreJsNoise.Dto.ShowsResponse
        }
    }
}