
using CoreJsNoise.Dto;
using GraphQL.Types;

public class ShowDtoType: ObjectGraphType<ShowDto>{
    public ShowDtoType(){
            Field((t => t.Id));
            Field((t => t.Title));
            Field((t => t.Mp3));
            Field(t => t.ProducerName);
            Field(t => t.PublishedDate);
            Field(t => t.ProducerId);
            Field(t => t.Description);
    }
}