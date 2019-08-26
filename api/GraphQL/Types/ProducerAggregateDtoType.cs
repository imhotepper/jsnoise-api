using CoreJsNoise.Dto;
using GraphQL.Types;

public class ProducerAggregateDtoType: ObjectGraphType<ProducerAggregateDto>{
    public ProducerAggregateDtoType()
    {
        Field(t => t.Id);
        Field(t => t.Name);
        Field(t => t.Count);
    }
}