using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using GraphQL.Types;
using System.Linq;

namespace CoreJsNoise.GraphQL.Types
{
    public class ShowsResponseType: ObjectGraphType<ShowsResponse>
    {
        public ShowsResponseType()
        {            
            Field((t => t.First));
            Field((t => t.Last));
           // Field((t => t.Shows,true));
            Field<ListGraphType<ShowDtoType>>(
               "shows",
               resolve: context => context.Source.Shows
           );
        }
    }
}