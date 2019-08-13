

using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using GraphQL.Types;

public class ProducerType: ObjectGraphType<Producer>{
    
    public ProducerType()
    {
        Field(t=>t.Id);
        Field(t=>t.Name);
        Field(t=>t.FeedUrl);
        Field(t=>t.Url);
    }
}