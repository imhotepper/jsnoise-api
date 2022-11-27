using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using GraphQL.Types;

namespace CoreJsNoise.GraphQL.Types
{



    public class ShowType: ObjectGraphType<Show>
    {
        public ShowType()
        {
            Field((t => t.Id));
            Field((t => t.Title));
            Field((t => t.Mp3));
            Field(t => t.Producer.Name).Name("Producer");
            Field(t => t.PublishedDate);
            Field(t => t.ProducerId);
        }
    }
}